using System;
using AR_Zhuk_DataModel;
using AR_Zhuk_InsSchema.DB;
using AR_Zhuk_InsSchema.Insolation;

namespace AR_Zhuk_InsSchema.Scheme.Cutting
{
    public static class CuttingFactory
    {
        public static ICutting Create (HouseSpot houseSpot, SpotInfo sp)
        {
            ICutting cutting;
            IInsolation insService = new InsolationSection(sp);
            IDBService dbService = new DBService();

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