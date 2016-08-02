using System.Collections.Generic;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.DB
{
    public interface IDBService
    {
        List<FlatInfo> GetSections (int countStep, string type, string levels);
    }
}