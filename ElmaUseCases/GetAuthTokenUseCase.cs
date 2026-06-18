namespace ContractLoader.ElmaUseCases
{
    public class GetAuthTokenUseCase
    {
        public static async Task<string> GetAuthToken(LoginAuthDataSet dataSet)
        {
            LoginClient loginClient = new(dataSet.host, dataSet.client);
            var token = loginClient.LoginAndGetToken(dataSet.username, dataSet.userPassword);
            if (token == null || token.Length < 5)
            {
                Console.WriteLine("Не удалось получить токен");
                Console.ReadKey();
                throw new Exception("Did not get token");
            }

            AuthClient authClient = new(token, dataSet.host);
            var authToken = await authClient.Auth(dataSet.isProd);
            if (!string.IsNullOrEmpty(authToken))
            {
                Console.WriteLine("Логин и авторизация прошли успешно");
                return authToken;
            }
            else
            {
                Console.WriteLine("Произошла ошибка аутентификации");
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
