using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContractLoader.ElmaUseCases
{
    public class CheckContractUseCase
    {
        public static async Task<string> CheckIfContractInSystem(HttpClient httpClient, string fileGuid)
        {
            string payloadJson = $"{{\"filter\":{{\"and\":[{{\"and\":[{{\"eq\":[{{\"field\":\"ExternalGUID\"}},{{\"const\":\"{fileGuid}\"}}]}},{{\"eq\":[{{\"field\":\"__deletedAt\"}},null]}}]}}]}},\"offset\":0,\"limit\":1,\"order\":[]}}";
            var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            var url = $"pub/v1/app/dogovor/outgoingcontracts/list";
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                if (responseBody is not null)
                {
                    ApiResponse? apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);
                    if (apiResponse is not null)
                    {
                        if (apiResponse.result.total > 0 && apiResponse.result.result.Count > 0)
                        {
                            return apiResponse.result.result[0].__id;
                        }
                    }
                }                
            }
            return "";
        }

        public class ContractItem
        {
            public string __id { get; set; }
        }

        public class ResultWrapper
        {
            public List<ContractItem> result { get; set; }
            public int total { get; set; }
        }

        public class ApiResponse
        {
            public ResultWrapper result { get; set; }
        }
    }
}
