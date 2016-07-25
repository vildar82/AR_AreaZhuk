﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Помощник определения стартовой ячейки переданной секции
    /// Стартовая ячейка - левый верхний угол секции в стандарртном положении (рядовой и угловой)
    /// </summary>
    class StartCellHelper
    {   
        Section previousSection;
        Cell defaultCellSection;

        InsolationSpot insSpot;        
        SpotInfo spotInfo;
        int[] indexSelectedSize;        

        /// <summary>
        /// Определенная стартовая точка для текущей секции
        /// </summary>
        public Cell StartCell { get; internal set; }

        public StartCellHelper (InsolationSpot insSpot, Section s, SpotInfo spotInfo,
            int[] indexSelectedSize, Cell cellFirstSection)
        {
            this.insSpot = insSpot;            
            this.spotInfo = spotInfo;
            this.indexSelectedSize = indexSelectedSize;
            previousSection = s;

            if (s.IsCorner)
            {
                StartCell = defineStartCellFirstSectionCorner(s, cellFirstSection);
            }
            else
            {
                StartCell = defineStartCellFirstSectionOrdinary(s, cellFirstSection);
            }
        }        

        /// <summary>
        /// Определение стартовой точки секции в матрице инсоляции
        /// верхняя правая точка. (в стандартном положении секции - горизонтально, ЛЛУ сверху)
        /// </summary>        
        public void Define (Section s)
        {
            // Определение стартовой точки для следующей секции в доме
            if (previousSection.NumberInSpot != s.NumberInSpot)
            {
                if (s.IsCorner)
                {
                    StartCell = defineStartCellCorner(s);
                }
                else
                {
                    StartCell = defineStartCellOridinary(s);
                }
                previousSection = s;
            }            
        }

        /// <summary>
        /// стартовая ячейка первой секции
        /// </summary>        
        private Cell defineStartCellFirstSectionOrdinary (Section s, Cell cellFirstSection)
        {
            Cell startCell = cellFirstSection;
            defaultCellSection = cellFirstSection;

            if (s.IsVertical)
            {
                // Вертикальная
                // Определение направления первой секции - вниз или вверх
                if (isDirectionFirstSectionDown())
                {
                    // Вниз
                    startCell.Col += InsolationSpot.CountStepWithSection - 1;
                    startCell.Row += s.CountStep - 1;
                    defaultCellSection.Row += s.CountStep;
                }
                else
                {
                    // Вверх
                    startCell.Col += InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Row -= s.CountStep;
                }
            }
            else
            {
                // Горизонтальная
                startCell.Row += InsolationSpot.CountStepWithSection - 1;
                startCell.Col += s.CountStep - 1;
                defaultCellSection.Col += s.CountStep;
            }
            return startCell;
        }

        /// <summary>
        /// Определение стартовой ячейки первой секции (угловой)
        /// Если угловая секция первая в доме, то стартовать она может только с хвоста.
        /// </summary>        
        private Cell defineStartCellFirstSectionCorner (Section s, Cell cellFirstSection)
        {            
            Cell startCell = cellFirstSection;
            defaultCellSection = cellFirstSection;

            int tailLength = GetTailLength(s.CountStep);

            if (s.IsVertical)
            {
                // Вертикальнаял
                // Направление
                if (isDirectionFirstSectionDown())
                {
                    // Вниз                    
                    startCell.Col += InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Col += 2;
                    defaultCellSection.Row += s.CountStep-2; //-1 шаг загиба
                }
                else
                {
                    // Вверх
                    startCell.Col += InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Col = startCell.Col + 2; // +1 шаг загиба
                    defaultCellSection.Row -= tailLength;
                }
            }
            else
            {
                // Горизонтальная
                // Направление
                if (isDirectionFirstSectionDown())
                {
                    // Вниз
                    defaultCellSection.Col += tailLength;
                    defaultCellSection.Row += 2;
                }
                else
                {
                    // Вверх
                    startCell.Row -= InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Col = startCell.Col + tailLength;
                    defaultCellSection.Row = startCell.Row - 2;
                }
            }
            return startCell;
        }

        /// <summary>
        /// Определение стартовой точки рядовой секции
        /// </summary>        
        private Cell defineStartCellOridinary (Section s)
        {
            Cell startCell = defaultCellSection;
                        
            if (s.IsVertical)
            {
                // Вертикальная
                // определение направления
                if (isDirectionDown())
                {
                    // Направление Вниз                    
                    defaultCellSection.Row += s.CountStep;
                    startCell.Col = defaultCellSection.Col + InsolationSpot.CountStepWithSection - 1;
                    startCell.Row = defaultCellSection.Row - 1;
                }
                else
                {
                    // Направление Вверх
                    startCell = defaultCellSection;
                    startCell.Col += InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Row -= s.CountStep;
                }
            }
            else
            {
                // Горизонтальная
                defaultCellSection.Col += s.CountStep;
                startCell = defaultCellSection;
                startCell.Col--;
                startCell.Row -= InsolationSpot.CountStepWithSection - 1;                
            }
            return startCell;
        }

        /// <summary>
        /// Определение стартовой точки угловой секции
        /// </summary>        
        private Cell defineStartCellCorner (Section s)
        {
            var startCell = defaultCellSection;
            int tailLenght = GetTailLength(s.CountStep);            
            // Направление
            if (s.IsVertical)
            {
                // Вертикальная
                // Направление
                if (isDirectionDown())
                {
                    // Вниз
                    startCell.Col += defaultCellSection.Col + InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Col = startCell.Col + 2;
                    defaultCellSection.Row += s.CountStep - 2;
                }
                else
                {
                    // Вверх
                    startCell.Col = defaultCellSection.Col + InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Col = startCell.Col + 2;
                    defaultCellSection.Row -= tailLenght;
                }
            }
            else
            {
                // Горизонтальная
                if (isDirectionDown())
                {
                    // поворот сверху - в право    
                    startCell.Col += s.CountStep - 2;
                    startCell.Row--;
                    defaultCellSection.Col = startCell.Col + 1;
                    defaultCellSection.Row = startCell.Row + InsolationSpot.CountStepWithSection - 1;                
                }
                else
                {
                    // поворот снизу - в право
                    startCell.Row--;
                    startCell.Col += s.CountStep - 2;
                    defaultCellSection.Col = startCell.Col + 1;
                    defaultCellSection.Row = startCell.Row;
                }
            }                     
            return startCell;
        }

        /// <summary>
        /// Направление по вертикали для первой секции
        /// </summary>
        /// <returns>True - вниз, false - вверх</returns>
        private bool isDirectionFirstSectionDown ()
        {
            var res = defaultCellSection.Row == insSpot.MaxLeftXY[1];
            return res;
        }

        /// <summary>
        /// Направление движения по вертикали.
        /// </summary>
        /// <returns>True - вниз, false - вверх</returns>
        private bool isDirectionDown ()
        {
            var res = StartCell.Row <= defaultCellSection.Row;
            return res;
        }               

        /// <summary>
        /// Длина хвоста угловой секции - в шагах        
        /// </summary>
        /// <param name="countStep">Кол шагов угловой секции</param>        
        private int GetTailLength (int countStep)
        {
            var res = countStep - 5;
            return res;
        }
    }
}
