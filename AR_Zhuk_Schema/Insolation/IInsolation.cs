using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_Schema.Insolation
{
    /// <summary>
    /// Проверка инсоляции
    /// </summary>
    public interface IInsolation
    {
        /// <summary>
        /// Проверка инсоляции секции
        /// </summary>
        /// <param name="section">Проверяемая секция</param>
        /// <returns>Секции прошедшие инсоляцию</returns>
        List<FlatInfo> GetInsolationSections (Section section);
        RoomInsolation FindRule (RoomInfo flat);
        List<RoomInfo> GetSideFlatsInSection (List<RoomInfo> sectionFlats, bool isTop);
    }
}
