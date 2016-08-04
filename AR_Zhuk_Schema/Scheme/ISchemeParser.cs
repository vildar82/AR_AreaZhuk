using System.Collections.Generic;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_Schema.Scheme
{
    public interface ISchemeParser
    {
        /// <summary>
        /// Пятна домов
        /// </summary>
        List<HouseSpot> HouseSpots { get; }
        /// <summary>
        /// Парсинг файла схемы
        /// </summary>
        /// <param name="schemeFile">Полный путь к файлу схемы</param>
        void Parse (string schemeFile);
        /// <summary>
        /// Определение длины дома в шагах
        /// </summary>        
        List<Module> GetSteps (Cell cellStart, Cell direction, out Cell lastCell);
        /// <summary>
        /// Проверка ячейки в схеме - это ячейка дома - P|A, соответствует записи
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        bool IsInsCell (Cell cell);
    }
}