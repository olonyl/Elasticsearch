using Elasticsearch.Infrastructure.Extension;
using Elasticserach.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Elasticsearch.Infrastructure.Repositories
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger _logger;
        public BuildingRepository(
            IElasticClient elasticClient,
            ILogger<BuildingRepository> logger)
        {
            if (elasticClient == null)
            {
                logger?.LogError("Elastic Search Client DI has not been specified");
                throw new ArgumentNullException(nameof(elasticClient));
            }
            _elasticClient = elasticClient;
            _logger = logger;
        }
        public List<Building> FindBuildings(string phrase, List<string> market, int limit)
        {
            try
            {
                _logger?.LogInformation($"Starting search into Elasticsearch {JsonConvert.SerializeObject(new { phrase,market,limit })}...");
                var query = _elasticClient.Search<Building>(s => s
             .Index(nameof(Building).ToLower())
             .From(0)
             .Size(limit)
             .Query(q =>
                 q.Bool(b => b
                  .Should(market.Select(t => BuildPhraseQueryContainer(q, t, 1)).ToArray())
                 ) && q.MultiMatch(mm => mm
                        .Fields(fs => fs.Field(p => p.Name).Field(p => p.Formername).Field(p => p.StreetAddress))
                         .Query(phrase)
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
             .Index(nameof(Building).ToLower())
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
