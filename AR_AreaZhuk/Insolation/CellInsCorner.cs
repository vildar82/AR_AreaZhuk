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
        
        Cell directionGeneralToLLU; // смещение по главному направлению, от стартовой точки в сторону ллу (стандартное - в лево: col -1)        
        Cell directionOrthoFromLLU;  // направление по вертикали от ЛЛУ вниз (стандартное - вверх: row -1)        

        public CellInsCorner (InsCheckCorner insCheck) : base(insCheck)
        {            
        }

        public override void DefineIns()
        {   
            InsBot = new string[countStep+3];                                         

            if (isVertic)
            {                
                // Вертикальная
                if (insCheck.startCellHelper.IsDirectionDown)
                {
                    // вниз
                    directionGeneralToLLU.Row = 1;                    
                    directionOrthoFromLLU.Col = -1;
                }
                else
                {
                    // Вверх
                    directionGeneralToLLU.Row = -1;
                    directionOrthoFromLLU.Col = -1;
                }
            }
            else
            {
                // Горизонтальная
                if (insCheck.startCellHelper.IsDirectionDown)
                {
                    // загиб сверху-вбок
                    directionGeneralToLLU.Col = -1;
                    directionOrthoFromLLU.Row = 1;
                }
                else
                {
                    // загиб снизу-вбок
                    directionGeneralToLLU.Col = -1;
                    directionOrthoFromLLU.Row = -1;
                }
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
            if (insCheck.IsEndSection())
            {
                var cellSide = cell;
                cellSide.Offset(directionOrthoFromLLU);
                InsSideTop = GetInsIndex(cellSide, isRequired: false);
                cellSide.Offset(directionOrthoFromLLU);
                InsSideBot = GetInsIndex(cellSide, isRequired: false);
            }

            // Инсоляция верхних квартир (справа-налево), до 1 углового шага (сверху)
            var topFlats = insCheck.insSpot.insFramework.GetTopFlatsInSection(insCheck.sections.First().Flats, true, false);            
            foreach (var topFlat in topFlats)
            {
                if (topFlat.SubZone == "0")
                {
                    // ячейка над ллу           
                    // скачек на две ячейки по основному направлению и на 1 в негативном орто направлении
                    cell.Offset(directionGeneralToLLU);
                    cell.Offset(directionGeneralToLLU);
                    cell.OffsetNegative(directionOrthoFromLLU);
                    InsTop[indexStep] = GetInsIndex(cell);
                    break;
                }
                for (int i = 0; i < topFlat.SelectedIndexTop; i++)
                {
                    InsTop[indexStep] = GetInsIndex(cell);
                    cell.Offset(directionGeneralToLLU);
                    indexStep++;
                }                
            }

            // Инсоляция боковой угловой части, сверху-вниз, начиная с 1 шага
            indexStep = 0;
            // Скачек на ширину секции
            int width = InsolationSpot.CountStepWithSection - 1;
            cell.Row += width * directionGeneralToLLU.Row;
            cell.Col += width * directionGeneralToLLU.Col;            
            
            for (int i = 0; i < 4; i++) // 4 - кол боковых ячеек (начиная с 1 шага)
            {                
                InsBot[i] = GetInsIndex(cell);
                cell.OffsetNegative(directionOrthoFromLLU);
            }
            int indexBot = 4;

            cell.OffsetNegative(directionOrthoFromLLU);

            // Нижняя инсоляция - до начальной ячейки            
            for (int i = indexBot; i < countStep+indexBot; i++)
            {
                InsBot[i] = GetInsIndex(cell);
                cell.OffsetNegative(directionGeneralToLLU);
            }
        }
    }
}
