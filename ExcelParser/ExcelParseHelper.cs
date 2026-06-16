using ClosedXML.Excel;
using ContractLoader.Interfaces;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLoader.ExcelParser
{
    public class ExcelParseHelper(string path) : IExcelParser
    {
        public XLWorkbook Workbook {  get; set; } = new XLWorkbook(path);

        public List<ExcelRecord> Parse()
        {
            Console.WriteLine("Парсим excel-файл...");
            var excelRecords = new List<ExcelRecord>();            
            var worksheet = Workbook.Worksheet(1);
            var rows = worksheet?.RangeUsed()?.RowsUsed().Skip(1);
            if (rows is not null)
            {
                foreach (var row in rows)
                {
                    var record = new ExcelRecord
                    {
                        FileGuid = row.Cell(2).Value.ToString(),
                        DocumentGuid = row.Cell(4).Value.ToString(),
                        FileModifiedDate = row.Cell(17).Value.ToString(),
                        PathToFile = row.Cell(21).Value.ToString(),
                        FileModifiedUser = row.Cell(27).Value.ToString()

                    };
                    excelRecords.Add(record);
                }
            }
            Console.WriteLine($"Парсинг усешно завершен, обнаружено {excelRecords.Count} записей.");
            return excelRecords;
        }

        public void UpdateRecord(string fileGuid, string message)
        {
            var worksheet = Workbook.Worksheet(1);
            var foundCells = worksheet.Column(1)
                .CellsUsed()
                .Where(cell => cell.GetFormattedString() == fileGuid);
            foreach (var cell in foundCells.ToList())
            {
                var row = cell.WorksheetRow();
                row.Cell(30).SetValue(message);
            }
            Workbook.Save();
        }

        public void UpdateRecords(ConcurrentDictionary<string, string> results)
        {
            var worksheet = Workbook.Worksheet(1);
            var rows = worksheet?.RangeUsed()?.RowsUsed().Skip(1);
            if (rows is not null)
            {
                foreach (var row in rows)
                {
                    var fileGuid = row.Cell(2).Value.ToString();
                    results.TryGetValue(fileGuid, out string? message);
                    if (message is string)
                    {
                        row.Cell(30).SetValue(message);
                    }                    
                }
            }
            Workbook.Save();
        }
    }
}
