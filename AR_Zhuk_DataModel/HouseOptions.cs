using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_Zhuk_DataModel
{
    /// <summary>
    /// Параметры дома
    /// </summary>
    public class HouseOptions
    {
        public string HouseName { get; set; }
        /// <summary>
        /// Основная этажность секций
        /// </summary>
        public int CountFloorsMain { get; set; }
        /// <summary>
        /// Этажность доминантных секций
        /// </summary>
        public int CountFloorsDominant { get; set; }
        /// <summary>
        /// Позиции доминантных секций - 1,2,3, предпоследняя, последняя
        /// </summary>
        public List<bool> DominantPositions { get; set; }

        public HouseOptions(string houseName, int floorsMain, int floorsDominant, List<bool> dominantPos)
        {
            HouseName = houseName;
            CountFloorsMain = floorsMain;
            CountFloorsDominant = floorsDominant;
            DominantPositions = dominantPos;
        }
    }
}
