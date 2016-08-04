using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;
using AR_Zhuk_Schema.DB.SAPRTableAdapters;
using static AR_Zhuk_Schema.DB.SAPR;

namespace AR_Zhuk_Schema.DB
{
    public class DBService : IDBService
    {
        private static Dictionary<string, List<FlatInfo>> dictSections = new Dictionary<string, List<FlatInfo>>();

        public List<FlatInfo> GetSections (Section section, string type, string levels, SpotInfo sp, int maxSectionBySize)
        {
            List<FlatInfo> sectionsBySyze;
            string key = section.CountStep + type + levels;

            if (!dictSections.TryGetValue(key, out sectionsBySyze))
            {
                FlatsInSectionsTableAdapter flatsIsSection = new FlatsInSectionsTableAdapter();
                List<FlatsInSectionsRow> flatsDb;
                if (maxSectionBySize == 0)
                {
                    flatsDb = flatsIsSection.GetFlatsInTypeSection(section.CountStep, type, levels).ToList();
                }
                else
                {
                    flatsDb = flatsIsSection.GetFlatsInTypeSectionMax(maxSectionBySize, section.CountStep, type, levels).ToList();
                }

                sectionsBySyze = new List<FlatInfo>();                
                flatsDb = flatsDb.OrderBy(x => x.ID_FlatInSection).ToList();                
                FlatInfo fl = new FlatInfo();
                bool isValidSection = true;
                var groupFlats = flatsDb.GroupBy(x => x.ID_Section).Select(x => x.ToList()).ToList();
                foreach (var gg in groupFlats)
                {
                    fl = new FlatInfo();

                    fl.Floors = section.Floors;
                    fl.CountStep = section.CountStep;
                    fl.Flats = new List<RoomInfo>();
                    fl.IsCorner = section.IsCorner;
                    isValidSection = true;
                    bool isContains = false;
                    for (int i = 0; i < gg.Count; i++)
                    {
                        var f = gg[i];
                        fl.IdSection = f.ID_Section;
                        isContains = false;
                        if (!f.SubZone.Equals("0"))
                        {
                            isValidSection = false;
                            foreach (var r in sp.requirments.Where(x => x.CodeZone.Equals(f.SubZone)).ToList())
                            {
                                if (!(r.MinArea - 4 <= f.AreaTotalStandart & r.MaxArea + 4 >= f.AreaTotalStandart))
                                    continue;
                                isContains = true;
                                break;
                            }

                            if (!isContains)
                            {
                                isValidSection = false;
                                break;
                            }
                        }

                        var fflat = new RoomInfo(f.ShortType, f.SubZone, f.TypeFlat, "",
                            "", f.LinkageBefore, f.LinkageAfter, "", "", "", f.Levels, "", "", f.LightBottom, f.LightTop,
                            "");
                        fflat.AreaModules = f.AreaInModule;
                        fflat.AreaTotal = f.AreaTotalStrong;
                        fflat.AreaTotalStandart = f.AreaTotalStandart;
                        fflat.SelectedIndexTop = f.SelectedIndexTop;
                        fflat.SelectedIndexBottom = f.SelectedIndexBottom;
                        fl.Flats.Add(fflat);
                        
                        if (!isValidSection)
                            continue;

                    }
                    if (!isContains)
                        continue;
                    sectionsBySyze.Add(fl);                    

                    if (maxSectionBySize != 0 && sectionsBySyze.Count == maxSectionBySize)
                    {
                        break;
                    }
                }
                dictSections.Add(key, sectionsBySyze);
            }
            return sectionsBySyze;
        }        
    }
}