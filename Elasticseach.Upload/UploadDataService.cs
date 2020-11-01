using Elasticseach.Upload.DTO.JSON;
using Elasticsearch.Service.DTO;
using Elasticserach.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Elasticseach.Upload
{
    public class UploadDataService
    {
        private readonly IElasticsearchService _searchEngineService;
        public UploadDataService(IElasticsearchService searchEngineService) => _searchEngineService = searchEngineService;

        public void UploadData()
        {

            var data = GetProperties();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Uploading {data.Count} records into Elastic Search");

            _searchEngineService.UploadBuildings(data);
        }
        public static List<BuildingDTO> GetProperties()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Reading files...");
            List<BuildingDTO> result = new List<BuildingDTO>();
            try
            {
                string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);

                var strMgmt = File.ReadAllText(@$"{_filePath}\Data\mgmt.json").Replace("\n", "").Replace("\r", "");
                var strProperties = File.ReadAllText(@$"{_filePath}\Data\properties.json").Replace("\n", "").Replace("\r", "");
                var management = JsonSerializer.Deserialize<List<Managements>>(strMgmt);
                var properties = JsonSerializer.Deserialize<List<Apartments>>(strProperties);

                var managementTransformation = management.Select(s => new BuildingDTO
                {
                    Id = s.Management.Id,
                    Name = s.Management.Name,
                    Market = s.Management.Market,
                    State = s.Management.State,
                    Type = "M"
                }).ToList();
                result.AddRange(managementTransformation);

                var propertiesTransformation = properties.Select(s => new BuildingDTO
                {
                    Id = s.Property.Id,
                    Name = s.Property.Name,
                    Market = s.Property.Market,
                    State = s.Property.State,
                    City = s.Property.City,
                    StreetAddress = s.Property.StreetAddress,
                    Formername = s.Property.FormerName,
                    Type = "P"
                }).ToList();
                result.AddRange(propertiesTransformation);

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error Uploading Files");
                Console.WriteLine(ex.Message);
            }
            return result;
        }
    }
}
