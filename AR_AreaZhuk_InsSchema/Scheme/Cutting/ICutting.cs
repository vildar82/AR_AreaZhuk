using System.Collections.Generic;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Scheme.Cutting
{
    public interface ICutting
    {
        List<Section> Cut ();
    }
}