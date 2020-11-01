using System;
using System.Collections.Generic;
using System.Text;

namespace Elasticserach.Domain.Interfaces
{
    public interface IBuildingRepository
    {
        List<Building> FindBuildings(string phrase, List<string> market, int limit);
        void UploadBuildings(List<Building> data);
    }
}
