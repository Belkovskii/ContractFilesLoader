using ContractLoader.FileLoader;
using Newtonsoft.Json;
using System.Net;

namespace ContractLoader
{
    public class LoginClient(string _host, HttpClient _client)
    {
        public static string authToken;

        private readonly string host = _host;

        private readonly HttpClient client = _client;

        public string LoginAndGetToken(string username, string password)
        {
            string loginJson = JsonConvert.SerializeObject(new LoginPayload(username, password));
            string loginEndpoint = host + "/guard/login";
            CalloutManager loginCalloutManager = new(client, loginEndpoint, loginJson, "", HttpMethod.Post);
            string logResponse = loginCalloutManager.SendRequest(out HttpStatusCode statusCode);
            if (statusCode == HttpStatusCode.OK)
            {
                ResponseJSON responseJson = JsonConvert.DeserializeObject<ResponseJSON>(logResponse) ?? new();
                string loginToken = responseJson.token;
                return loginToken;
            }
            Console.WriteLine($"Error login: status code: {statusCode}");
            return "";
        }
    }
    public class LoginPayload(string username_, string password_)
    {
        [JsonProperty("auth_login")]
        public string username = username_;

        [JsonProperty("password")]
        public string password = password_;

    }

    public class ResponseJSON
    {
        [JsonProperty("token")]
        public string token = "";
    }
}
