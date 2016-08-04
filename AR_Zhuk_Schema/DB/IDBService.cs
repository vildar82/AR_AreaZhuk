using System.Collections.Generic;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_Schema.DB
{
    public interface IDBService
    {
        List<FlatInfo> GetSections (Section section, string type, string levels, SpotInfo sp, int maxSectionBySize);
    }
}