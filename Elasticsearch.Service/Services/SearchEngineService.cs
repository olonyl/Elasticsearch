using Elasticsearch.Service.Configuration;
using Elasticsearch.Service.DTO;
using Elasticsearch.Service.Helper;
using Elasticsearch.Service.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;

namespace Elasticsearch.Service.Services
{
    public class SearchEngineService : ISearchEngineService
    {
        private ElasticClient _client;
        private readonly ILogger<SearchEngineService> _logger;

        public SearchEngineService(IOptions<SearchEngineServiceOptions> options, 
            ILogger<SearchEngineService> logger = null)
        {
            _client = options.Value.Client;
            _logger = logger;
        }

     
        public List<Building> Search(SearchFilter filter)
        {
            try
            {
                _logger?.LogInformation($"Starting search into Elasticsearch {JsonConvert.SerializeObject(filter)}...");
                var query = _client.Search<Building>(s => s
             .Index(ConstantsContainer.INDEXNAME)
             .From(0)
             .Size(filter.Limit)
             .Query(q =>
                 q.Bool(b => b
                  .Should(filter.Market.Select(t => BuildPhraseQueryContainer(q, t, 1)).ToArray())
                 ) && q.MultiMatch(mm => mm
                        .Fields(fs => fs.Field(p => p.Name).Field(p => p.Formername).Field(p => p.StreetAddress))
                         .Query(filter.Phrase)
                        )
                     )
                 );

                var result = query.Documents.ToList();

                _logger?.LogInformation($"The search result was {result.Count} records");

                return result;
            }
            catch (Exception ex) {
                _logger?.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
          
        }
        public void UploadBuildings(List<Building> data)
        {
            try
            {
                _logger?.LogInformation($"Starting to upload {data.Count} records into Elasticsearch...");
                var bulkAll = _client.BulkAll(data, b => b
             .Index(ConstantsContainer.INDEXNAME) /* index */
             .BackOffRetries(2)
             .BackOffTime("30s")
             .RefreshOnCompleted(true)
             .MaxDegreeOfParallelism(4)
             .Size(1000)
         );
                var waitHandle = new CountdownEvent(1);
                bulkAll.Subscribe(new BulkAllObserver(
                    onNext: (b) => { 
                    //This is just to display the progress onto the windows console, remember this application can be something else
                        Console.Write(".");
                        _logger?.LogInformation($"{b.Items.Count} records to be uploaded");
                    },
                    onError: (e) => { throw e; },
                    onCompleted: () =>
                    {
                        waitHandle.Signal();
                    }
                    )); ;
                waitHandle.Wait();
            }
            catch (Exception ex) {
                _logger?.LogError(ex.Message);
            }
        
        }

        #region Helper
        QueryContainer BuildPhraseQueryContainer(QueryContainerDescriptor<Building> qd, string term, int slop)
        {
            return qd.MatchPhrase(m => m.Field(f => f.Market).Query(term.ToLower()).Slop(slop));
        }
        #endregion

    }
}
