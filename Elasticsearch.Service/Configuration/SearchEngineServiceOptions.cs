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
        public ElasticClient Client { get; set; }
        public void Congifure()
        {
            CreateIndex();
        }

        private void CreateIndex()
        {
            if (Client.Indices.Exists(ConstantsContainer.INDEXNAME).Exists)
                return;

            Client.Indices.Create(ConstantsContainer.INDEXNAME, i => i
              .Settings(s => s
                  .NumberOfShards(2)
                  .NumberOfReplicas(0)
              )
              .Map<Building>(m => m
              .Properties(p => p
                               .Text(c => c
                                   .Name(n => n.Name)
                                   //.Analyzer("snowball")
                                   //.SearchAnalyzer("snowball")
                               )
                               .Text(c => c
                                   .Name(n => n.Market)
                                   // .Analyzer("keyworkd")
                                   //.SearchAnalyzer("keyworkd")                                   
                               )
                                 .Text(c => c
                                   .Name(n => n.Formername)
                                   //.Analyzer("snowball")
                                   //.SearchAnalyzer("snowball")
                               )
                                 .Text(c => c
                                .Name(n => n.City)
                                //.Analyzer("snowball")
                                //.SearchAnalyzer("snowball")
                               )
                                   .Text(c => c
                                .Name(n => n.StreetAddress)
                                //.Analyzer("snowball")
                                //.SearchAnalyzer("snowball")
                               )
                                .Text(c => c
                                   .Name(n => n.State)
                                   // .Analyzer("keyworkd")
                                   //.SearchAnalyzer("keyworkd")
                               )
                           )
              )
          );
        }
    }
}
