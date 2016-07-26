using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Значения инсоляции угловой секции
    /// InsTop - от левого угла секции до шага над ЛЛУ, шаги ЛЛУ пропускаются
    /// InsBot - включает боковые ячейки не попадающие в шаги секции
    /// </summary>
    class CellInsCorner : CellInsBase
    {           
        /// <summary>
        /// Инсоляция с торца - верхняя ячейка
        /// Вдруг угловая сеция будет последней (или первой)
        /// </summary>
        public string InsSideTop { get; private set; } = "";
        public string InsSideBot { get; private set; } = "";
        
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

            InsBot = new string[countStep+3];                                         

            if (isLeftNiz)
            {                
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
            Cell cell = insCheck.startCellHelper.StartCell;
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
            cell.Col += (InsolationSpot.CountStepWithSection - 1) * directionHor;
            
            for (int i = 0; i < 4; i++) // 4 - кол боковых ячеек (начиная с 1 шага)
            {                
                InsBot[i] = GetInsIndex(cell);
                cell.Row -= directionVertic;
            }
            int indexBot = 4;
            cell.Row -= directionVertic;

            // Нижняя инсоляция - до начальной ячейки            
            for (int i = indexBot; i < countStep+indexBot; i++)
            {
                InsBot[i] = GetInsIndex(cell);
                cell.Col -= directionHor;
            }
        }
    }
}
