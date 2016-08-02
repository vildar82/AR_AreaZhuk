using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_InsSchema.Insolation
{
    interface IInsCheck
    {        
        bool CheckSection (FlatInfo sect, bool isRightOrTopLLu);
    }
}
