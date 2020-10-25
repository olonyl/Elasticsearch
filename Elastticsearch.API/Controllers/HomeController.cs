using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Service.DTO;
using Elasticsearch.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Elastticsearch.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<HomeController> _logger;
        private readonly ISearchEngineService _elasticsearchService;
        public HomeController(ILogger<HomeController> logger, ISearchEngineService elasticsearchService)
        {
            _logger = logger;
            _elasticsearchService = elasticsearchService;
        }

        [HttpGet]
        public IEnumerable<SearchResult> Get()
        {
            var parameter = new SearchParameter
            {
                Market = String.Empty,
                Phrase = "Abilene"
            };
         return   _elasticsearchService.Search(parameter);
         
        }
    }
}
