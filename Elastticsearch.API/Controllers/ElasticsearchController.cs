using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Service.DTO;
using Elasticserach.Service.Interfaces;
using Elastticsearch.API.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Elastticsearch.API.Controllers
{
   
    public class ElasticsearchController : BaseController
    {
     
        private readonly ILogger<ElasticsearchController> _logger;
        private readonly IElasticsearchService _elasticsearchService;
        public ElasticsearchController(ILogger<ElasticsearchController> logger,
            IElasticsearchService elasticsearchService)
        {
            _logger = logger;
            _elasticsearchService = elasticsearchService;
        }
        /// <summary>
        /// Return a List  of Properties Based on some parameters    
        /// </summary>
        /// <param name="filter">Filters to be used to search the Properties</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize("read:properties")]
        public IActionResult Get([FromQuery] SearchFilter filter)
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

        /// <summary>
        /// This method is used to insert data into Elasticsearch
        /// </summary>
        /// <param name="data">Information to be inserted</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize("write:properties")]
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
