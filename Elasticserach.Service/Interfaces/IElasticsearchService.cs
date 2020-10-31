using Elasticsearch.Service.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elasticserach.Service.Interfaces
{
    public interface IElasticsearchService
    {
        List<Building> Search(SearchFilter filter);
        void UploadBuildings(List<Building> data);
    }
}
