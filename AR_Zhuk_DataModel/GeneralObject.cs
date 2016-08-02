using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_Zhuk_DataModel
{
    public class GeneralObject
    {
        public string GUID { get; set; }
        public List<HouseInfo> Houses = new List<HouseInfo>();
        public SpotInfo SpotInf { get; set; }

    }
}
