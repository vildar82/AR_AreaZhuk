using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_AreaZhuk.PIK1TableAdapters;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.DB
{
    public class DBService : IDBService
    {
        private static Dictionary<string, List<FlatInfo>> dictSections = new Dictionary<string, List<FlatInfo>>();
        public List<FlatInfo> GetSections (int countStep, string type, string levels)
        {
            List<FlatInfo> resFlats;
            string key = countStep + type + levels;

            if (!dictSections.TryGetValue(key, out resFlats))
            {
                FlatsInSectionsTableAdapter flatsIsSection = new FlatsInSectionsTableAdapter();
                var flatsDb = flatsIsSection.GetFlatsInTypeSection(countStep, type, levels).ToList();

                resFlats = new List<FlatInfo>();

                FlatInfo flat = null;
                long lastIdSection = 0;
                foreach (var flatDb in flatsDb)
                {
                    if (flatDb.ID_Section != lastIdSection)
                    {
                        lastIdSection = flatDb.ID_Section;
                        flat = new FlatInfo();
                        flat.IdSection = flatDb.ID_Section;
                        flat.CountStep = countStep;
                        resFlats.Add(flat);
                    }
                    var room = new RoomInfo(flatDb.ShortType, flatDb.SubZone, flatDb.TypeFlat, flatDb.AreaLive.ToString(),
                           flatDb.AreaTotalStandart.ToString(),
                           flatDb.AreaTotalStrong.ToString(), flatDb.CountModules.ToString(), "",
                           "", flatDb.LinkageBefore, flatDb.LinkageAfter, "", "", "",
                           flatDb.Levels, "", "", flatDb.LightBottom, flatDb.LightTop, ""
                           );
                    room.SelectedIndexTop = flatDb.SelectedIndexTop;
                    room.SelectedIndexBottom = flatDb.SelectedIndexBottom;
                    flat.Flats.Add(room);

                    // Только для тестов!!! Ограничение количества секций
                    if (resFlats.Count > 1000)
                    {
                        break;
                    }
                }
                dictSections.Add(key, resFlats);
            }
            return resFlats;
        }
    }
}