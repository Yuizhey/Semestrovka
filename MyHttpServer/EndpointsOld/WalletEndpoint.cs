using System.Data.SqlClient;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyServer.services;

namespace MyServer.Endpoints;

public class WalletEndpoint : EndpointBase
{
    [Get("wallet")]
    public IHttpResponseResult GetLoginPage()
    {
        if (!IsAuthorized(Context)) // Проверяем авторизацию
        {
            return Redirect("/auth/login");
        }
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Pages", "Wallet", "my-wallet.html");

        if (!File.Exists(filePath))
        {
            return Html("<h1>404 - File Not Found</h1>");
        }

        // string content = File.ReadAllText(filePath);
        // return Html(content);
        var cookie = Context.Request.Cookies["session-token"];
        string token = cookie.Value;

        // Получаем имя пользователя через SessionStorage
        string userName = SessionStorage.GetUserId(token);
        var balance = GetUserBalance(userName);
        // Загружаем шаблон и заменяем плейсхолдер
        string content = File.ReadAllText(filePath);
        content = content.Replace("{{TotalBalance}}", balance.ToString());
        content = content.Replace("{{Login}}", userName);
        return Html(content);
    }
    
    private string GetUserNameFromToken(string token)
    {
        // Разделяем токен, предполагая формат "Name:Login"
        var parts = token.Split(':');
        return parts.Length > 0 ? parts[0] : "Гость";
    }
    
    private bool IsAuthorized(HttpRequestContext context)
    {
        // Проверка наличия Cookie с session-token
        if (context.Request.Cookies.Any(c => c.Name == "session-token"))
        {
            var cookie = context.Request.Cookies["session-token"];
            return SessionStorage.ValidateToken(cookie.Value);
        }

        return false;
    }
    private int GetUserBalance(string username)
    {
        int balance = 0; // Инициализируем переменную для хранения баланса
        string query = "SELECT TotalBalance FROM Users WHERE Login = @Login"; // SQL-запрос для получения баланса

        using (var sqlConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            sqlConnection.Open(); // Открываем соединение

            using (var command = new SqlCommand(query, sqlConnection))
            {
                command.Parameters.AddWithValue("@Login", username); // Добавляем параметр

                // Выполняем команду и читаем результат
                var result = command.ExecuteScalar(); // Получаем одно значение (баланс)
                
                if (result != null)
                {
                    balance = Convert.ToInt32(result); // Преобразуем результат в decimal
                }
            }
        }
        return balance; // Возвращаем баланс
    }
}
