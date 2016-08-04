using System;
using System.Collections.Concurrent;
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
        private Dictionary<string, List<FlatInfo>> dictSections = new Dictionary<string, List<FlatInfo>>();
        private ConcurrentDictionary<SelectSectionParam, List<FlatsInSectionsRow>> dictDbFlats = 
                    new ConcurrentDictionary<SelectSectionParam, List<FlatsInSectionsRow>>();
        

        SpotInfo sp;
        int maxSectionBySize;        

        public DBService(SpotInfo sp, int maxSectionBySize)
        {
            this.sp = sp;
            this.maxSectionBySize = maxSectionBySize;
        }

        public List<FlatInfo> GetSections (Section section, SelectSectionParam selecSectParam)
        {
            List<FlatInfo> sectionsBySyze;
            string key = section.CountStep + selecSectParam.Type + selecSectParam.Levels;

            if (!dictSections.TryGetValue(key, out sectionsBySyze))
            {
                List<FlatsInSectionsRow> flatsDb;                
                if (!dictDbFlats.TryGetValue(selecSectParam, out flatsDb))
                {
                    flatsDb = LoadFromDbSection(selecSectParam);
                    dictDbFlats.TryAdd(selecSectParam, flatsDb);
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

        public void PrepareLoadSections (List<SelectSectionParam> selectSects)
        {
            // отбор типов секций которые не загружались
            var notInDictSS = selectSects.Where(s => !dictDbFlats.ContainsKey(s)).ToList();
            if (notInDictSS.Count > 0)
            {
                // Паралельная загрузка секций                
                Parallel.ForEach(notInDictSS, (s) => dictDbFlats.TryAdd(s, LoadFromDbSection(s)));                                
            }
        }

        private List<FlatsInSectionsRow> LoadFromDbSection (SelectSectionParam selectSectParam)
        {
            List<FlatsInSectionsRow> flatsDb;
            FlatsInSectionsTableAdapter flatsIsSection = new FlatsInSectionsTableAdapter();
            if (maxSectionBySize == 0)
            {
                flatsDb = flatsIsSection.GetFlatsInTypeSection(selectSectParam.Step, 
                            selectSectParam.Type, selectSectParam.Levels).ToList();
            }
            else
            {
                flatsDb = flatsIsSection.GetFlatsInTypeSectionMax(maxSectionBySize, 
                            selectSectParam.Step, selectSectParam.Type, selectSectParam.Levels).ToList();
            }
            return flatsDb;            
        }
    }

    public class SelectSectionParam : IEquatable<SelectSectionParam>
    {
        public readonly int Step;
        public readonly string Type;
        public readonly string Levels;

        public SelectSectionParam(int step, string type, string levels)
        {
            Step = step;
            Type = type;
            Levels = levels;
        }

        public bool Equals (SelectSectionParam other)
        {
            return Step == other.Step && Type == other.Type && Levels == other.Levels;
        }

        public override int GetHashCode ()
        {
            return Step.GetHashCode() ^ Type.GetHashCode() ^ Levels.GetHashCode();
        }
    }
}