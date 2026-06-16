
using ContractLoader;
using ContractLoader.ExcelParser;
using ContractLoader.Interfaces;
using System.Collections.Concurrent;


using (ExcelParseHelper parser = new("C:\\xlsx_files\\январь-декабрь2026.xlsx"))
{
    await Proceed(parser);
}

static async Task Proceed(ExcelParseHelper parser)
{
    var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 50};
       
    var excelRecords = parser.Parse();
    
    var resultsConcurrentDictionary = new ConcurrentDictionary<string, string>();

    await Parallel.ForEachAsync(excelRecords, parallelOptions, async (excelRecord, cancellationToken) =>
    {
        if (excelRecord.FileGuid is string)
        {
            var processResult = await ProcessRecord(excelRecord);
            resultsConcurrentDictionary.TryAdd(excelRecord.FileGuid, processResult);
        }        
    });
    
    parser.UpdateRecords(resultsConcurrentDictionary);
}

static async Task<string> ProcessRecord(ExcelRecord record)
{
    //TODO
    return "success";
}

