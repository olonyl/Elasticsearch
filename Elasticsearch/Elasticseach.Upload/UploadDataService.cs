using Elasticsearch.Service.DTO;
using Elasticsearch.Service.DTO.JSON;
using Elasticsearch.Service.Helper;
using Elasticsearch.Service.Interface;
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
        private readonly ISearchEngineService _searchEngineService;
        public UploadDataService(ISearchEngineService searchEngineService) => _searchEngineService = searchEngineService;

        public void UploadData()
        {

            var data = GetProperties();

            data.Add(new Building
            {
                Name = "San Andres",
                Formername = "San Andres",
                Market = "San Andres"
            });
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Uploading {data.Count} records into Elastic Search:Index - {ConstantsContainer.INDEXNAME}");

            _searchEngineService.UploadBuildings(data);
        }
        public static List<Building> GetProperties()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Reading files...");
            List<Building> result = new List<Building>();
            try
            {
                string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);

                var strMgmt = File.ReadAllText(@$"{_filePath}\Data\mgmt.json").Replace("\n", "").Replace("\r", "");
                var strProperties = File.ReadAllText(@$"{_filePath}\Data\properties.json").Replace("\n", "").Replace("\r", "");
                var management = JsonSerializer.Deserialize<List<Managements>>(strMgmt);
                var properties = JsonSerializer.Deserialize<List<Apartments>>(strProperties);

                var managementTransformation = management.Select(s => new Building
                {
                    Id = s.Management.Id,
                    Name = s.Management.Name,
                    Market = s.Management.Market,
                    State = s.Management.State,
                    Type = ConstantsContainer.TYPEMANAGEMENT
                }).ToList();
                result.AddRange(managementTransformation);

                var propertiesTransformation = properties.Select(s => new Building
                {
                    Id = s.Property.Id,
                    Name = s.Property.Name,
                    Market = s.Property.Market,
                    State = s.Property.State,
                    City = s.Property.City,
                    StreetAddress = s.Property.StreetAddress,
                    Formername = s.Property.FormerName,
                    Type = ConstantsContainer.TYPEPROPERTY
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
