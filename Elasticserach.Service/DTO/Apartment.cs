using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Elasticsearch.Service.DTO
{
    public class Apartment
    { 
        [JsonPropertyName("propertyID")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("formerName")]
        public string FormerName { get; set; }

        [JsonPropertyName("streetAddress")]
        public string StreetAddress { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("market")]
        public string Market { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("lat")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("lng")]
        public decimal Longitude { get; set; }
    }
}
