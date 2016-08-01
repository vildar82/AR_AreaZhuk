using System;

namespace AR_AreaZhuk.Scheme.Cutting
{
    public static class CuttingFactory
    {
        public static ICutting Create (HouseSpot houseSpot)
        {
            ICutting cutting;

            if (houseSpot.IsTower)
            {
                cutting = null;                    
            }
            else
            {
                cutting = new CuttingOrdinary(houseSpot);
            }
            return cutting;
        }
    }
}