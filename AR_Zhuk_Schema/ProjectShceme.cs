using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;
using AR_Zhuk_Schema.Scheme;
using AR_Zhuk_Schema.Scheme.Cutting;

namespace AR_Zhuk_Schema
{
    /// <summary>
    /// Объект - проектируемый объект застройки
    /// </summary>
    public class ProjectScheme
    {
        private List<HouseOptions> houseOptions;
        private SpotInfo sp;

        /// <summary>
        /// Схема пятен домов в объекте застройки
        /// </summary>
        public List<HouseSpot> HouseSpots { get; private set; }        

        public ProjectScheme (List<HouseOptions> houseOptions, SpotInfo sp)
        {
            this.houseOptions = houseOptions;
            this.sp = sp;
        }

        /// <summary>
        /// Чтенее файла схемы инсоляции и определение пятен домов
        /// </summary>
        /// <param name="insolationFile">Excel файл схемы объекта застройки и инсоляции</param>
        /// <exception cref="Exception">Недопустимое имя пятна дома.</exception>
        public void ReadScheme (string schemeFile)
        {
            // Чтение матрицы ячеек первого листа в Excel файле
            ISchemeParser parserExcel = new ParserExcel();
            parserExcel.Parse(schemeFile);
            HouseSpots = parserExcel.HouseSpots;

            foreach (var houseSpot in HouseSpots)
            {
                var houseOpt = houseOptions.Find(o => o.HouseName == houseSpot.SpotName);
                if (houseOpt == null)
                {
                    string allowedHouseNames = string.Join(",", houseOptions.Select(h => h.HouseName));
                    throw new Exception("Имя пятна дома определенное в файле инсоляции - '" + houseSpot.SpotName + "' не соответствует одному из допустимых значений: " + allowedHouseNames);
                }
                houseSpot.HouseOptions = houseOpt;
            }
        }

        /// <summary>
        /// Получение всех вариантов домов для всех пятен домов
        /// <param name="maxSectionBySize">Максимальное кол-во вариантов секций одного размера загружаемых из базы. 0 - все.</param>
        /// </summary>        
        public List<List<HouseInfo>> GetTotalHouses (int maxSectionBySize = 0)
        {
            List<List<HouseInfo>> totalHouses = new List<List<HouseInfo>>();
            foreach (var item in HouseSpots)
            {
                ICutting cutting = CuttingFactory.Create(item, sp, maxSectionBySize);
                var houses = cutting.Cut();
                if (houses.Count != 0)
                {
                    totalHouses.Add(houses);
                }
            }
            return totalHouses;
        }
    }
}