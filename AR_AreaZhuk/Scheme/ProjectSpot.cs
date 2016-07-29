using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Scheme
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

        public ProjectSpot ()
        {            
        }        

        /// <summary>
        /// Чтенее файла схемы инсоляции и определение пятен домов
        /// </summary>
        /// <param name="insolationFile">Excel файл схемы объекта застройки и инсоляции</param>
        public void ReadScheme (string insolationFile)
        {
            // Чтение матрицы ячеек первого листа в Excel файле
        }
    }
}
