using Elasticsearch.Service.Configuration;
using Elasticsearch.Service.Interface;
using Elasticsearch.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elasticsearch.Service.Externsion
{
    public static class SearchEngineServiceCollectionExtensions
    {
        public static IServiceCollection AddSearchEngineService(this IServiceCollection collection,  Action<SearchEngineServiceOptions> options)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (options == null) throw new ArgumentNullException(nameof(options));

            collection.Configure(options);
            return collection.AddTransient<ISearchEngineService, SearchEngineService>();
        }
    }
}
