﻿using System;
using AR_AreaZhuk.DB;
using AR_AreaZhuk.Insolation;

namespace AR_AreaZhuk.Scheme.Cutting
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