using Elasticsearch.Service.DTO;
using Elasticsearch.Service.Helper;
using Microsoft.VisualBasic;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elasticsearch.Service.Configuration
{
    public class SearchEngineServiceOptions
    {
        public string Url { get; set; }
        public void Congifure()
        {

            var settings = new ConnectionSettings(new Uri(Url))
                .EnableDebugMode();
            var client = new ElasticClient(settings);

            CreateManagementIndex(client);
            CreateProperyIndex(client);
        }

        private void CreateManagementIndex(ElasticClient client) {
            if (client.Indices.Exists(ConstantsContainer.INDEXMANAGEMENT).Exists)
                return;

            client.Indices.Create(ConstantsContainer.INDEXMANAGEMENT, i => i
              .Settings(s => s
                  .NumberOfShards(2)
                  .NumberOfReplicas(0)
              )
              .Map<Management>(m => m
              .Properties(p => p
                               .Text(c => c
                                   .Name(n => n.Name)
                                   .Analyzer("snowball")
                                   .SearchAnalyzer("snowball")
                               )
                               .Text(c => c
                                   .Name(n => n.Market)
                                   .Analyzer("snowball")
                                   .SearchAnalyzer("snowball")
                               )
                                .Text(c => c
                                   .Name(n => n.State)
                               )
                           )
              )
          );
        }
        private void CreateProperyIndex(ElasticClient client)
        {
            if (client.Indices.Exists(ConstantsContainer.INDEXPROPERTY).Exists)
                return;

            client.Indices.Create(ConstantsContainer.INDEXPROPERTY, i => i
              .Settings(s => s
                  .NumberOfShards(2)
                  .NumberOfReplicas(0)
              )
              .Map<Apartment>(m => m
              .Properties(p => p
                               .Text(c => c
                                   .Name(n => n.Name)
                                   .Analyzer("snowball")
                                   .SearchAnalyzer("snowball")
                               )
                               .Text(c => c
                                   .Name(n => n.Market)
                                   .Analyzer("snowball")
                                   .SearchAnalyzer("snowball")
                               )
                                 .Text(c => c
                                   .Name(n => n.FormerName)
                                   .Analyzer("snowball")
                                   .SearchAnalyzer("snowball")
                               )
                                 .Text(c => c
                                .Name(n => n.City)
                                .Analyzer("snowball")
                                .SearchAnalyzer("snowball")
                               )
                                .Text(c => c
                                   .Name(n => n.State)
                               )
                           )
              )
          );
        }
    }
}
