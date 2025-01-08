using System.Data.SqlClient;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyHttpServer.Helpers;
using MyORMLibrary;
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
        // var template = TemplateStorage.MovieDetailsTemplate;
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<Movie>(dbConnection);
            var movie = context.ReadById(id,"Movies");
            // var contextSecond = new OrmContext<MovieStatistic>(dbConnection);// Получение всех записей из таблицы
            // var stats = contextSecond.FirstOrDefault(m => m.Movie_Id == movie.Id);
            // var contextThird = new OrmContext<MovieDetails>(dbConnection);
            // var details = contextThird.FirstOrDefault(m => m.MovieId == movie.Id);
            // var contextFour = new OrmContext<Producer>(dbConnection);
            // var producer = contextFour.FirstOrDefault(m=> m.Id == details.ProducerId);
            var contextSecond = new OrmContext<MovieDetails>(dbConnection);
            var details = contextSecond.GetAll("MovieDetails").FirstOrDefault(x => x.MovieId == movie.Id);
            var contextThird = new OrmContext<MovieStatistic>(dbConnection);
            var stats = contextThird.GetAll("MovieStatistic").FirstOrDefault(x => x.Movie_Id == movie.Id);
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
                Name = producer.Name,
                Quality = details.Quality,
                VideoURL = details.VideoURL
            };
            renderedHtml = engine.Render(fileText, obj);
        }
        if (!AuthorizedCheck.IsAuthorized(Context))
        {
            return Html(engine.Render(renderedHtml, "{data}", "ВОЙТИ"));
        }
    
        return Html(engine.Render(renderedHtml, "{data}", "КАБИНЕТ"));
    }
}