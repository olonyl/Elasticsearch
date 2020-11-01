using Elasticsearch.Service.DTO;
using Elasticserach.Domain.Interfaces;
using Elasticserach.Service.Interfaces;
using Elasticserach.Service.SeedWork;
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
        private readonly ILogger _logger;
        private readonly IBuildingRepository _buildingRepository;
        public ElasticsearchService(IBuildingRepository buildingRepository,
            ILogger<ElasticsearchService> logger)
        {
            if (buildingRepository == null) {
                logger?.LogError("IBuildingRepository has not been specified");
                throw new ArgumentNullException(nameof(buildingRepository));
            }
            _logger = logger;
            _buildingRepository = buildingRepository;
        }

        public List<BuildingDTO> Search(SearchFilterDTO filter)
        {
            try
            {
                _logger?.LogInformation($"Starting search into Elasticsearch {JsonConvert.SerializeObject(filter)}...");
            
                var result = _buildingRepository.FindBuildings(filter.Phrase,filter.Market,filter.Limit);

                _logger?.LogInformation($"The search result was {result.Count} records");

                return result.ProjectedAsCollection<BuildingDTO>();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public void UploadBuildings(List<BuildingDTO> data)
        {
            try
            {
                _logger?.LogInformation($"Uplading {data.Count} records");
                _buildingRepository.UploadBuildings(data.ProjectedAsCollection<Building>());
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
