using System.Data.SqlClient;
using System.Net;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyHttpServer.Helpers;
using MyORMLibrary;
using MyServer.services;
using Server.Models;

namespace MyHttpServer.Endpoints;

public class MainPageEndpoint : EndpointBase
{
    [Get("films")]
    public IHttpResponseResult GetMainPage()
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
            var moviesId = movies.Select(i => i.Id).ToList();
            var contextSecond = new OrmContext<MovieStatistic>(dbConnection);
            var movieStats = contextSecond.GetAll("MovieStatistic").Where(i => moviesId.Contains(i.Id)).ToList(); // Убедитесь, что movieStats в виде списка
            var mainModel = new List<MainPageMovie>();
            var movieStatsDictionary = movieStats.ToDictionary(s => s.MovieId);
            for (int i = 0; i < movies.Count; i++)
            {
                var movieId = movies[i].Id;
                if (movieStatsDictionary.TryGetValue(movieId, out var movieStat))
                {
                    mainModel.Add(new MainPageMovie
                    {
                        Id = movies[i].Id,
                        RuTitle = movies[i].RuTitle,
                        ReleaseYear = movies[i].ReleaseYear,
                        ImageSource = movies[i].ImageSource,
                        KP_Rating = movieStat.KP_Rating,
                        IMDB_Rating = movieStat.IMDB_Rating
                    });
                }
                else
                {
                    mainModel.Add(new MainPageMovie
                    {
                        Id = movies[i].Id,
                        RuTitle = movies[i].RuTitle,
                        ReleaseYear = movies[i].ReleaseYear,
                        ImageSource = movies[i].ImageSource,
                        KP_Rating = 0, 
                        IMDB_Rating = 0 
                    });
                }
            }

            var itemTemplate = TemplateStorage.MovieCardTemplate;

            
            // Преобразуем данные в шаблон
            renderedHtml = engine.Render(fileText, mainModel, itemTemplate);
        }
        if (!AuthorizedCheck.IsAuthorized(Context))
        {
            return Html(engine.Render(renderedHtml, "{data}", TemplateStorage.UnauthorizedPlaceholder));
        }
    
        return Html(engine.Render(renderedHtml, "{data}", TemplateStorage.AuthorizedPlaceholder));
    }


    [Post("films")]
    public IHttpResponseResult Login(string login, string password)
    {
        if (AuthorizedCheck.IsAuthorized(Context))
        {
            return Redirect("/films");
        }

        // Если нет — проверяем логин и пароль
        var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));

        // Ищем пользователя по логину
        var user = context.FirstOrDefault(u => u.Login == login);
    
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
}