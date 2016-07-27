using AR_Zhuk_DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AR_AreaZhuk
{
  public static  class FormManager
    {


      public static void Panel_Show(Panel panel, Button btn, int minSize, int maxSize)
      {
          if (panel.Height == minSize)
          {
              for (int i = minSize; i <= maxSize; i++)
              {
                  System.Windows.Forms.Application.DoEvents();
                  panel.Height = i;
                  Thread.Sleep(1);
              }
              btn.Image = Properties.Resources.up;
          }
          else
          {
              for (int i = maxSize; i >= minSize; i--)
              {
                  System.Windows.Forms.Application.DoEvents();
                  panel.Height = i;
                  Thread.Sleep(1);
              }
              btn.Image = Properties.Resources.down;
          }
      }

      public static SpotInfo GetSpotTaskFromDG(SpotInfo spotInfo, DataGridView dg)
      {
          for (int i = 0; i < dg.RowCount - 1; i++)
          {
              string[] parse = dg[1, i].Value.ToString().Split('-');
              spotInfo.requirments.Where(x => x.SubZone.Equals(dg[0, i].Value.ToString()))
                  .Where(x => x.MinArea.ToString().Equals(parse[0]))
                  .ToList()[0].Percentage =
                  Convert.ToInt16(dg[2, i].Value);
              spotInfo.requirments.Where(x => x.SubZone.Equals(dg[0, i].Value.ToString()))
                  .Where(x => x.MinArea.ToString().Equals(parse[0]))
                  .ToList()[0].OffSet =
                  Convert.ToInt16(dg[3, i].Value);
          }
          return spotInfo;
      }
      public static void ViewDataProcentage(DataGridView dg2, List<SpotInfo> spinfos)
      {
          DataSet dataSet = new DataSet();
          BindingSource bs = new BindingSource();
          bs.DataSource = dataSet;
          DataTable dt = new DataTable();
          dt.Columns.Add("Площадь (м2.)", typeof(Double));
          dt.Columns.Add("Общее кол-во секций (шт.)", typeof(Int16));
          dt.Columns.Add("Кол-во одинаковых секций (шт.)", typeof(string));
          dt.Columns.Add("Кол-во квартир (шт.)", typeof(Int16));
          dt.Columns.Add("Студии 22-23м2 (%)", typeof(Double));
          dt.Columns.Add("Студии 33-35м2 (%)", typeof(Double));
          dt.Columns.Add("Однокомн. 35-47м2 (%)", typeof(Double));
          dt.Columns.Add("Двухкомн. 45-47м2 (%)", typeof(Double));
          dt.Columns.Add("Двухкомн. 53-56м2 (%)", typeof(Double));
          dt.Columns.Add("Двухкомн. 68-70м2 (%)", typeof(Double));
          dt.Columns.Add("Трехкомн. 85-95м2 (%)", typeof(Double));
          dt.Columns.Add("GUID", typeof(String));
          foreach (var ss in spinfos)
          {
              if (ss == null)
                  continue;
              List<double> percent = new List<double>();
              foreach (var s in ss.requirments)
              {
                  percent.Add(Math.Round(s.RealPercentage, 1));
              }
              object[] newrow = new object[]
                {
                    Math.Round(ss.RealArea,1),ss.TotalSections,ss.TypicalSections,ss.TotalFlats, percent[0], percent[1], percent[2], percent[3], percent[4], percent[5],
                    percent[6], ss.GUID
                };
              dt.Rows.Add(newrow);
          }

          dg2.DataSource = dt;
          dg2.Columns[dg2.Columns.Count - 1].Visible = false;
      }
    }
}
