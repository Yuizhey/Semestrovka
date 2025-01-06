using HttpServerLibrary.Core;
using MyServer.services;

namespace MyHttpServer.Helpers;

public static class AuthorizedCheck
{
    public static bool IsAuthorized(HttpRequestContext context)
    {
        // Проверка наличия куки с session-token
        var cookie = context.Request.Cookies.FirstOrDefault(c => c.Name == "session-token");
        if (cookie != null)
        {
            var isValid = SessionStorage.ValidateToken(cookie.Value);
            return isValid;
        }
        return false;  
    }
}