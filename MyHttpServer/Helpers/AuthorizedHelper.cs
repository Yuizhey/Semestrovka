using System.Data.SqlClient;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using MyORMLibrary;
using MyServer.services;
using Server.Models;

namespace MyHttpServer.Helpers;

public static class AuthorizedHelper
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
    
    public static string GetUserLogin(string token)
    {
        var userId = SessionStorage.GetUserId(token);
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<User>(dbConnection);
            var user = context.ReadById(int.Parse(userId),"Users");
            return user.Login;
        }
    }
}