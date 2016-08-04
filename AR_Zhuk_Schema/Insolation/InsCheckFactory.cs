using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_Schema.Insolation
{
    static class InsCheckFactory
    {
        public static IInsCheck CreateInsCheck (IInsolation insService, Section section)
        {
            IInsCheck insCheck = null;
            if(section.IsCorner)
            {
                insCheck = new InsCheckCorner(insService, section);
            }
            else
            {
                insCheck = new InsCheckOrdinary(insService, section);
            }

            return insCheck;
        }        
    }
}
