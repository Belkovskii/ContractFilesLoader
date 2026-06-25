using ContractLoader;
using ContractLoader.ElmaUseCases;
using ContractLoader.ExcelParser;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

class Program
{
    private static GetAuthTokenUseCase.LoginAuthDataSet loginAuthDataSet;
    private static HttpClient _httpClient;
    private static string bearerToken;
    private static string pathToExcelFile;
    private static string pathToUploadFiles;
    private static string _host;

    static Program(){}

    static void Initialize()
    {
        var isTest = true;
        if (!isTest) Console.Write("Input bearer token of your system: ");
        bearerToken = isTest ? "9ebe84a2-369b-4e35-89e5-72104a85b6f3" : Console.ReadLine();

        if (!isTest) Console.Write("Input host of your system: ");
        _host = isTest ? "https://contracting-dev.neadru.local/" : Console.ReadLine();

        if (!isTest) Console.Write("Input your username: ");
        var username = isTest ? "valentina.ivanova@logmol.ru" : Console.ReadLine();

        if (!isTest) Console.Write("Input your password: ");
        var userPassword = isTest ? "Jc!i&t0YIz" : Console.ReadLine();

        if (!isTest) Console.Write("Input Y or y for PROD or any key for other type of org): ");
       // var isProdInput = Console.ReadLine();
        //bool isProd = isTest ? "9ebe84a2-369b-4e35-89e5-72104a85b6f3" : string.Equals(isProdInput, "Y", StringComparison.OrdinalIgnoreCase);
        bool isProd = true;

        if (!isTest) Console.Write("Input excel file path: ");
        pathToExcelFile = isTest ? "D:\\Profiles\\belkovde\\Desktop\\loader\\январь-декабрь2026.xlsx" : Console.ReadLine();

        if (!isTest) Console.Write("Input files path: ");
        pathToUploadFiles = isTest ? "F:\\Storage_do" : Console.ReadLine();

        _httpClient = new()
        {
            BaseAddress = new Uri(_host),
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
            username = username,
            userPassword = userPassword,
            isProd = false//isProd
        };
    }

    static async Task Main(string[] args)
    {
        try
        {
            Initialize();
            var authToken = await GetAuthTokenUseCase.GetAuthToken(loginAuthDataSet);
            using ExcelParseHelper parser = new(pathToExcelFile);        
            await Proceed(parser, authToken);
        } 
        catch(Exception e)
        {
            Console.WriteLine($"final error: {e.ToString()}");
        }
        finally
        {
            Console.ReadKey();
        }
    }
    
    static async Task Proceed(ExcelParseHelper parser, string authToken)
    {
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10 };
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
                return "error: file is already loaded";
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






