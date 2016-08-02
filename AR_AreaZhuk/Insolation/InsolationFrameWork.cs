using AR_Zhuk_DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk
{
    class InsolationFrameWork
    {
        public List<RoomInfo> GetTopFlatsInSection(List<RoomInfo> section, bool isTop, bool isRight)
        {
            List<RoomInfo> topFlats = new List<RoomInfo>();
            if (isTop)
            {
                if (!isRight)
                {
                    for (int i = section.Count - 3; i < section.Count; i++)
                    {
                        if (section[i].SelectedIndexTop == 0) continue;
                        topFlats.Add(section[i]);
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        if (section[i].SelectedIndexTop == 0) continue;
                        topFlats.Add(section[i]);
                    }

                }
                else
                {
                    for (int i = 3; i >= 0; i--)
                    {
                        if (section[i].SelectedIndexTop == 0) continue;
                        topFlats.Add(section[i]);
                    }

                    for (int i = section.Count - 1; i > section.Count - 4; i--)
                    {
                        if (section[i].SelectedIndexTop == 0) continue;
                        topFlats.Add(section[i]);
                    }
                }
            }
            else
            {
                if (!isRight)
                {
                    for (int i = 0; i < section.Count; i++)
                    {
                        if (section[i].SelectedIndexTop != 0) continue;
                        topFlats.Add(section[i]);
                    }
                }
                else
                {


                    for (int i = section.Count - 1; i >= 0; i--)
                    {
                        if (section[i].SelectedIndexTop != 0) continue;
                        topFlats.Add(section[i]);
                    }
                }
            }
            return topFlats;
        }

        public int[] GetLightingPosition(string lightStr, RoomInfo room, List<RoomInfo> allRooms)
        {
            int[] light = new int[5];
            string[] masStr = lightStr.Split(';');
            var l = lightStr.Length;
            if (masStr.Length > 1)
            {

                string[] ss = masStr[1].Split('*');
                if (allRooms.IndexOf(room) - 1 < 0)
                    return null;
                var preRoom = allRooms[allRooms.IndexOf(room) - 1];
                if (preRoom.LinkagePOSLE.Contains(ss[0].Trim().Substring(0, 1)) &
                    (room.LinkageDO.Contains(ss[0].Trim().Substring(1, 1))))
                {
                    masStr[0] = ss[1];
                }
            }
            if (masStr[0].Contains('|'))
            {
                if (masStr[0].Contains('-'))
                {
                    string[] ss = masStr[0].Split('-');
                    if (ss[0].Contains('|'))
                    {

                        light[0] = -Convert.ToInt16(ss[0].Split('|')[0]);
                        light[1] = -Convert.ToInt16(ss[0].Split('|')[1]);
                        light[2] = Convert.ToInt16(ss[1]);
                    }
                    else
                    {
                        light[1] = -Convert.ToInt16(ss[1].Split('|')[0]);
                        light[2] = -Convert.ToInt16(ss[1].Split('|')[1]);
                        light[0] = Convert.ToInt16(ss[0]);
                    }
                }
                else
                {
                    light[0] = -Convert.ToInt16(masStr[0].Split('|')[0]);
                    light[1] = -Convert.ToInt16(masStr[0].Split('|')[1]);
                }
            }
            else if (masStr[0].Contains('-'))
            {
                int counter = 0;
                string[] ms = masStr[0].Split('|');
                if (ms[0].Contains('-'))
                {
                    for (int i = Convert.ToInt16(ms[0].Split('-')[0]); i <= Convert.ToInt16(ms[0].Split('-')[1]); i++)
                    {
                        light[counter] = i;
                        counter++;
                    }
                }
                else
                {
                    for (int i = Convert.ToInt16(ms[1].Split('-')[0]); i <= Convert.ToInt16(ms[1].Split('-')[1]); i++)
                    {
                        light[counter] = i;
                        counter++;
                    }
                }
            }
            else if (masStr[0].Contains(','))
            {
                string[] mass = masStr[0].Split(',');
                int counter = 0;
                for (int i = 0; i < mass.Length; i++)
                {
                    light[counter] = Convert.ToInt16(mass[i]);
                    counter++;
                }
            }
            else light[0] = Convert.ToInt16(masStr[0]);
            return light;
        }


    }




}
