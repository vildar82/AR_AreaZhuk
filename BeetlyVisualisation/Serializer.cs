using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using AR_Zhuk_DataModel;

namespace BeetlyVisualisation
{
    public class Serializer
    {
        public void SerializeList(List<GeneralObject> genObject)
        {


            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "xml files |*.xml";
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (
               FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        XmlSerializer ser = new XmlSerializer(genObject.GetType());
                        ser.Serialize(fs, genObject);
                    }
                    catch
                    {
                    }

                }
            }
           
        }

        public GeneralObject DeserializeXmlFile(string filePath)
        {

            GeneralObject genObject = new GeneralObject();


            Type typ = genObject.GetType();

            try
            {
                XmlSerializer ser = new XmlSerializer(typ);
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    try
                    {
                        genObject = (GeneralObject)ser.Deserialize(reader);
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                        System.Windows.Forms.MessageBox.Show(message);
                    }
                }
            }
            catch (Exception ex)
            {

                 MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            return genObject;
        }
    }
}
