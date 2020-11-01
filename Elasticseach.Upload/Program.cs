using Elasticsearch.Infrastructure.Adapter;
using Elasticsearch.Infrastructure.Extension;
using Elasticsearch.Infrastructure.Repositories;
using Elasticserach.Domain.Interfaces;
using Elasticserach.Service.Interfaces;
using Elasticserach.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.IO;
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

            var services = new ServiceCollection()
            .AddLogging(configure => configure.AddSerilog());

            services.AddElasticsearch(Configuration);
            services.AddScoped<IElasticsearchService, ElasticsearchService>();
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddSingleton<ITypeAdapterFactory, AutomapperTypeAdapterFactory>();

     
            var serviceProvider = services.BuildServiceProvider();

            var typeAdapterFactory = serviceProvider.GetService<ITypeAdapterFactory>();
            if (typeAdapterFactory == null) throw new Exception("TypeAdapterFactory has not been added");
            TypeAdapterFactory.SetCurrent(typeAdapterFactory);


            var service = serviceProvider.GetService<IElasticsearchService>();
            UploadData(service);
        }
        public static void UploadData(IElasticsearchService service)
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
