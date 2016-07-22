using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    class CellInsOrdinary
    {
        private const int WithSectionModules = 4;
        private InsCheckOrdinary insCheck;

        public string[] InsTop { get; set; }
        public string[] InsBot { get; set; }

        public CellInsOrdinary (InsCheckOrdinary insCheck)
        {
            this.insCheck = insCheck;            
        }

        public void DefineIns ()
        {
            int countStep = insCheck.section.CountStep;            
            bool isVertic = insCheck.isVertical;

            InsTop = new string[countStep];
            InsBot = new string[countStep];

            int indexWithSection = WithSectionModules - 1;

            int rowTop;
            int rowBot;
            int colTop;
            int colBot;
            int stepRow;
            int stepCol;

            if (!isVertic)
            {
                // Для горизонтальной секции, ЛЛУ сверху                    
                rowTop = insCheck.indexRowStart;
                rowBot = rowTop + indexWithSection;
                stepCol = -1;
                colTop = insCheck.indexColumnStart-1;
                colBot = colTop;
                stepRow = 0;
            }
            else
            {
                // Для вертикальной секции, ЛЛУ справа                    
                colTop = insCheck.indexColumnStart+ indexWithSection;
                colBot = colTop - indexWithSection;
                rowTop = insCheck.indexRowStart-1;
                stepRow = -1;
                rowBot = rowTop;
                stepCol = 0;
            }            

            for (int i = 0; i < countStep; i++)
            {
                InsTop[i] = getInsIndex(colTop, rowTop);
                InsBot[i] = getInsIndex(colBot, rowBot);
                rowTop += stepRow;
                rowBot += stepRow;
                colTop += stepCol;
                colBot += stepCol;
            }
            // реверс нижней инс?
            InsBot = InsBot.Reverse().ToArray();
        }

        private string getInsIndex (int row, int col)
        {
            var cellValue = insCheck.insSpot.Matrix[row, col];
            string resInsIndex = string.Empty;
            var splitSpot = cellValue.Split('|');
            if(splitSpot.Length>1)
            {
                resInsIndex = splitSpot[1];
                // проверка допустимого индекса инсоляции
                if (!RoomInsulation.AllowedIndexes.Contains(resInsIndex))
                {
                    throw new Exception($"Недопустимый индекс инсоляции в задании - '{resInsIndex}', в ячейке [c{col},r{row}].\n " +
                         $"Допустимые индексы инсоляции {string.Join(", ", RoomInsulation.AllowedIndexes)}");
                }
            }
            else
            {
                throw new Exception($"Не задан индекс инсоляции в ячейке [c{col},r{row}].");
            }
            return resInsIndex;
        }
    }
}
