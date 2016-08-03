using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace BeetlyVisualisation
{
    public class FlatType
    {

        public string Type { get; set; }
        public string HorisontalModules { get; set; }
        public string CurrentOffsetX { get; set; }
        public string NextOffsetX { get; set; }
        public string FlatUntil { get; set; }
        public string FlatAfter { get; set; }

        public FlatType(string type, string horisontalModules, string currentOffsetX, string nextOffsetX, string flatUntil, string flatAfter)
        {
            this.Type = type;
            this.HorisontalModules = horisontalModules;
            this.CurrentOffsetX = currentOffsetX;
            this.NextOffsetX = nextOffsetX;
            this.FlatAfter = flatAfter;
            this.FlatUntil = flatUntil;
        }


        private int getHorisontalModules(string SelectedFlatUntil, string SelectedFlatAfter)
        {
            int HorModules = 0;
            string FlatToParce = string.Empty;
            string SelectedFlat = string.Empty;

            // Если в поле числа модулей по горизонтали присутствует разделитель ";" то выполняется выбор числа модулей в соответствии
            // с выбранным значением поля "ДО квартиры" или "ПОСЛЕ КВАРТИРЫ"
            // Например, в базе поле "ДО квартиры" или "ПОСЛЕ КВАРТИРЫ" имеет значение 4|A|C;2|A|H  ,  поле "Число модулей по горизонтали" имеет значение  3;2|_U
            // если выбрано значение "ДО квартиры" - 4|A|C, то ему соответствует Число модулей по горизонтали - 3
            // если выбрано значение "ДО квартиры" - 2|A|H, то ему соответствует Число модулей по горизонтали - 2, суффикс файла изображения - _U
            // Если разделитель ";" отсутствует, это означает, что есть только один вариант положения квартиры, число модулей по горизонтали выбирается из соответствующего поля

            if (HorisontalModules.Contains(";"))
            {

                // Задание значения, в соответствии с которым будет выполнено приведение
                if (FlatUntil.Contains(";"))
                {
                    FlatToParce = FlatUntil;
                    SelectedFlat = SelectedFlatUntil;
                }

                if (FlatAfter.Contains(";"))
                {
                    FlatToParce = FlatAfter;
                    SelectedFlat = SelectedFlatAfter;
                }

                if (FlatToParce != string.Empty)
                {
                    int i = 0;
                    string[] values = FlatToParce.Split(';');
                    i = Array.IndexOf(values, SelectedFlat);
                    string selectedHorisontalModules = values[i];

                    if (selectedHorisontalModules.Contains("|"))
                    {
                        int.TryParse(selectedHorisontalModules.Split('|')[0].Trim(), out HorModules);
                    }
                    else
                    {
                        int.TryParse(selectedHorisontalModules.Trim(), out HorModules);
                    }
                }
            }
            else
            {
                int.TryParse(HorisontalModules.Trim(), out HorModules);
            }

            return HorModules;
        }

        public void SetRoominFoParameters(RoomInfo ri)
        {
            // TODO: выполнить проверку на целочисленные значения
            ri.NextOffsetX = int.Parse(NextOffsetX.Trim());
            ri.CurrentOffsetX = int.Parse(CurrentOffsetX.Trim());


            int HorModules = 0;
            string FlatToParce = string.Empty;
            string SelectedFlat = string.Empty;


            // Если в поле числа модулей по горизонтали присутствует разделитель ";" то выполняется выбор числа модулей в соответствии
            // с выбранным значением поля "ДО квартиры" или "ПОСЛЕ КВАРТИРЫ"
            // Например, в базе поле "ДО квартиры" или "ПОСЛЕ КВАРТИРЫ" имеет значение 4|A|C;2|A|H  ,  поле "Число модулей по горизонтали" имеет значение  3;2|_U
            // если выбрано значение "ДО квартиры" - 4|A|C, то ему соответствует Число модулей по горизонтали - 3
            // если выбрано значение "ДО квартиры" - 2|A|H, то ему соответствует Число модулей по горизонтали - 2, суффикс файла изображения - _U
            // Если разделитель ";" отсутствует, это означает, что есть только один вариант положения квартиры, число модулей по горизонтали выбирается из соответствующего поля
            if (HorisontalModules.Contains(";"))
            {
                // Задание значения, в соответствии с которым будет выполнено приведение
                if (FlatUntil.Contains(";"))
                {
                    FlatToParce = FlatUntil;
                    SelectedFlat = ri.LinkageDO;
                }

                if (FlatAfter.Contains(";"))
                {
                    FlatToParce = FlatAfter;
                    SelectedFlat = ri.LinkagePOSLE;
                }

                if (FlatToParce != string.Empty)
                {
                    int i = 0;
                    string[] values = FlatToParce.Split(';');
                    i = Array.IndexOf(values, SelectedFlat);
                    string selectedHorisontalModules = HorisontalModules.Split(';')[i];                        

                    if (selectedHorisontalModules.Contains("|"))
                    {
                        int.TryParse(selectedHorisontalModules.Split('|')[0].Trim(), out HorModules);
                        string ImageFileSuffix = selectedHorisontalModules.Split('|')[1].Trim();
                        if(ImageFileSuffix != null)
                        {
                            ri.ImageNameSuffix = ImageFileSuffix;
                        }
                    }
                    else
                    {
                        int.TryParse(selectedHorisontalModules.Trim(), out HorModules);
                        
                        ri.ImageNameSuffix = string.Empty;
                    }

                    ri.HorisontalModules = HorModules;


                }
            }
            else
            {
                ri.HorisontalModules = int.Parse(HorisontalModules.Trim());
                ri.ImageNameSuffix = string.Empty;
            }         
        }
    }
}
