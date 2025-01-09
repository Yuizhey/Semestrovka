using System.Data.SqlClient;
using HttpServerLibrary.Configurations;
using MyHttpServer.Helpers;
using MyORMLibrary;
using Server.Models;

namespace MyServer.services;

public static class SessionStorage
{

    private static readonly Dictionary<string, string> _sessions = new Dictionary<string, string>();
    
    // Сохранение токена и его соответствующего ID пользователя
    public static void SaveSession(string token, string userId)
    {
        _sessions[token] = userId;
    }
 
    // Проверка токена
    public static bool ValidateToken(string token)
    {
        return _sessions.ContainsKey(token);
    }
 
    // Получение ID пользователя по токену
    public static string GetUserId(string token)
    {
        return _sessions.TryGetValue(token, out var userId) ? userId : null;
    }

    public static string GetUserLogin(string token)
    {
        var userId = GetUserId(token);
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<User>(dbConnection);
            var user = context.ReadById(int.Parse(userId),"Users");
            return user.Login;
        }
    }
}