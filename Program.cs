using ContractLoader;
using ContractLoader.ElmaUseCases;
using ContractLoader.ExcelParser;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

class Program
{
    private static GetAuthTokenUseCase.LoginAuthDataSet loginAuthDataSet;
    private static readonly HttpClient _httpClient;
    private static readonly string bearerToken;
    private static readonly string pathToExcelFile;
    private static readonly string pathToUploadFiles;
    private static readonly string _host;
    static Program()
    {
        bearerToken = "bcf83280-631c-4ce6-8bf2-70cd49d79faa";
        _host = "https://l42bom5pymlbs.elma365.ru/";
        _httpClient = new()
        {
            BaseAddress = new Uri("https://l42bom5pymlbs.elma365.ru/"),
            Timeout = TimeSpan.FromSeconds(30),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", bearerToken)
            }
        };
        loginAuthDataSet = new()
        {
            host = _host,
            client = _httpClient,
            username = "denis.belkovsky@masterdata.ru",
            userPassword = "Asz79!#58",
            isProd = false
        };
        pathToExcelFile = "C:\\xlsx_files\\январь-декабрь2026.xlsx";
        pathToUploadFiles = "C:\\files_to_uload_to_contracts";
        
    }

    static async Task Main(string[] args)
    {
        var authToken = await GetAuthTokenUseCase.GetAuthToken(loginAuthDataSet);
        using ExcelParseHelper parser = new(pathToExcelFile);
        await Proceed(parser, authToken);
    }
    
    static async Task Proceed(ExcelParseHelper parser, string authToken)
    {
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 50 };
        var excelRecords = parser.Parse();
        var resultsConcurrentDictionary = new ConcurrentDictionary<string, string>();
        //foreach (var excelRecord in excelRecords)
        //{
        //    if (excelRecord.FileGuid is string)
        //    {
        //        var processResult = await ProcessRecord(excelRecord, authToken);
        //        resultsConcurrentDictionary.TryAdd(excelRecord.FileGuid, processResult);
        //    }
        //}
        await Parallel.ForEachAsync(excelRecords, parallelOptions, async (excelRecord, cancellationToken) =>
        {
            if (excelRecord.FileGuid is string)
            {
                var processResult = await ProcessRecord(excelRecord, authToken);
                resultsConcurrentDictionary.TryAdd(excelRecord.FileGuid, processResult);
            }
        });
        parser.UpdateRecords(resultsConcurrentDictionary);
        Console.ReadKey();
    }
    
    
    static async Task<string> ProcessRecord(ExcelRecord record, string authToken)
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
            FileUploadData fileUploadData = new(_httpClient, record, contractId, pathToUploadFiles, _host, authToken);
            (bool uploadResult, string message) = await CreateFileUseCase.UploadContractFile(fileUploadData);
            if (uploadResult)
            {
                return $"success: new file record id = {message}";
            }
            else
            {
                return $"error: {message}";
            }
        }
        catch (Exception ex)
        {
            return $"error: {ex.Message}";
        }  
    }
    
}






