using System;
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
    /// Для рядовой - это правый верхний угол от ллу.
    /// Для угловой - это правый верхни  угол от ллу.
    /// </summary>
    class StartCellHelper
    {   
        Section previousSection;
        Cell defaultCellSection;

        InsolationSpot insSpot;        
        SpotInfo spotInfo;        

        /// <summary>
        /// Определенная стартовая точка для текущей секции
        /// </summary>
        public Cell StartCell { get; private set; }
        public bool IsDirectionDown { get; private set; }       

        public StartCellHelper (InsolationSpot insSpot, Section s, SpotInfo spotInfo,Cell cellFirstSection)
        {
            this.insSpot = insSpot;            
            this.spotInfo = spotInfo;            
            previousSection = s;

            // Определение стартовой точки для первой секции
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
        /// Определение стартовой ячейки первой секции (угловой)
        /// Если угловая секция первая в доме, то стартовать она может только с хвоста.
        /// </summary>        
        private Cell defineStartCellFirstSectionCorner (Section s, Cell cellFirstSection)
        {
            Cell startCell = cellFirstSection;
            defaultCellSection = cellFirstSection;

            // длина хвоста
            int tailLength = GetTailLength(s.CountStep);
            IsDirectionDown = isDirectionFirstSectionDown();
            if (s.IsVertical)
            {
                // Вертикальная
                // Направление                
                if (IsDirectionDown)
                {
                    // Вниз                    
                    startCell.Col += InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Col = startCell.Col + 2;
                    defaultCellSection.Row += s.CountStep - 2; //-1 шаг загиба
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
                // Направление загиба - вверх или вниз               
                if (IsDirectionDown)
                {
                    // Вниз
                    // стартовая точка совпадаетс дефолтной
                    defaultCellSection.Col += tailLength;
                    defaultCellSection.Row += 2;
                }
                else
                {
                    // Вверх
                    startCell.Row -= InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Col += tailLength;
                    defaultCellSection.Row = startCell.Row - 2;
                }
            }
            return startCell;
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
                IsDirectionDown = isDirectionFirstSectionDown();
                if (IsDirectionDown)
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
                startCell.Row -= InsolationSpot.CountStepWithSection - 1;
                startCell.Col += s.CountStep - 1;
                defaultCellSection.Col += s.CountStep;
            }
            return startCell;
        }   

        /// <summary>
        /// Определение стартовой точки угловой секции
        /// </summary>        
        private Cell defineStartCellCorner (Section s)
        {
            var startCell = defaultCellSection;
            // длина хвоста угловой секции
            int tailLenght = GetTailLength(s.CountStep);            
            IsDirectionDown = isDirectionDown();
            // Направление
            if (s.IsVertical)
            {
                // Вертикальная
                // Направление
                if (IsDirectionDown)
                {
                    // Вниз
                    startCell.Col += InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Col = startCell.Col + 2;
                    defaultCellSection.Row += s.CountStep - 2;
                }
                else
                {
                    // Вверх
                    startCell.Col += InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Col = startCell.Col + 2;
                    defaultCellSection.Row -= tailLenght;
                }
            }
            else
            {
                // Горизонтальная
                if (IsDirectionDown)
                {
                    // поворот сверху - вправо    
                    startCell.Col += s.CountStep - 2;
                    startCell.Row++;
                    defaultCellSection.Col = startCell.Col + 1;
                    defaultCellSection.Row = startCell.Row + InsolationSpot.CountStepWithSection - 1;                
                }
                else
                {
                    // поворот снизу - вправо
                    startCell.Row--;
                    startCell.Col += s.CountStep - 2;
                    defaultCellSection.Col = startCell.Col + 1;
                    defaultCellSection.Row = startCell.Row;
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
                IsDirectionDown = isDirectionFirstSectionDown();
                if (IsDirectionDown)
                {
                    // Направление Вниз                    
                    defaultCellSection.Row += s.CountStep;
                    startCell.Col += InsolationSpot.CountStepWithSection - 1;
                    startCell.Row--;
                }
                else
                {
                    // Направление Вверх                    
                    startCell.Col += InsolationSpot.CountStepWithSection - 1;
                    defaultCellSection.Row -= s.CountStep;
                }
            }
            else
            {
                // Горизонтальная                                
                defaultCellSection.Col += s.CountStep;
                startCell.Col = defaultCellSection.Col - 1;
                startCell.Row -= InsolationSpot.CountStepWithSection - 1;
                
            }
            return startCell;
        }

        /// <summary>
        /// Направление по вертикали для первой секции
        /// </summary>
        /// <returns>True - вниз, false - вверх</returns>
        private bool isDirectionFirstSectionDown ()
        {
            var res = defaultCellSection.Row == insSpot.MinLeftXY[1];
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