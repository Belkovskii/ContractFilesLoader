
using ClosedXML.Excel;
using ContractLoader.Interfaces;

using var workbook = new XLWorkbook("C:/xlsx_files/Файлы по документам Январь2026-Декабрь2026.xlsx");
var worksheet = workbook.Worksheet(1);
var firstDataRow = worksheet.Row(1);

Console.WriteLine("Содержимое первой строки и номера столбцов:");

foreach (var cell in firstDataRow.CellsUsed())
{    
    int columnNumber = cell.Address.ColumnNumber;
    Console.WriteLine($"Столбец: {columnNumber}, Значение: {cell.Value}");
}

static void Proceed(IExcelParser parser)
{
    var pathToExcelFile = "";
    var excelRecords = parser.Parse(pathToExcelFile);
}