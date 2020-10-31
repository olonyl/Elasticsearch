using Elasticsearch.Service.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elasticserach.Service.Extension
{
    public static class ElasticsearchExtensions
    {
        public static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["ElasticserachOptions:Url"];
            var defaultIndex = configuration["ElasticserachOptions:Index"];

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex);

            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);

            CreateIndex(client, defaultIndex);

            return services.AddSingleton<IElasticClient>(client);

        }

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings.
                DefaultMappingFor<Building>(m => m
                .Ignore(p => p.Latitude)
                .Ignore(p => p.Longitud)
                .Ignore(p => p.Id)
                .Ignore(p => p.UniqueId)
            );
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            if (client.Indices.Exists(indexName).Exists)
                return;

            var createIndexResponse = client.Indices.Create(indexName, i => i
              .Settings(s => s
                  .NumberOfShards(2)
                  .NumberOfReplicas(0)
                  .Analysis(a => a
                    .Analyzers(an => an
                        .Custom("customanalyzer", ca => ca
                            .Tokenizer("standard")
                            .Filters("my_custom_stop_words_filter", "snowball", "lowercase")
                        )
                        .Custom("marketanalyzer", c => c
                            .Tokenizer("keyword")
                            .Filters("lowercase")
                        )
                    )
                    .TokenFilters(an => an
                    .Stop("my_custom_stop_words_filter", s => s
                           .StopWords("and", "or", "the", "into")
                    )
                    )
                  )
              )
              .Map<Building>(m => m
              .Properties(p => p
                               .Text(c => c
                                   .Name(n => n.Name)
                                   .Analyzer("customanalyzer")
                                   .SearchAnalyzer("customanalyzer")
                               )
                               .Text(c => c
                                   .Name(n => n.Market)
                                    .Analyzer("marketanalyzer")
                                   .SearchAnalyzer("marketanalyzer")
                               )
                                 .Text(c => c
                                   .Name(n => n.Formername)
                                   .Analyzer("customanalyzer")
                                   .SearchAnalyzer("customanalyzer")
                               )
                                 .Text(c => c
                                .Name(n => n.City)
                                .Analyzer("customanalyzer")
                                .SearchAnalyzer("customanalyzer")
                               )
                                   .Text(c => c
                                .Name(n => n.StreetAddress)
                                .Analyzer("customanalyzer")
                                .SearchAnalyzer("customanalyzer")
                               )
                                .Text(c => c
                                   .Name(n => n.State)
                                    .Analyzer("keyword")
                               )
                           )
              )
          );
        }
    }
}
