using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ContractLoader.FileLoader
{
    public class CalloutManager(HttpClient client, string endpoint, string? payloadJSON, string token, HttpMethod httpMethod)
    {
        HttpClient HttpClient { get; set; } = client;

        string Endpoint { get; set; } = endpoint;

        string? PayloadJSON { get; set; } = payloadJSON;

        string Token { get; set; } = token;

        HttpMethod Method { get; set; } = httpMethod;

        public Dictionary<string, string> Headers { get; set; } = [];

        public async Task<string> SendRequest(out HttpStatusCode statusCode)
        {
            using var request = new HttpRequestMessage(Method, Endpoint);
            StringContent? contentBody = null;
            if (PayloadJSON != null)
            {
                contentBody = new StringContent(PayloadJSON, Encoding.UTF8, "application/json");
                request.Content = contentBody;
            }
            if (Token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }
            if (Headers != null)
            {
                foreach (var header in Headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            var response = await HttpClient.SendAsync(request);
            statusCode = response.StatusCode;
            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}
