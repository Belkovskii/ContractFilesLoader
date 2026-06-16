
using ContractLoader;
using ContractLoader.ExcelParser;
using ContractLoader.Interfaces;
using System.Collections.Concurrent;

ExcelParseHelper parser = new("C:\\xlsx_files\\январь-декабрь2026.xlsx");
Proceed(parser);

static void Proceed(IExcelParser parser)
{
       
    var excelRecords = parser.Parse();
    
    var resultsConcurrentDictionary = new ConcurrentDictionary<string, string>();

    Parallel.ForEach(excelRecords, async excelRecord =>
    {
        if (excelRecord.FileGuid is string)
        {
            var processResult = await ProcessRecord(excelRecord);
            resultsConcurrentDictionary.TryAdd(excelRecord.FileGuid, processResult);
        }        
    });

    

}

static async Task<string> ProcessRecord(ExcelRecord record)
{

    return "";
}

