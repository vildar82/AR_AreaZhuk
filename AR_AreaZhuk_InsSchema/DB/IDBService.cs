using System.Collections.Generic;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_InsSchema.DB
{
    public interface IDBService
    {
        List<FlatInfo> GetSections (Section section, string type, string levels, SpotInfo sp);
    }
}