using Elasticsearch.Service.Configuration;
using Elasticsearch.Service.DTO;
using Elasticsearch.Service.Helper;
using Elasticsearch.Service.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
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

        public SearchEngineService(IOptions<SearchEngineServiceOptions> options)
        {
            _client = options.Value.Client;
        }

        public List<Building> Search(SearchFilter filter)
        {
            var result = _client.Search<Building>(s => s
          .Index(ConstantsContainer.INDEXNAME)
          .From(0)
          .Size(filter.Limit)
          .Query(q => q
               .MultiMatch(m => m
                  .Fields(fs => fs.Field(p => p.Market))
                  .Query(filter.Market)
               )
            )
           ).Documents.ToList();
            return result;
        }
        public void UploadBuildings(List<Building> data)
        {
            // _client.IndexMany(data, ConstantsContainer.INDEXNAME);
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
                onNext: (b) => { Console.Write("."); },
                onError: (e) => { throw e; },
                onCompleted: () => {
                    Console.WriteLine();
                    waitHandle.Signal();
                }
                )); ;
            waitHandle.Wait();
        }

    }
}
