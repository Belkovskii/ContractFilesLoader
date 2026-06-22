using ClosedXML.Excel;
using ContractLoader.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Concurrent;

namespace ContractLoader.ExcelParser
{
    public class ExcelParseHelper(string path) : IExcelParser, IDisposable
    {
        private bool _disposed = false;
        public XLWorkbook Workbook {  get; set; } = new XLWorkbook(path);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
           // Workbook.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( !_disposed)
            {
                if (disposing)
                {
                    Workbook?.Dispose();
                }
                _disposed = true;
            }
        }
        ~ExcelParseHelper()
        {
            Dispose(false);
        }

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
                        RecordName = row.Cell(9).Value.ToString(),
                        Author = row.Cell(10).Value.ToString(),
                        CreationDate = row.Cell(12).Value.ToString(),
                        FullFileName1C = row.Cell(14).Value.ToString(),
                        CurrentFileModifiedBy = row.Cell(16).Value.ToString(),
                        FileModifiedDate = row.Cell(17).Value.ToString(),
                        PathToFile = row.Cell(21).Value.ToString(),
                        ApproverUser1C = row.Cell(25).Value.ToString(),
                        FileType1C = row.Cell(26).Value.ToString(),
                        FileModifiedUser = row.Cell(27).Value.ToString(),                        
                    };
                    excelRecords.Add(record);
                }
            }
            Console.WriteLine($"Парсинг усешно завершен, обнаружено {excelRecords.Count} записей.");
            return excelRecords;
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
                    Console.WriteLine($"при записи в файл обнаружен fileGuid {fileGuid} и результат {message}");
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
