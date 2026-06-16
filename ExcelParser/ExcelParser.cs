using ClosedXML.Excel;
using ContractLoader.Interfaces;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLoader.ExcelParser
{
    public class ExcelParser : IExcelParser
    {

         
    public ExcelRecord[] Parse(string path)
        {
            using var workbook = new XLWorkbook(path);
            var worksheet = workbook.Worksheet(1);
            var firstDataRow = worksheet.Row(1);

            Console.WriteLine("Содержимое первой строки и номера столбцов:");

            foreach (var cell in firstDataRow.CellsUsed())
            {
                // Порядковый номер столбца (A=1, B=2, ...)
                int columnNumber = cell.Address.ColumnNumber;

                // Выводим содержимое ячейки и номер столбца
                // Для корректного вывода чисел и дат используем .Value2
                Console.WriteLine($"Столбец: {columnNumber}, Значение: {cell.Value}");
            }

            throw new NotImplementedException();
        }
    }
}
