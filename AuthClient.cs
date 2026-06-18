using System.Net;
using System.Net.Http.Headers;

namespace ContractLoader
{
    public class AuthClient(string _loginToken, string _host)
    {
        private readonly string loginToken = _loginToken;

        private readonly string host = _host;

        public async Task<string> Auth(bool isProd)
        {
            string authEndoint = host + "api/auth";
            Uri baseAddress = new(authEndoint);
            var cookieContainer = new CookieContainer();
            using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            using var client = new HttpClient(handler) { BaseAddress = baseAddress };

            if (!isProd)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginToken);
            } 
            else
            {
                cookieContainer.Add(baseAddress, new Cookie("vtoken", loginToken));
            }

            //for dev
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginToken);

            //for prod
            //cookieContainer.Add(baseAddress, new Cookie("vtoken", loginToken));

            HttpResponseMessage response = await client.GetAsync(baseAddress);
            Console.WriteLine($"Auth response status code: {response.StatusCode}");
            string resultText = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"resultText: {resultText}");
            HttpHeaders headers = response.Headers;

            if (!isProd)
            {                
                return _loginToken;// - for dev
            }
            else
            {
                // for prod
                string authToken = "";
                string responseToken = "";
                if (headers.TryGetValues("token", out IEnumerable<string> values))
                {
                    responseToken = values.First();
                    authToken = values.First();
                }
                return authToken;
            }

            
        }
    }
}
