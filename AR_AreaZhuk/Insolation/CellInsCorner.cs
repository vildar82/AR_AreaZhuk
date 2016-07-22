using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Значения инсоляции угловой секции
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
        public string[] InsSide { get; set; }     

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

        public void DefineIns()
        {            
            var isLeftNiz = insCheck.insSpot.IsLeftNizSection;
            var isLeftTop = insCheck.insSpot.IsLeftTopSection;
            var isRightNiz = insCheck.insSpot.IsRightNizSection;
            var isRightTop = insCheck.insSpot.IsRightTopSection;

            InsSide = new string[CountStepWithSection + CountStepShortEnd - 1];

            if (isLeftNiz)
            {
                startRow = insCheck.indexRowStart;
                startCol = insCheck.indexColumnStart;
                directionVertic = -1;
                directionHor = -1;                
            }

            define();
        }        

        /// <summary>
        /// Определение инсоляции для левой нижней угловой секции
        /// </summary>
        private void define ()
        {
            Cell cell = new Cell(startRow, startCol);                     
            int indexStep = 0;

            // Инсоляция верхних квартир (справа-налево), до 1 углового шага (сверху)
            var topFlats = insCheck.insFramework.GetTopFlatsInSection(insCheck.sectionInfo.Flats, true, false);            
            foreach (var topFlat in topFlats)
            {
                if (topFlat.SubZone == "0")
                {
                    // ячейка над ллу           
                    cell.Col += directionHor*2;             
                    cell.Row += directionVertic;
                    InsTop[++indexStep] = GetInsIndex(cell);
                    break;
                }             
                InsTop[indexStep] = GetInsIndex(cell);
                cell.Col += directionHor;
                indexStep++;
            }

            // Инсоляция боковой угловой части, сверху-вниз, начиная с 1 шага
            indexStep = 0;
            cell.Col += (CountStepWithSection - 1) * directionHor;
            for (int i = 0; i < InsSide.Length; i++)
            {
                InsSide[i] = GetInsIndex(cell);
                cell.Row -= directionVertic;
            }

            // Нижняя инсоляция - слева-направо, до начальной ячейки
            indexStep = 0;
            do
            {
                InsBot[indexStep] = GetInsIndex(cell);
                cell.Row -= directionHor;
            } while (startRow>cell.Row);            
        }
    }
}
