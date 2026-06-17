using ContractLoader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLoader.ElmaUseCases
{
    public class CreateFileUseCase
    {
    }

    public class CreateFilePayload
    {
        [JsonProperty("FileGUIDFrom1C")]
        public string FileGUIDFrom1C { get; set; }

        [JsonProperty("FileChangeDateFrom1C")]
        public string FileChangeDateFrom1C { get; set; }

        [JsonProperty("FileModifiedUserFrom1C")]
        public string[] FileModifiedUserFrom1C { get; set; }

        [JsonProperty("OutgoingContractId")]
        public string OutgoingContractId { get; set; }

        [JsonProperty("File")]
        public List<FileAttachment> File { get; set; }

        [JsonProperty("FileCreatedUser")]
        public string FileCreatedUser { get; set; }

        [JsonProperty("FileCreatedDate1C")]
        public string FileCreatedDate1C { get; set; }
        

    }
}
