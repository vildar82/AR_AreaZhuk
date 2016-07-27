using AR_AreaZhuk.PIK1TableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Controller
{
   public static class Exporter
    {
       public static void ExportFlatsToSQL()
       {
           FrameWork fw = new FrameWork();
           var roomInfo = fw.GetRoomData("");
           PIK1TableAdapters.C_Flats_PIK1TableAdapter flatsPIK1 = new C_Flats_PIK1TableAdapter();
           foreach (var rr in roomInfo)
           {
               flatsPIK1.InsertFlat(rr.Type, rr.ShortType, rr.AreaLive, rr.AreaTotalStandart, rr.AreaTotal,
                   Convert.ToInt16(rr.AreaModules), 0, 0, rr.LinkageDO, rr.LinkagePOSLE,
                   rr.FactorSmoke, rr.LightingNiz, rr.LightingTop, rr.IndexLenghtTOP, rr.IndexLenghtNIZ, rr.SubZone,rr.LinkageOR);
           }
       }


       public static void ExportSectionsToSQL(int countModules,string typeSection,int countFloors,bool isCornerLeft,bool isCornerRight)
       {

           FrameWork fw = new FrameWork();
            var roomInfo = fw.GetRoomData("");
           var sections = fw.GenerateSections(roomInfo, countModules, isCornerLeft, isCornerRight, countFloors);
           string floors = "10-18";
           if(countFloors==25)
               floors = "19-25";
           else if (countFloors == 9)
               floors = "9";
           PIK1TableAdapters.C_SectionsTableAdapter sects = new C_SectionsTableAdapter();
           PIK1TableAdapters.F_nn_FlatsInSectionTableAdapter flatInSection = new F_nn_FlatsInSectionTableAdapter();
           PIK1TableAdapters.C_Flats_PIK1TableAdapter flatsISectionDB = new C_Flats_PIK1TableAdapter();
           foreach (var section in sections)
           {
               var idSection = sects.InsertSection(countModules / 4, typeSection, floors);
               foreach (var flat in section.Flats)
               {
                   try
                   {
                       var idFlat = flatsISectionDB.GetIdFlat(flat.Type, flat.LinkageDO.Trim(), flat.LinkagePOSLE.Trim());
                       flatInSection.InsertFlatInSection(Convert.ToInt32(idSection), Convert.ToInt32(idFlat), flat.SelectedIndexBottom,
                           flat.SelectedIndexTop);
                   }
                   catch
                   {

                   }

               }
           }
           Environment.Exit(48);

       }


       
    }
}
