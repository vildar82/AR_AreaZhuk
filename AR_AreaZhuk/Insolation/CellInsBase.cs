﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    abstract class CellInsBase
    {        
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

        public abstract void DefineIns ();

        public abstract Side GetSectionEndSide ();

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
        /// <param name="cell">Ячейка в матрице инсоляции</param>
        /// <param name="isRequired">Должна быть задана инсоляция в этой ячеке. Точно определять торцы я пока не умею, но надо</param>
        /// <returns>Индекс инсоляции - A,B,C,D</returns>
        protected string GetInsIndex (Cell cell, bool isRequired = true)
        {
            var cellValue = insCheck.insSpot.Matrix[cell.Col, cell.Row];
            string resInsIndex = string.Empty;
            var splitSpot = cellValue.Split('|');
            if (splitSpot.Length > 1)
            {
                resInsIndex = splitSpot[1];
                // проверка допустимого индекса инсоляции
                if (!RoomInsulation.AllowedIndexes.Contains(resInsIndex) && isRequired)
                {
                    throw new Exception("Недопустимый индекс инсоляции в задании - '" + resInsIndex + "', " +
                        "в ячейке [c" + cell.Col + ",r" + cell.Row + "].\n " +
                        "Допустимые индексы инсоляции " + string.Join(", ", RoomInsulation.AllowedIndexes));
                }
            }
            else if (isRequired)
            {
                throw new Exception("Не задан индекс инсоляции в ячейке [c"+ cell.Col + ",r"+ cell.Row + "].");
            }
            return resInsIndex;
        }        
    }
}
