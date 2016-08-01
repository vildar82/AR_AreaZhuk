using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.DB
{
    /// <summary>
    /// Сервис доступа к базе данных
    /// </summary>
    public interface IDBService
    {
        List<FlatInfo> GetSections (int CountStep, string Type, string Levels);
    }
}