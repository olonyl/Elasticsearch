using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Elasticseach.Upload.DTO.JSON
{
    public class Managements
    {
        [JsonPropertyName("mgmt")]
        public Management Management { get; set; }
    }
}
