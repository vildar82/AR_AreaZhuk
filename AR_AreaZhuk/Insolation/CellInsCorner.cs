﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Значения инсоляции угловой секции
    /// InsTop - от левого угла секции до шага над ЛЛУ, шаги ЛЛУ пропускаются
    /// </summary>
    class CellInsCorner : CellInsBase
    {
        /// <summary>
        /// Длина загиба короткого конца угловой секции - в шагах (модулях)
        /// </summary>
        const int CountStepShortEnd = 1;

        /// <summary>
        /// Индексы инсоляции сбоку в углу - начиная с первого шага
        /// </summary>
        public string[] InsCornerSide { get; set; }
        /// <summary>
        /// Инсоляция с торца - верхняя ячейка
        /// Вдруг угловая сеция будет последней (или первой)
        /// </summary>
        public string InsSideTop { get; private set; } = "";
        public string InsSideBot { get; private set; } = "";

        /// <summary>
        /// Начальная точка - в стандартном положении (левый нижний) - верхний правый угол
        /// </summary>
        int startRow;
        int startCol;
        int directionVertic; // направление по вертикали (стандартное - вверх: row -1)
        int directionHor; // смещение по главному направлению, от стартовой точки в сторону ллу (стандартное - в лево: col -1)

        public CellInsCorner (InsCheckCorner insCheck) : base(insCheck)
        {            
        }

        public override void DefineIns()
        {            
            var isLeftNiz = insCheck.insSpot.IsLeftNizSection;
            var isLeftTop = insCheck.insSpot.IsLeftTopSection;
            var isRightNiz = insCheck.insSpot.IsRightNizSection;
            var isRightTop = insCheck.insSpot.IsRightTopSection;

            InsCornerSide = new string[CountStepWithSection + CountStepShortEnd - 1];

            if (isLeftNiz)
            {
                startRow = insCheck.indexRowStart;
                startCol = insCheck.indexColumnStart-1;
                directionVertic = -1;
                directionHor = -1;                
            }

            define();
        }        

        /// <summary>
        /// Определение инсоляции угловой горизонтально расположенной в Excel секции
        /// </summary>
        private void define ()
        {
            Cell cell = new Cell(startRow, startCol);                     
            int indexStep = 0;

            // Если угловая секция первая или последняя в доме, то запись инсоляции в торце
            if (isEndSection())
            {
                var cellSide = new Cell(cell.Row + 1, cell.Col);
                InsSideTop = GetInsIndex(cellSide, isRequired: false);
                cellSide.Row++;
                InsSideBot = GetInsIndex(cellSide, isRequired: false);
            }

            // Инсоляция верхних квартир (справа-налево), до 1 углового шага (сверху)
            var topFlats = insCheck.insSpot.insFramework.GetTopFlatsInSection(insCheck.sections.First().Flats, true, false);            
            foreach (var topFlat in topFlats)
            {
                if (topFlat.SubZone == "0")
                {
                    // ячейка над ллу           
                    cell.Col += directionHor*2;             
                    cell.Row += directionVertic;
                    InsTop[indexStep] = GetInsIndex(cell);
                    break;
                }
                for (int i = 0; i < topFlat.SelectedIndexTop; i++)
                {
                    InsTop[indexStep] = GetInsIndex(cell);
                    cell.Col += directionHor;
                    indexStep++;
                }                
            }

            // Инсоляция боковой угловой части, сверху-вниз, начиная с 1 шага
            indexStep = 0;
            cell.Col += (CountStepWithSection - 1) * directionHor;
            for (int i = 0; i < InsCornerSide.Length; i++)
            {
                InsCornerSide[i] = GetInsIndex(cell);
                cell.Row -= directionVertic;
            }

            // Нижняя инсоляция - слева-направо, до начальной ячейки            
            for (int i = 0; i < countStep; i++)
            {
                InsBot[i] = GetInsIndex(cell);
                cell.Col -= directionHor;
            }
        }
    }
}