using ContractLoader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ContractLoader.ElmaUseCases
{
    public class CreateFileUseCase
    {
        public static async Task<(bool, string)> UploadContractFile(HttpClient httpClient, ExcelRecord excelRecord, string contractId)
        {
            string pathToFile = parseFilePath(excelRecord.PathToFile);
            FileAttachment fileAttachment = await loadFileFromPath(pathToFile);
            CreateFilePayload payload = new()
            {
                File = [fileAttachment],
                OutgoingContractId = [contractId]
            };
            if (excelRecord.DocumentGuid is not null) payload.FileGUIDFrom1C = excelRecord.DocumentGuid;
            if (excelRecord.FileModifiedDate is not null) payload.FileChangeDateFrom1C = excelRecord.FileModifiedDate;
            if (excelRecord.FileModifiedUser is not null) payload.FileGUIDFrom1C = excelRecord.FileModifiedUser;
            if (excelRecord.Author is not null) payload.FileCreatedUser = excelRecord.Author;
            if (excelRecord.CreationDate is not null) payload.FileChangeDateFrom1C = excelRecord.CreationDate;

            
            return (true, "");
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
}
