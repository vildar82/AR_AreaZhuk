using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;
using AR_Zhuk_InsSchema.Scheme.Cutting;

namespace AR_Zhuk_InsSchema.Scheme
{
    /// <summary>
    /// Объект - проектируемый объект застройки
    /// </summary>
    public class ProjectSpot
    {
        /// <summary>
        /// Схема пятен домов в объекте застройки
        /// </summary>
        public List<HouseSpot> HouseSpots { get; private set; }
        private List<HouseOptions> houseOptions;
        private SpotInfo sp;

        public ProjectSpot (List<HouseOptions> houseOptions, SpotInfo sp)
        {
            this.houseOptions = houseOptions;
            this.sp = sp;
        }

        /// <summary>
        /// Чтенее файла схемы инсоляции и определение пятен домов
        /// </summary>
        /// <param name="insolationFile">Excel файл схемы объекта застройки и инсоляции</param>
        public void ReadScheme (string schemeFile)
        {
            // Чтение матрицы ячеек первого листа в Excel файле
            ISchemeParser parserExcel = new ParserExcel();
            parserExcel.Parse(schemeFile);
            HouseSpots = parserExcel.HouseSpots;

            foreach (var houseSpot in HouseSpots)
            {
                var houseOpt = houseOptions.Find(o => o.HouseName == houseSpot.SpotName);
                houseSpot.HouseOptions = houseOpt;
            }
        }

        /// <summary>
        /// Получение всех вариантов домов для всех пятен домов
        /// </summary>        
        public List<List<HouseInfo>> GetTotalHouses ()
        {
            List<List<HouseInfo>> totalHouses = new List<List<HouseInfo>>();
            foreach (var item in HouseSpots)
            {
                ICutting cutting = CuttingFactory.Create(item, sp);
                var houses = cutting.Cut();
                totalHouses.Add(houses);
            }
            return totalHouses;
        }
    }
}