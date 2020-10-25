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

namespace Elasticsearch.Service.Services
{
    public class SearchEngineService : ISearchEngineService
    {
        private readonly string _Url;
        private ElasticClient _client;

        public SearchEngineService(IOptions<SearchEngineServiceOptions> options) {
            _Url = options.Value.Url;
            InitializeClient();
            }

        private void InitializeClient()
        {
            var settings = new ConnectionSettings(new Uri(_Url));
            _client = new ElasticClient(settings);
        }
        public List<SearchResult> Search(SearchParameter parameter)
        {
            var result = new List<SearchResult>();

            var managements = GetManagements(parameter);
            var properties =GetProperties(parameter);
         
            result.AddRange(managements);
            result.AddRange(properties);


            return result;
        }

        private List<SearchResult> GetManagements(SearchParameter parameter)
        {
            var result = _client.Search<Management>(s => s
            .Index(ConstantsContainer.INDEXMANAGEMENT)
            .From(0)
            .Size(parameter.Limit)
            .Query(q => q
                 .MultiMatch(m => m
                    .Fields(fs => fs.Field(p => p.Market))
                    .Query(parameter.Phrase)
                 )
            )
       ).Documents.ToList();
            var transformedResult = result.Select(s => new SearchResult
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Market = s.Market,
                State = s.State,
                Type= ConstantsContainer.TYPEMANAGEMENT
            }).ToList();
            return transformedResult;
        }
        private List<SearchResult> GetProperties(SearchParameter parameter) {

            var result = _client.Search<Apartment>(s => s
            .Index(ConstantsContainer.INDEXPROPERTY)
            .From(0)
            .Size(parameter.Limit)
            .Query(q => q
                 .MultiMatch(m => m
                    .Fields(fs => fs.Field(p => p.Market))
                    .Query(parameter.Phrase)
                 )
            )
       ).Documents.ToList();
            var transformedResult = result.Select(s => new SearchResult
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Market = s.Market,
                City = s.City,
                Forename = s.FormerName,
                State=s.State,
                StreetAddress =s.StreetAddress,
                Type = ConstantsContainer.TYPEPROPERTY
            }).ToList();
            return transformedResult;
        }
    }
}
