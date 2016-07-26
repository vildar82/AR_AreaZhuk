using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Значение инсолиции ячеек в прямой секции (в стандартном положении - горизонтально, ЛЛУ сверху)
    /// Сверху - справа-налево
    /// Снизу - слева-направо
    /// Сбоку - 2 слева, 2 справа (сверху и снизу по ячейке)
    /// </summary>
    class CellInsOrdinary : CellInsBase
    {
        /// <summary>
        /// Инсоляция с торца секции - 2 ячейки справа и слеева
        /// </summary>
        public string InsSideTopLeft { get; private set; } = "";
        public string InsSideBotLeft { get; private set; } = "";
        public string InsSideTopRight { get; private set; } = "";
        public string InsSideBotRight { get; private set; } = "";

        public CellInsOrdinary (InsCheckOrdinary insCheck) : base(insCheck)
        {                       
        }

        public CellInsOrdinary Invert()
        {
            var invert = new CellInsOrdinary((InsCheckOrdinary)insCheck);
            invert.InsTop = InsBot.Reverse().ToArray();
            invert.InsBot = InsTop.Reverse().ToArray();

            invert.InsSideBotLeft = InsSideTopRight;
            invert.InsSideBotRight = InsSideTopLeft;
            invert.InsSideTopLeft = InsSideBotRight;
            invert.InsSideTopRight = InsSideBotLeft;

            return invert;
        }

        public override void DefineIns ()
        {            
            int indexWithSection = InsolationSpot.CountStepWithSection - 1;
            
            Cell cellTop = insCheck.startCellHelper.StartCell;            
            Cell cellBot = cellTop;
            Cell offset = new Cell();            

            if (isVertic)
            {
                // Для вертикальной секции, ЛЛУ справа       
                cellBot.Col -= indexWithSection;
                offset.Row = -1;

                // Торцевая инсоляция
                if (isEndSection())
                {
                    var cel = new Cell(cellTop.Row, cellTop.Col - 1);
                    InsSideTopRight = GetInsIndex(cel, isRequired: false);
                    cel.Col--;
                    InsSideBotRight = GetInsIndex(cel, isRequired: false);
                    cel.Row -= countStep - 1;
                    InsSideBotLeft = GetInsIndex(cel, isRequired: false);
                    cel.Col++;
                    InsSideTopLeft = GetInsIndex(cel, isRequired: false);
                }
            }
            else
            {
                // Для горизонтальной секции, ЛЛУ сверху                    
                cellBot.Row += indexWithSection;
                offset.Col = -1;

                // Торцевая инсоляция
                if (isEndSection())
                {
                    var cel = new Cell(cellTop.Row + 1, cellTop.Col);
                    InsSideTopRight = GetInsIndex(cel, isRequired: false);
                    cel.Row++;
                    InsSideBotRight = GetInsIndex(cel, isRequired: false);
                    cel.Col -= countStep - 1;
                    InsSideBotLeft = GetInsIndex(cel, isRequired: false);
                    cel.Row--;
                    InsSideTopLeft = GetInsIndex(cel, isRequired: false);
                }                
            }            

            for (int i = 0; i < countStep; i++)
            {
                InsTop[i] = GetInsIndex(cellTop);
                InsBot[i] = GetInsIndex(cellBot);

                cellTop.Offset(offset);
                cellBot.Offset(offset);                
            }
            // реверс нижней инс?
            InsBot = InsBot.Reverse().ToArray();
        }        
    }
}
