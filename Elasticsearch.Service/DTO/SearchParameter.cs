using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elasticsearch.Service.DTO
{
    public class SearchParameter
    {
        public int Limit { get; set; } = 25;
        public string Market { get; set; } = String.Empty;
        [Required]
        public string Phrase{ get; set; }
    }
}
