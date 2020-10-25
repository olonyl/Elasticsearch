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
        public HomeController(ILogger<HomeController> logger, 
            ISearchEngineService elasticsearchService)
        {
            _logger = logger;
            _elasticsearchService = elasticsearchService;
        }

        [HttpGet]
        public IActionResult Get(SearchFilter filter)
        {
            try
            {
                var data = _elasticsearchService.Search(filter);
                if (data == null) return NotFound();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult Post(List<Building> data)
        {
            try
            {
                _elasticsearchService.UploadBuildings(data);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
