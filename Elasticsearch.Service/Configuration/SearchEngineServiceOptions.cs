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

           var result =  Client.Indices.Create(ConstantsContainer.INDEXNAME, i => i
              .Settings(s => s
                  .NumberOfShards(2)
                  .NumberOfReplicas(0)
                  .Analysis(a=>a
                    .Analyzers(an=> an
                        .Custom("customanalyzer", ca=>ca
                            .Tokenizer("standard")
                            .Filters("my_custom_stop_words_filter", "snowball", "lowercase")
                        )
                        .Custom("marketanalyzer", c=>c
                            .Tokenizer("keyword")
                            .Filters("lowercase")
                        )
                    )
                    .TokenFilters(an=> an
                    .Stop("my_custom_stop_words_filter", s=> s
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
