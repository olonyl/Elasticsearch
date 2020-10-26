﻿using Elasticsearch.Service.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elasticsearch.Service.Interface
{
    public interface ISearchEngineService
    {
        List<Building> Search(SearchFilter parameter);
        void UploadBuildings(List<Building> data);
    }
}
