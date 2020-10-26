using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth0API.Options
{
    public class SwaggerOptions
    {
        public string JsonRoute { get; set; }
        public string Description { get; set; }
        public string UIEndpoint { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactProfile { get; set; }

    }
}
