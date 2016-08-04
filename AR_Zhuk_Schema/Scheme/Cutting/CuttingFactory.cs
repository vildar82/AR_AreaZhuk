using System;
using AR_Zhuk_DataModel;
using AR_Zhuk_Schema.DB;
using AR_Zhuk_Schema.Insolation;

namespace AR_Zhuk_Schema.Scheme.Cutting
{
    public static class CuttingFactory
    {
        public static ICutting Create (HouseSpot houseSpot, SpotInfo sp, int maxSectionbySize)
        {
            ICutting cutting;
            IInsolation insService = new InsolationSection(sp);
            IDBService dbService = new DBService(sp, maxSectionbySize);

            if (houseSpot.IsTower)
            {
                cutting = null;                    
            }
            else
            {
                cutting = new CuttingOrdinary(houseSpot, dbService, insService, sp);
            }
            return cutting;
        }
    }
}