using AR_Zhuk_DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AR_AreaZhuk.Insolation
{
    class InsolationFrameWork
    {
        public List<RoomInfo> GetTopFlatsInSection (List<RoomInfo> sectionFlats, bool isTop)
        {
            List<RoomInfo> topFlats = new List<RoomInfo>();

            if (isTop)
            {
                int indexFirstBottomFlat = 0;
                for (int i = 0; i < sectionFlats.Count; i++)
                {
                    indexFirstBottomFlat = i;
                    if (sectionFlats[i].SelectedIndexTop == 0)
                        break;
                }                

                for (int i = indexFirstBottomFlat; i < sectionFlats.Count; i++)
                {
                    if (sectionFlats[i].SelectedIndexTop == 0) continue;
                    topFlats.Add(sectionFlats[i]);
                }

                for (int i = 0; i < indexFirstBottomFlat; i++)
                {
                    if (sectionFlats[i].SelectedIndexTop == 0) continue;
                    topFlats.Add(sectionFlats[i]);
                }
            }
            else
            {
                for (int i = 0; i < sectionFlats.Count; i++)
                {
                    if (sectionFlats[i].SelectedIndexTop != 0) continue;
                    topFlats.Add(sectionFlats[i]);
                }
            }
                        
            return topFlats;
        }
    }
}
