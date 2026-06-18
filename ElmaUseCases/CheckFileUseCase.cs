using System.Text;
using System.Text.Json;

namespace ContractLoader.ElmaUseCases
{
    internal class CheckFileUseCase
    {
        public static async Task<bool> CheckIfFileInSystem(HttpClient httpClient, string documentGuid)
        {
            var url = "pub/v1/app/dogovor/OtherFiles/list";
            string payloadJson = $"{{\"filter\":{{\"and\":[{{\"and\":[{{\"eq\":[{{\"field\":\"FileGUIDFrom1C\"}},{{\"const\":\"{documentGuid}\"}}]}},{{\"eq\":[{{\"field\":\"__deletedAt\"}},null]}}]}}]}},\"offset\":0,\"limit\":1,\"order\":[]}}";
            var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                FileApiResponse? apiResponse = JsonSerializer.Deserialize<FileApiResponse>(responseBody);
                if (apiResponse is not null)
                {
                    return apiResponse.result.total > 0;
                }
            }
            return false;
        }

        public class FileItem
        {
            public string __id { get; set; }
        }

        public class FileResultWrapper
        {
            public List<FileItem> result { get; set; }
            public int total { get; set; }
        }

        public class FileApiResponse
        {
            public FileResultWrapper result { get; set; }
        }
    }
}
