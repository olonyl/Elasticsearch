using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elasticsearch.Service.DTO
{
    public class Building
    {
        public string Id { get; set; }
        public int OriginalId { get; set; }
        public string Name { get; set; }
        public string Market { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Formername { get; set; }
        public string Type { get; set; }
        public string StreetAddress { get; set; }
    }
}
