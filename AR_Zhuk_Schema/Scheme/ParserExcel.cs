using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;
using AR_Zhuk_Schema.Insolation;
using OfficeOpenXml;

namespace AR_Zhuk_Schema.Scheme
{
    class ParserExcel : ISchemeParser
    {
        /// <summary>
        /// Область для поиска - квадрат заданной длины от первой ячейки листа.
        /// </summary>
        const int SCANAREA = 100;        

        public List<HouseSpot> HouseSpots { get; private set; }

        ExcelWorksheet worksheet;        

        Module module;
        string spotName;

        public ParserExcel ()
        {            
        }

        public void Parse (string schemeFile)
        {
            HouseSpots = new List<HouseSpot>();
            // временный файл для копии схемы
            string tempFileScheme = Path.GetTempFileName();
            try
            {
                // копирование схемы во временный файл
                File.Copy(schemeFile, tempFileScheme, true);
                // Открытие файла схемы и парсинг домов
                using (var xlPackage = new ExcelPackage(new FileInfo(tempFileScheme)))
                {                    
                    // Первый лист
                    worksheet = xlPackage.Workbook.Worksheets[1];
                    // Разъдинение ячеек для упрощения
                    UnmergeCell();
                    // Определение домов
                    ScanHouses();

                    HouseSpots.Sort((h1, h2) => h1.SpotName.CompareTo(h2.SpotName));
                }
            }
            finally
            {
                // удаление временного файла
                File.Delete(tempFileScheme);
            }
        }

        private void ScanHouses ()
        {
            // от 1 ячейки - сканирование до заданного значения сканирования
            for (int c = 1; c <= SCANAREA; c++)
            {
                // проверка колонки
                for (int r = 1; r <= SCANAREA; r++)
                {
                    checkCell(new Cell(r, c));
                }                
            }
        }
        
        private void checkCell (Cell cell)
        {            
            // Если это ячейка пятна дома
            if (IsInsCell(cell) && !HouseSpots.Any(h => h.HasCell(cell)))
            {
                // Создание пятна дома
                var houseSpot = new HouseSpot(spotName, cell, this);
                HouseSpots.Add(houseSpot);
            }            
        }        

        /// <summary>
        /// Определение шагов - кол-во ячеек дома в заданном направлении
        /// </summary>        
        public List<Module> GetSteps (Cell cellStart, Cell direction, out Cell lastCell)
        {
            // определение длины ячеек в заданном направлении          
            List<Module> modules = new List<Module>();            
            var nextCell = cellStart;
            lastCell = cellStart;
            while (IsInsCell(nextCell))
            {
                modules.Add(module);
                lastCell = nextCell;
                nextCell = nextCell.Offset(direction);
            }
            return modules;              
        }

        /// <summary>
        /// Проверка - это ячейка инсоляции дома - т.е. внешняя ячейка дома с заданной инсоляцией по расчету.
        /// Все внешние ячейки дома должны быть со значением инсоляции
        /// </summary>        
        private bool IsInsCell (string cellValue)
        {
            bool isInsCell = false;
            module = null;

            // Если значение вида P#|[A-D]|# Например: P1, P1|C, P2|A|25, где P# - имя пятна, C - инсоляция, 25 - длина ячейки в метрах
            var match = Regex.Match(cellValue, @"^(?<spot>P\d?)\|?(?<ins>[A-D]?)\|?(?<length>\d*?[.|,]?\d*?)$", RegexOptions.ExplicitCapture);
            if (match.Success)
            {                
                spotName = match.Groups[1].Value;
                var insValue = match.Groups[2].Value;
                if (!string.IsNullOrEmpty(insValue))
                {
                    var groupLength = match.Groups[3];
                    double length = 0;
                    if (groupLength.Success)
                    {
                        if (!string.IsNullOrEmpty(groupLength.Value))
                        {
                            length = Convert.ToDouble(groupLength.Value.Replace(',', '.'));
                        }
                    }
                    module = Module.Create(insValue, length);
                    isInsCell = true;
                }                
            }
            return isInsCell;
        }
        public bool IsInsCell (Cell cell)
        {
            if (cell.Row <= 0 || cell.Col <= 0) return false;
            var cellValue = worksheet.Cells[cell.Row, cell.Col].Text;
            return IsInsCell(cellValue);
        }

        /// <summary>
        /// Разъединение ячеек - и заполнение каждой ранее объединенной ячейки
        /// </summary>
        private void UnmergeCell ()
        {    
            foreach (var mc in worksheet.MergedCells)
            {                
                if (mc == null) continue;
                var range = worksheet.Cells[mc];
                var mergedValue = range.First(c => c.Text != null).Text;                
                if (IsInsCell(mergedValue))
                {                    
                    range.Merge = false;
                    // Определение длины каждой ячейки - общую длину разделить на кол-во ячеек
                    double lengthOneCell = module.Length / range.Rows*range.Columns;
                    // значение для каждой ячейки
                    var cellValue = spotName + "|" + module.InsValue + "|" + module.Length; // P1|C|25 или P1||0 - допустимо
                    foreach (var cell in range)
                    {
                        cell.Value = cellValue;
                    }
                }
            }
        }
    }
}