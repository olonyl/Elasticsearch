using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Elasticseach.Upload.DTO
{
    public class Management
    {
        [JsonPropertyName("mgmtID")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("market")]
        public string Market { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }
        
    }
}
