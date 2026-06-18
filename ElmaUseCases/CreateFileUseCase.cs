using ContractLoader.ExcelParser;
using ContractLoader.FileLoader;
using ContractLoader.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Timers;
using static ContractLoader.ElmaUseCases.CheckContractUseCase;

namespace ContractLoader.ElmaUseCases
{
    public record struct FileUploadData(
        HttpClient httpClient,
        ExcelRecord excelRecord,
        string contractId,
        string generalPathToFiles,
        string host,
        string token
    );

    public class CreateFileUseCase
    {
        
        public static async Task<(bool, string)> UploadContractFile(FileUploadData fileUploadData)
        {
            var(httpClient, excelRecord, contractId, generalPathToFiles, host, token) = fileUploadData;
            if (excelRecord.PathToFile is not string)
            {
                return (false, "no path to file in excel row");
            }
            string pathToFile = PathParser.GetPathToFile(generalPathToFiles, excelRecord.PathToFile);
            FileUploadManager fileLoader = new(httpClient, token, host);
            (FileAttachment? fileAttachment, string resultMessage) = await fileLoader.GetFileBodyAndUpload(pathToFile);
            if (fileAttachment is not null)
            {
                string payloadJson = getCreateFileRequestPayload(fileAttachment, excelRecord, contractId);
                var url = "pub/v1/app/dogovor/OtherFiles/create";
                var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string newRecordIdOrError = await GetNewRecordIdOrError(response);
                    return (newRecordIdOrError.Contains("error"), newRecordIdOrError);                        
                }
                else
                {
                    return (false, "error");
                }
            }
            else
            {
                return (false, resultMessage);
            }
        }

        private static async Task<string> GetNewRecordIdOrError(HttpResponseMessage response)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<FileUploadApiResponse>(responseContent);
            if (apiResponse is null) return "error: could not parse response offile upload";
            if (apiResponse.Success)
            {
                return apiResponse.Item.Id;
            }
            else
            {
                return apiResponse.Error;
            }
            
        }

        private static string getCreateFileRequestPayload(FileAttachment fileAttachment, ExcelRecord excelRecord, string contractId)
        {
            CreateFilePayload payload = new()
            {
                File = [fileAttachment],
                OutgoingContractId = [contractId]
            };
            if (excelRecord.DocumentGuid is not null) payload.FileGUIDFrom1C = excelRecord.DocumentGuid;
            if (excelRecord.FileModifiedDate is not null) payload.FileChangeDateFrom1C = excelRecord.FileModifiedDate;
            if (excelRecord.FileModifiedUser is not null) payload.FileModifiedUserFrom1C = excelRecord.FileModifiedUser;
            if (excelRecord.Author is not null) payload.FileCreatedUser = excelRecord.Author;
            if (excelRecord.CreationDate is not null) payload.FileChangeDateFrom1C = excelRecord.CreationDate;
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            };            
            return JsonConvert.SerializeObject(payload, jsonSerializerSettings);
        }
        
    }

    public class CreateFilePayload
    {
        [JsonProperty(nameof(FileGUIDFrom1C))]
        public string? FileGUIDFrom1C { get; set; }

        [JsonProperty(nameof(FileChangeDateFrom1C))]
        public string? FileChangeDateFrom1C { get; set; }

        [JsonProperty(nameof(FileModifiedUserFrom1C))]
        public string? FileModifiedUserFrom1C { get; set; }

        [JsonProperty(nameof(OutgoingContractId))]
        public string[]? OutgoingContractId { get; set; }

        [JsonProperty(nameof(File))]
        public List<FileAttachment>? File { get; set; }

        [JsonProperty(nameof(FileCreatedUser))]
        public string? FileCreatedUser { get; set; }

        [JsonProperty(nameof(FileCreatedDate1C))]
        public string? FileCreatedDate1C { get; set; }
        
    }

    public class RootObject
    {
        [JsonProperty("context")]
        public CreateFilePayload context { get; set; }
    }

    public class FileUploadApiItemResponse
    {
        [JsonPropertyName("__id")]
        public string Id { get; set; }
    }


    public class FileUploadApiResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public FileUploadApiItemResponse Item { get; set; }
    }
}
