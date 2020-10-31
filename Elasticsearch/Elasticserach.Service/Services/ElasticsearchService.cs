using Elasticsearch.Service.DTO;
using Elasticserach.Service.Extension;
using Elasticserach.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elasticserach.Service.Services
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger _logger;
        private readonly string _Index;
        public ElasticsearchService(IElasticClient elasticClient,
            ILogger<ElasticsearchService> logger,
            IOptions<ElasticserachOptions> options)
        {
            _elasticClient = elasticClient;
            _logger = logger;
            _Index = options.Value.Index;
        }

        public List<Building> Search(SearchFilter filter)
        {
            try
            {
                _logger?.LogInformation($"Starting search into Elasticsearch {JsonConvert.SerializeObject(filter)}...");
                var query = _elasticClient.Search<Building>(s => s
             .Index(_Index)
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
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public void UploadBuildings(List<Building> data)
        {
            try
            {
                _logger?.LogInformation($"Starting to upload {data.Count} records into Elasticsearch...");
                var bulkAll = _elasticClient.BulkAll(data, b => b
              .Index(_Index)
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
            catch (Exception ex)
            {
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
