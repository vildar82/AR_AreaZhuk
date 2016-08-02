using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_Zhuk_InsSchema.Scheme
{
    /// <summary>
    /// Объект - проектируемый объект застройки
    /// </summary>
    public class ProjectSpot
    {
        /// <summary>
        /// Пятна домов в объекте застройки
        /// </summary>
        public List<HouseSpot> HouseSpots { get; private set; }
        private List<HouseOptions> houseOptions;

        public ProjectSpot (List<HouseOptions> houseOptions)
        {
            this.houseOptions = houseOptions;
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
    }
}
