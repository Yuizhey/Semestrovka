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

public class CardPageEndpoint : EndpointBase
{
    [Get("card")]
public IHttpResponseResult GetCardPage(int id)
{
    string renderedHtml;
    var engine = new HtmlTemplateEngine();
    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", "card.html");
    var fileText = File.ReadAllText(filePath);
    
    try
    {
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<Movie>(dbConnection);
            var movie = context.ReadById(id, "Movies");
            
            // Если фильм не найден, возвращаем JSON с сообщением
            if (movie == null)
            {
                return Json("Information about movie not found" );
            }

            var contextSecond = new OrmContext<MovieDetails>(dbConnection);
            var details = contextSecond.GetAll("MovieDetails").FirstOrDefault(x => x.MovieId == movie.Id);
            
            // Проверяем наличие деталей фильма
            if (details == null)
            {
                return Json("Information about movie not found");
            }

            var contextThird = new OrmContext<MovieStatistic>(dbConnection);
            var stats = contextThird.GetAll("MovieStatistic").FirstOrDefault(x => x.Movie_Id == movie.Id);
            
            // Проверяем наличие статистики фильма
            if (stats == null)
            {
                return Json("Information about movie not found");
            }

            var contextFour = new OrmContext<Producer>(dbConnection);
            var producer = contextFour.GetAll("Producer").FirstOrDefault(x => x.Id == details.ProducerId);

            var obj = new CardPageMovie
            {
                RuTitle = movie.RuTitle,
                EngTitle = details.EngTitle,
                ReleaseYear = movie.ReleaseYear,
                ImageSource = movie.ImageSource,
                KP_Rating = stats.KP_Rating,
                IMDB_Rating = stats.IMDB_Rating,
                Likes_Count = stats.Likes_Count,
                Dislikes_Count = stats.Dislikes_Count,
                MovieDescription = details.MovieDescription,
                Country = details.Country,
                Name = producer?.Name ?? "Неизвестный", // Если продюсер не найден
                Quality = details.Quality,
                VideoURL = details.VideoURL
            };
            renderedHtml = engine.Render(fileText, obj);
        }

        if (!AuthorizedHelper.IsAuthorized(Context))
        {
            return Html(engine.Render(renderedHtml, "{data}", "ВОЙТИ"));
        }
        
        renderedHtml = engine.Render(renderedHtml, "{login}", AuthorizedHelper.GetUserLogin(Context.Request.Cookies.FirstOrDefault(c => c.Name == "session-token").Value));
        return Html(engine.Render(renderedHtml, "{data}", "КАБИНЕТ"));
    }
    catch (Exception ex)
    {
        return Html(ex.Message);
    }
}

    [Post("card/reaction")]
    public IHttpResponseResult PutReaction(Object obj)
    {
        try
        {
            // Проверяем авторизацию пользователя
            if (!AuthorizedHelper.IsAuthorized(Context))
            {
                return Json(new { success = false, message = "Чтобы ставить реакции вы должны войти в аккаунт." });
            }

            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
   
            // Десериализуем JSON-строку в объект типа CardReaction
            var newReaction = System.Text.Json.JsonSerializer.Deserialize<CardReaction>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<CardReaction>(dbConnection);

                // Здесь вы могли бы выполнять проверку на существование реакции
                // при необходимости

                // Вставка нового (или обновление существующего) пользователя и получение Id
                context.Update(newReaction.Id, newReaction, "MovieStatistic");

                // Если метод Create возвращает объект CardReaction с заполненным Id, возвращаем Id
                return Json(new { success = true, likescount = newReaction.Likes_Count, dislikescount = newReaction.Dislikes_Count });
            }
        }
        catch (Exception ex)
        {
            // Возвращаем false и сообщение об ошибке
            return Json(new { success = false, message = ex.Message });
        }
    }
}