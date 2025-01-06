using System.Data.SqlClient;
using System.Net;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyORMLibrary;
using MyServer.services;
using Server.Models;

namespace MyHttpServer.Endpoints;

public class MainPageEndpoint : EndpointBase
{
    [Get("films")]
    public IHttpResponseResult GetPage()
    {
        string renderedHtml;
        var engine = new HtmlTemplateEngine();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", "index.html");
        var fileText = File.ReadAllText(filePath);
    
        // Инициализация ORM и получение данных из таблицы MoviesTable
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<Movie>(dbConnection);
            var movies = context.GetAll("Movies"); // Получение всех записей из таблицы
    
            var itemTemplate = @"
                <div class='cards-item' onclick='location.href=""card.html""'>
                    <div class='cards-item-year'>
                        <p class='cards-item-year-info'>{releaseyear}</p>
                    </div>
                    <img src='images/test-image.jpeg' alt='' class='cards-item-img'>
                    <div class='cards-item-info'>
                        <p class='cards-item-info-name'>{rutitle}</p>
                    </div>
                    <div class='cards-item-marks'>
                        <p class='cards-item-marks-first'>КП <span class='cards-item-mark'>{status}</span></p>
                        <p class='cards-item-marks-second'>IMDB <span class='cards-item-mark'>{status}</span></p>
                    </div>
                </div>";

            
            // Преобразуем данные в шаблон
            renderedHtml = engine.Render(fileText, movies, itemTemplate);
        }
        if (!IsAuthorized(Context))
        {
            return Html(engine.Render(renderedHtml, "{data}", "ВОЙТИ"));
        }
    
        return Html(engine.Render(renderedHtml, "{data}", "КАБИНЕТ"));
    }


    [Post("films")]
    public IHttpResponseResult Login(string login, string password)
    {
        if (IsAuthorized(Context))
        {
            return Redirect("/films");
        }

        // Если нет — проверяем логин и пароль
        var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));

        // Ищем пользователя по логину
        var user = context.FirstOrDefaultByLogin(login);
    
        // Если пользователя нет или пароль неверный
        if (user == null || user.Password != password)
        {
            return Redirect("/films");
        }

        // Генерация нового токена для сессии и сохранение в cookies
        var token = Guid.NewGuid().ToString();
        Cookie nameCookie = new Cookie("session-token", token)
        {
            HttpOnly = true,
            Secure = false, // Если вы работаете не через HTTPS
            Path = "/",     // Делаем cookie доступными для всех маршрутов
            Expires = DateTime.Now.AddDays(1)
        };
        Context.Response.Cookies.Add(nameCookie);

        // Сохраняем сессию в хранилище
        SessionStorage.SaveSession(token, user.Id.ToString());

        // Перенаправляем на дашборд после успешного логина
        return Redirect("/films");
    }
    private bool IsAuthorized(HttpRequestContext context)
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