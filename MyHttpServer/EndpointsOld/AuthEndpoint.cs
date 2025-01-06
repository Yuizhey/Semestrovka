using System.Data.SqlClient;
using System.Net;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyORMLibrary;
using MyServer.services;
using Server.Models;
using TemplateEngine;


namespace MyServer.Endpoints;

public class AuthEndpoint : EndpointBase
{
   
    
    [Get("auth/login")]
        public IHttpResponseResult GetLoginPage()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Pages", "Auth", "signin.html");

            if (!File.Exists(filePath))
            {
                return Html("<h1>404 - File Not Found</h1>");
            }

            string content = File.ReadAllText(filePath);
            return Html(content);
        }

    [Post("auth/login")]
        public IHttpResponseResult Login(string login, string password)
        {
            // Проверяем входные данные
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                string path = Path.Combine("Templates", "Pages", "Auth", "signin.html");
                var loginPage = File.ReadAllText(path);
                return Html(loginPage);
            }

            login = login.Trim();
            password = password.Trim();

            string connectionString = @AppConfig.GetInstance().ConnectionString;

            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string query = "SELECT Login FROM Users WHERE Login = @Login AND Password = @Password";

            using (var command = new SqlCommand(query, sqlConnection))
            {
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Получаем имя пользователя
                        string name = reader["Login"].ToString();
                        string token = Guid.NewGuid().ToString(); // Генерируем токен

                        // Сохраняем токен и имя пользователя в памяти
                        SessionStorage.SaveSession(token, name);

                        // Устанавливаем куку с токеном
                        var cookie = new Cookie("session-token", token)
                        {
                            HttpOnly = true,
                            Secure = false,
                            Path = "/" // Доступно на всём сайте
                        };
                        Context.Response.Cookies.Add(cookie);

                        // Загружаем HTML-шаблон и заменяем плейсхолдер
                        // var dashboardPage = File.ReadAllText(@"Templates/Pages/Dashboard/index.html");
                        // dashboardPage = dashboardPage.Replace("{{Name}}", name);

                        return Redirect("/dashboard");
                    }
                }
            }
            
            return Redirect("/auth/login");
        }
    
    [Get("auth/register")]
    public IHttpResponseResult GetRegisterPage()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Pages", "Auth", "signup.html");

        if (!File.Exists(filePath))
        {
            return Html("<h1>404 - File Not Found</h1>");
        }

        string content = File.ReadAllText(filePath);
        return Html(content);
    }
    
    // [Post("auth/register")]
    // public IHttpResponseResult Register(string login, string password)
    // {
    //     // Проверяем входные данные
    //     if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
    //     {
    //         var loginPage = File.ReadAllText(@"Templates/Pages/Auth/signup.html");
    //         return Html(loginPage);
    //     }
    //
    //     login = login.Trim();
    //     password = password.Trim();
    //
    //     string connectionString = @AppConfig.GetInstance().ConnectionString;
    //
    //     using var sqlConnection = new SqlConnection(connectionString);
    //     sqlConnection.Open();
    //
    //     string query = "SELECT Name FROM Users WHERE Login = @Login AND Password = @Password";
    //
    //     using (var command = new SqlCommand(query, sqlConnection))
    //     {
    //         command.Parameters.AddWithValue("@Login", login);
    //         command.Parameters.AddWithValue("@Password", password);
    //
    //         using (var reader = command.ExecuteReader())
    //         {
    //             if (reader.Read())
    //             {
    //                 return Redirect("/auth/login");
    //             }
    //         }
    //     }
    //
    //     var loginPageFallback = File.ReadAllText(@"Templates/Pages/Auth/signup.html");
    //     return Redirect("/auth/register");
    // }
    [Post("auth/register")]
    public IHttpResponseResult Register(string login, string password)
    {
        // Проверяем входные данные
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            string path = Path.Combine("Templates", "Pages", "Auth", "signup.html");
            var loginPage = File.ReadAllText(path);
            return Html(loginPage);
        }

        login = login.Trim();
        password = password.Trim();

        string connectionString = AppConfig.GetInstance().ConnectionString;

        using var sqlConnection = new SqlConnection(connectionString);
        sqlConnection.Open();

        // Проверяем, существует ли пользователь (по логину)
        string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login";

        using (var command = new SqlCommand(query, sqlConnection))
        {
            command.Parameters.AddWithValue("@Login", login);

            int userCount = (int)command.ExecuteScalar();
    
            if (userCount > 0)
            {
                // Если пользователь уже существует, перенаправляем на страницу логина
                return Redirect("/auth/login");
            }
        }

        // Если пользователя нет, добавляем его в базу данных с TotalBalance = 0
        string insertUserQuery = "INSERT INTO Users (Login, Password, TotalBalance) VALUES (@Login, @Password, 0)";

        using (var insertCommand = new SqlCommand(insertUserQuery, sqlConnection))
        {
            insertCommand.Parameters.AddWithValue("@Login", login);
            insertCommand.Parameters.AddWithValue("@Password", password); // Сохраняем пароль в открытом виде

            insertCommand.ExecuteNonQuery();
        }

        // После успешной регистрации перенаправляем на страницу логина
        return Redirect("/auth/login");
    }
}