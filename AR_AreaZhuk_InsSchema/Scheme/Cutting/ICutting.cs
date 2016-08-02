using System.Collections.Generic;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_InsSchema.Scheme.Cutting
{
    public interface ICutting
    {
        List<Section> Cut ();
    }
}