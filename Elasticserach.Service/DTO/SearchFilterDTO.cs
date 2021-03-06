﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Elasticsearch.Service.DTO
{
    public class SearchFilterDTO
    {
        /// <summary>
        /// Total of records to be returned
        /// </summary>
        public int Limit { get; set; } = 25;
        /// <summary>
        /// Market to search properties
        /// </summary>
        public List<String> Market { get; set; } = new List<string>();
        /// <summary>
        /// Phrase to search
        /// </summary>
        [Required]
        public string Phrase { get; set; } 
    }
}
