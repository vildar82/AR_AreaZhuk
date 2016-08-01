using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_AreaZhuk.Scheme;

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
            var endSide = GetSectionEndSide();
            if (endSide == Side.Left || endSide == Side.Right)
            {
                var cellSide = cell.Offset(directionOrthoFromLLU);                
                InsSideTop = GetInsIndex(cellSide, isRequired: false);
                cellSide = cellSide.Offset(directionOrthoFromLLU);
                InsSideBot = GetInsIndex(cellSide, isRequired: false);
            }

            // Инсоляция верхних квартир (справа-налево), до 1 углового шага (сверху)
            var topFlats = insCheck.insSpot.insFramework.GetTopFlatsInSection(insCheck.sections.First().Flats, isTop:true);            
            foreach (var topFlat in topFlats)
            {
                if (topFlat.SubZone == "0")
                {
                    // ячейка над ллу           
                    // скачек на две ячейки по основному направлению и на 1 в негативном орто направлении
                    cell = cell.Offset(directionGeneralToLLU*2);                    
                    cell = cell.OffsetNegative(directionOrthoFromLLU);
                    InsTop[indexStep] = GetInsIndex(cell);
                    break;
                }
                for (int i = 0; i < topFlat.SelectedIndexTop; i++)
                {
                    InsTop[indexStep] = GetInsIndex(cell);
                    cell = cell.Offset(directionGeneralToLLU);
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
                cell = cell.Offset(directionOrthoFromLLU);
            }
            int indexBot = 4;            

            // Нижняя инсоляция - до начальной ячейки            
            for (int i = indexBot; i < countStep+indexBot-1; i++)
            {
                InsBot[i] = GetInsIndex(cell);
                cell = cell.OffsetNegative(directionGeneralToLLU);
            }
        }

        public override Side GetSectionEndSide ()
        {
            Side res = Side.None;
            if (insCheck.IsStartSection())
            {
                if (insCheck.isVertical)
                {
                    if (insCheck.startCellHelper.IsDirectionDown)
                        res = Side.Left;
                    else
                        res = Side.Right;
                }
                else
                {
                    if (insCheck.startCellHelper.IsDirectionDown)
                        res = Side.Right;
                    else
                        res = Side.Left;
                }
            }
            else if (insCheck.IsEndSection())
            {
                if (insCheck.isVertical)
                {
                    if (insCheck.startCellHelper.IsDirectionDown)
                        res = Side.Right;
                    else
                        res = Side.Left;
                }
                else
                {
                    if (insCheck.startCellHelper.IsDirectionDown)
                        res = Side.Right;
                    else
                        res = Side.Left;
                }
            }
            return res;
        }
    }
}
