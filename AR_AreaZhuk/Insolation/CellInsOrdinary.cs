using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Значение инсолиции ячеек в прямой секции (в стандартном положении - горизонтально, ЛЛУ сверху)
    /// Сверху и Снизу
    /// Сбоку???
    /// </summary>
    class CellInsOrdinary : CellInsBase
    {
        public CellInsOrdinary (InsCheckOrdinary insCheck) : base(insCheck)
        {                       
        }

        public CellInsOrdinary Invert()
        {
            var invert = new CellInsOrdinary((InsCheckOrdinary)insCheck);
            invert.InsTop = InsBot.Reverse().ToArray();
            invert.InsBot = InsTop.Reverse().ToArray();
            return invert;
        }

        public void DefineIns ()
        {            
            int indexWithSection = CountStepWithSection - 1;
            
            Cell cellTop = new Cell();            
            Cell cellBot = new Cell();
            Cell offset = new Cell();            

            if (!isVertic)
            {
                // Для горизонтальной секции, ЛЛУ сверху                    
                cellTop.Row = insCheck.indexRowStart;
                cellTop.Col = insCheck.indexColumnStart - 1;

                cellBot.Row = cellTop.Row + indexWithSection;                                               
                cellBot.Col = cellTop.Col;

                offset.Col = -1;
            }
            else
            {
                // Для вертикальной секции, ЛЛУ справа       
                cellTop.Col = insCheck.indexColumnStart + indexWithSection;
                cellTop.Row = insCheck.indexRowStart - 1;

                cellBot.Col = cellTop.Col - indexWithSection;                
                cellBot.Row = cellTop.Row;

                offset.Row = -1;                
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
