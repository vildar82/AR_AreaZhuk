﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Insolation
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
        Section GetInsolationSections (Section section);
    }
}
