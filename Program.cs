
using ContractLoader;
using ContractLoader.ElmaUseCases;
using ContractLoader.ExcelParser;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;

class Program
{
    private static readonly string bearerToken = "bcf83280-631c-4ce6-8bf2-70cd49d79faa";

    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://l42bom5pymlbs.elma365.ru/"),
        Timeout = TimeSpan.FromSeconds(30),
        DefaultRequestHeaders =
        {
            Authorization = new AuthenticationHeaderValue("Bearer", bearerToken)
        }
    };
    static async Task Main(string[] args)
    {
        using (ExcelParseHelper parser = new("C:\\xlsx_files\\январь-декабрь2026.xlsx"))
        {
            await Proceed(parser);
        }

        //await Test(_httpClient);
    }

    static async Task Test(HttpClient httpClient)
    {
        // var isContractInSystem = await CheckContractUseCase.CheckIfContractInSystem(_httpClient, "0ccc9496-f029-11ee-a342-005056ae7f7f");
        // Console.WriteLine($"result: {isContractInSystem}");
        var isInSystem = await CheckFileUseCase.CheckIfFileInSystem(_httpClient, "53f9df17-ef2e-11f0-a35e-005056ae7f7f");
        Console.WriteLine($"is in system: {isInSystem}");
    }
    
    static async Task Proceed(ExcelParseHelper parser)
    {
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 50 };

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
        try
        {
            var contractId = await CheckContractUseCase.CheckIfContractInSystem(_httpClient, record.FileGuid);
            if (string.IsNullOrEmpty(contractId))
            {
                return "error: no contract found in system";
            }
            var isFileAlreadyLoaded = await CheckFileUseCase.CheckIfFileInSystem(_httpClient, record.DocumentGuid);
            if (isFileAlreadyLoaded)
            {
                return "error: file isalready loaded";
            }
            var uploadResult = uploadContractFile(record, );
            if (uploadResult.isSuccess)
            {
                return $"success: new file record id = {uploadResult.newRecordId}";
            }
            else
            {
                return $"error: {uploadResult.errorMessage}";
            }

        }
        catch (Exception ex)
        {
            return $"error: {ex.Message}";
        }        
    }
    
}






