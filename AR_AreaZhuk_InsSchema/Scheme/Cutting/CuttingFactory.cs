using System;
using AR_AreaZhuk.DB;
using AR_Zhuk_InsSchema.Insolation;

namespace AR_Zhuk_InsSchema.Scheme.Cutting
{
    public static class CuttingFactory
    {
        public static ICutting Create (HouseSpot houseSpot)
        {
            ICutting cutting;
            IInsolation insService = new InsolationSection();
            IDBService dbService = new DBService();

            if (houseSpot.IsTower)
            {
                cutting = null;                    
            }
            else
            {
                cutting = new CuttingOrdinary(houseSpot, dbService, insService);
            }
            return cutting;
        }
    }
}