using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elasticsearch.Service.DTO
{
    public class SearchFilter
    {
        /// <summary>
        /// Total of records to be returned
        /// </summary>
        public int Limit { get; set; } = 25;
        /// <summary>
        /// Market to search properties
        /// </summary>
        public string Market { get; set; } = String.Empty;
        /// <summary>
        /// Phrase to search
        /// </summary>
        public string Phrase { get; set; } 
    }
}
