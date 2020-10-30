using Elasticsearch.Service.DTO;
using Elasticsearch.Service.DTO.JSON;
using Elasticsearch.Service.Externsion;
using Elasticsearch.Service.Helper;
using Elasticsearch.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Threading;

namespace Elasticseach.Upload
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
           .Enrich.FromLogContext()
            .WriteTo.Console(new RenderedCompactJsonFormatter())
             .WriteTo.File(new RenderedCompactJsonFormatter(), "/logs/elasticsearch.upload.log.ndjson")
           .CreateLogger();

            try
            {
                Log.Information("Starting up");
                InitializeApplication();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }                      
        }
        public static void InitializeApplication() {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddSerilog())
            .AddSearchEngineService(o =>
            {
                var url = Configuration.GetSection("Elasticsearch:Url").Value;
                var settings = new ConnectionSettings(new Uri(url)).EnableDebugMode();
                o.Client = new ElasticClient(settings);
                o.Congifure();
            }
          ).BuildServiceProvider();
            var service = serviceProvider.GetService<ISearchEngineService>();
            UploadData(service);
        }
        public static void UploadData(ISearchEngineService service)
        {
            try {
                Log.Information("Starting Procces to Upload Files into Elasticserach");
                var uploadService = new UploadDataService(service);
                uploadService.UploadData();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
            }
            finally
            {
                Log.Information("This console will close in a 5 seconds.");
                Log.Information("Process to upload files into Elasticserach has finished");
                Thread.Sleep(5000);
            }
        }
       
    }
}
