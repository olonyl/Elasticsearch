using Elasticsearch.Service.DTO;
using Elasticsearch.Service.DTO.JSON;
using Elasticsearch.Service.Externsion;
using Elasticsearch.Service.Helper;
using Elasticsearch.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
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
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceProvider = new ServiceCollection()
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
                var uploadService = new UploadDataService(service);
                uploadService.UploadData();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("This console will close in a 5 seconds.");
                Thread.Sleep(5000);
            }
        }
       
    }
}
