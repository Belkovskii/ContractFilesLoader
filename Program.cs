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

    static Program()
    {
        //bearerToken = "bcf83280-631c-4ce6-8bf2-70cd49d79faa";
        //_host = "https://l42bom5pymlbs.elma365.ru/";
        //_httpClient = new()
        //{
        //    BaseAddress = new Uri("https://l42bom5pymlbs.elma365.ru/"),
        //    Timeout = TimeSpan.FromSeconds(30),
        //    DefaultRequestHeaders =
        //    {
        //        Authorization = new AuthenticationHeaderValue("Bearer", bearerToken)
        //    }
        //};
        //loginAuthDataSet = new()
        //{
        //    host = _host,
        //    client = _httpClient,
        //    username = "denis.belkovsky@masterdata.ru",
        //    userPassword = "Asz79!#58",
        //    isProd = false
        //};
        //pathToExcelFile = "C:\\xlsx_files\\январь-декабрь2026.xlsx";
        //pathToUploadFiles = "C:\\files_to_uload_to_contracts";
        
    }

    static void Initialize()
    {
        Console.Write("Введите bearer token для вашей системы: ");
        bearerToken = Console.ReadLine();

        Console.Write("Введите host вашей системы (обязательно со знаком \\ в конце): ");
        _host = Console.ReadLine();

        Console.Write("Введите ваш username: ");
        var username = Console.ReadLine();

        Console.Write("Введите ваш password: ");
        var userPassword = Console.ReadLine();

        Console.Write("Если вы загружаете данные на prod, введите Y или y и нажмите enter (любой другой символ будет означать, что вы загружаете данные в dev среду с новым API Elma365): ");
        var isProdInput = Console.ReadLine();
        bool isProd = string.Equals(isProdInput, "Y", StringComparison.OrdinalIgnoreCase);

        Console.Write("Введите путь до excel-файла с данными: ");
        pathToExcelFile = Console.ReadLine();

        Console.Write("Введите путь до общей папки, где лежат разделы по датам: ");
        pathToUploadFiles = Console.ReadLine();

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
            isProd = isProd
        };
    }

    static async Task Main(string[] args)
    {
        Initialize();
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






