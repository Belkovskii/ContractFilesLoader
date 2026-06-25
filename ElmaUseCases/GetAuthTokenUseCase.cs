namespace ContractLoader.ElmaUseCases
{
    public class GetAuthTokenUseCase
    {
        public static async Task<string> GetAuthToken(LoginAuthDataSet dataSet)
        {
            Console.WriteLine($"dataSet.host: {dataSet.host}");
            LoginClient loginClient = new(dataSet.host, dataSet.client);
            var token = await loginClient.LoginAndGetToken(dataSet.username, dataSet.userPassword);
            if (token == null || token.Length < 5)
            {
                Console.WriteLine("Did not manage to get token");
                Console.ReadKey();
                throw new Exception("Did not get token");
            }

            AuthClient authClient = new(token, dataSet.host);
            var authToken = await authClient.Auth(dataSet.isProd);
            if (!string.IsNullOrEmpty(authToken))
            {
                Console.WriteLine("Succesfully logged-in");
                return authToken;
            }
            else
            {
                Console.WriteLine("Got authentication error");
                return "";
            }
        }

        public struct LoginAuthDataSet(string host, HttpClient client, string username, string userPassword, bool isProd)
        {
            internal string host = host;
            internal HttpClient client = client;
            internal string username = username;
            internal string userPassword = userPassword;
            internal bool isProd = isProd;
        }
    }
}
