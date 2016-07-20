using AR_Zhuk_DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace AR_AreaZhuk
{
    public class Serializer
    {
        public void SerializeList(GeneralObject go,string mark)
        {


            //SaveFileDialog sfd = new SaveFileDialog();

            //sfd.Filter = "xml files |*.xml";
            //sfd.FilterIndex = 2;
            //sfd.RestoreDirectory = true;

            //if (sfd.ShowDialog() == DialogResult.OK)
            //{
            if (File.Exists(@"E:\Экспорты\" + mark + ".xml"))
                mark += "1";
            Thread.Sleep(100);
            bool isContinue = true;
            while (isContinue)
            {
                try
                {
                    using (FileStream fs = new FileStream(@"E:\Экспорты\" + mark + ".xml", FileMode.Create, FileAccess.Write))
                    {
                        try
                        {
                            XmlSerializer ser = new XmlSerializer(go.GetType());
                            ser.Serialize(fs, go);
                            isContinue = false;
                        }
                        catch
                        {
                        }

                    }
                }
                catch { }
            }
           
               
            //}
           
        }

        public List<HouseInfo> DeserializeXmlFile(string filePath)
        {

            List<HouseInfo> houseInfo = new List<HouseInfo>();

            try
            {
                XmlSerializer ser = new XmlSerializer(houseInfo.GetType());
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    try
                    {
                        houseInfo = (List<HouseInfo>)ser.Deserialize(reader);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {

                //  MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            return houseInfo;
        }
    }
}
