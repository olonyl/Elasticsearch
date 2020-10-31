using System;
using System.Collections.Generic;
using System.Text;

namespace Elasticserach.Service.Extension
{
    public class ElasticserachOptions
    {
        public string Url { get; set; }
        public string Index { get; set; } = "default";
    }
}
