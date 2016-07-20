using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk
{
    //public  class RoomInfo:IEquatable<RoomInfo>
    //{

    //    public string ShortType { get; set; }
    //    public string SubZone { get; set; }
    //    public string Type { get; set; }
    //    public double AreaLive { get; set; }
    //    public double AreaTotalStandart { get; set; }
    //    public double AreaTotal { get; set; }
    //    public double AreaModules { get; set; }
    //    public string IndexLenghtNIZ { get; set; }
    //    public string IndexLenghtTOP { get; set; }
    //    public string LinkageDO { get; set; }
    //    public string LinkagePOSLE { get; set; }
    //    public List<string> LinkageOR { get; set; }
    //    public string Requirment { get; set; }
    //    public string TypeSection { get; set; }
    //    public string OrderBuild { get; set; }
    //    public string LevelsSection { get; set; }
    //    public string TypeHouse { get; set; }
    //    public string LightingNiz { get; set; }
    //    public string LightingTop { get; set; }
    //    public int SelectedIndexTop { get; set; }
    //    public int SelectedIndexBottom { get; set; }
    //    public bool IsInclude { get; set; }
    //    public string FactorSmoke { get; set; }

    //     public int NextOffsetX { get; set; }

    //     public int CurrentOffsetX { get; set; }
    //     public string ImageNameSuffix { get; set; }
    //     public int HorisontalModules { get; set; }

        

        

        
        


    //     public RoomInfo()
    //    { }
    //    public RoomInfo(string shortType, string subZone, string type, string liveArea, string totalAreaStandart,string totalArea, string axisArea,
    //string indexLenghtNIZ,string indexLenghtTOP, string linkageBefore, string linkageAfter, string linkageOR, string requirment, string typeSection, string levelSection,string typeHouse,
    //        string order,string lightNiz,string lightTop,string factorSmoke)
    //    {
    //        this.ShortType = shortType;
    //        this.SubZone = subZone;
    //        this.Type = type;
    //        this.AreaLive = Double.Parse(liveArea, CultureInfo.CurrentCulture);
    //        this.AreaTotalStandart = Double.Parse(totalAreaStandart, CultureInfo.CurrentCulture);
    //        this.AreaTotal = Double.Parse(totalArea, CultureInfo.CurrentCulture);
    //        this.AreaModules = Double.Parse(axisArea, CultureInfo.CurrentCulture);
    //        this.IndexLenghtNIZ = indexLenghtNIZ;
           
    //        this.LinkageDO = linkageBefore ?? "";
    //        this.LinkagePOSLE = linkageAfter ?? "";
    //        if (!string.IsNullOrWhiteSpace(linkageOR))
    //            this.LinkageOR = linkageOR.Trim().Split('/').ToList();
    //        this.Requirment = requirment;
    //        this.TypeSection = typeSection;
    //        this.LevelsSection = levelSection;
    //        this.IsInclude = false;
    //        this.IndexLenghtTOP = indexLenghtTOP;
    //        this.OrderBuild = order;
    //        this.TypeHouse = typeHouse;
    //        this.LightingNiz = lightNiz;
    //        this.LightingTop = lightTop;
    //        this.FactorSmoke = factorSmoke;
    //    }

    //    public RoomInfo(RoomInfo rInfo,string linkageDo,string lankagePosle)
    //    {
    //        this.ShortType = rInfo.ShortType;
    //        this.SubZone = rInfo.SubZone;
    //        this.Type = rInfo.Type;
    //        this.AreaLive = rInfo.AreaLive;
    //        this.AreaTotalStandart = rInfo.AreaTotalStandart;
    //        this.AreaTotal = rInfo.AreaTotal;
    //        this.AreaModules = rInfo.AreaModules;
    //        this.IndexLenghtNIZ = rInfo.IndexLenghtNIZ;

    //        this.LinkageDO = linkageDo;
    //        this.LinkagePOSLE = rInfo.LinkagePOSLE;
    //        this.LinkageOR = rInfo.LinkageOR;
    //        this.Requirment = rInfo.Requirment;
    //        this.TypeSection = rInfo.TypeSection;
    //        this.LevelsSection = rInfo.LevelsSection;
    //        this.IsInclude = rInfo.IsInclude;
    //        this.IndexLenghtTOP = rInfo.IndexLenghtTOP;
    //        this.OrderBuild = rInfo.OrderBuild;
    //        this.TypeHouse = rInfo.TypeHouse;
    //        this.TypeHouse = rInfo.TypeHouse;
    //        this.LightingNiz = rInfo.LightingNiz;
    //        this.LightingTop = rInfo.LightingTop;
    //        this.FactorSmoke = rInfo.FactorSmoke;
    //    }

    //    public bool Equals(RoomInfo r)
    //    {
    //        if (Object.ReferenceEquals(r, null)) return false;
    //        if (Object.ReferenceEquals(this, r)) return true;
    //        return Type.Equals(r.Type);
    //    }

    //    public override bool Equals(Object r)
    //    {
    //        RoomInfo ri = r as RoomInfo;

    //        if (ri == null)
    //            return false;
    //        return this.Equals(ri);
    //    }


    //    public override int GetHashCode()
    //    {
    //        return Type.GetHashCode();
    //    }
    //}


}
