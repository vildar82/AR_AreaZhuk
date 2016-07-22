using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    abstract class CellInsBase
    {
        /// <summary>
        /// Ширина секции - в шагах (модулях)
        /// </summary>
        protected const int CountStepWithSection = 4;
        protected InsCheckBase insCheck;
        protected int countStep;
        protected bool isVertic;
        /// <summary>
        /// Индескы инсоляции сверху секции, справа-налево, начиная с 0, кол шагов = длине секции
        /// </summary>
        public string[] InsTop { get; set; }
        /// <summary>
        /// Индескы инсоляции снизу секции, слева-направо
        /// </summary>
        public string[] InsBot { get; set; }

        public CellInsBase (InsCheckBase insCheck)
        {
            this.insCheck = insCheck;
            countStep = insCheck.section.CountStep;
            isVertic = insCheck.isVertical;
            InsTop = new string[countStep];
            InsBot = new string[countStep];
        }        

        /// <summary>
        /// Получение индекса из матрицы инсоляции
        /// </summary>
        /// <param name="row">Индекс строки (от 0)</param>
        /// <param name="col">Индекс столбца (от 0)</param>
        /// <returns>Индекс инсоляции - A,B,C,D</returns>
        protected string GetInsIndex (Cell cell)
        {
            var cellValue = insCheck.insSpot.Matrix[cell.Col, cell.Row];
            string resInsIndex = string.Empty;
            var splitSpot = cellValue.Split('|');
            if (splitSpot.Length > 1)
            {
                resInsIndex = splitSpot[1];
                // проверка допустимого индекса инсоляции
                if (!RoomInsulation.AllowedIndexes.Contains(resInsIndex))
                {
                    throw new Exception("Недопустимый индекс инсоляции в задании - '" + resInsIndex + "', " +
                        "в ячейке [c" + cell.Col + ",r" + cell.Row + "].\n " +
                        "Допустимые индексы инсоляции " + string.Join(", ", RoomInsulation.AllowedIndexes));
                }
            }
            else
            {
                throw new Exception("Не задан индекс инсоляции в ячейке [c"+ cell.Col + ",r"+ cell.Row + "].");
            }
            return resInsIndex;
        }
    }
}
