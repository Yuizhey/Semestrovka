using HttpServerLibrary.Attributes;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyServer.services;
using ServerLibrary.HttpResponse;

namespace MyServer.Endpoints;

public class DashboardEndpoint : EndpointBase
{
    [Get("dashboard")]
    public IHttpResponseResult GetPage()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Pages", "Dashboard", "index.html");

        if (!IsAuthorized(Context)) // Проверяем авторизацию
        {
            return Redirect("/auth/login");
        }

        // Получаем токен из куки
        var cookie = Context.Request.Cookies["session-token"];
        string token = cookie.Value;

        // Получаем имя пользователя через SessionStorage
        string userName = SessionStorage.GetUserId(token);

        // Загружаем шаблон и заменяем плейсхолдер
        string content = File.ReadAllText(filePath);
        content = content.Replace("{{Name}}", userName);

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
}