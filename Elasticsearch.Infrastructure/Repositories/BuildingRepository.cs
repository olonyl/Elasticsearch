using Elasticserach.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elasticsearch.Infrastructure.Repositories
{
    public class BuildingRepository : IBuildingRepository
    {
        public BuildingRepository()
        {

        }
        public List<Building> FindBuildings(string phrase, List<string> market, int Limit)
        {
            throw new NotImplementedException();
        }

        public void UploadBuildings(List<Building> data)
        {
            throw new NotImplementedException();
        }
    }
}
