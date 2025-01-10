using System.Data.SqlClient;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyHttpServer.Helpers;
using MyORMLibrary;
using Server.Models;

namespace MyHttpServer.Endpoints;

public class AdminMovieStatisticEndpoint : EndpointBase
{
    [Get("admin/movie-stats")]
    public IHttpResponseResult GetMovieStatiscticTable()
    {

        var engine = new HtmlTemplateEngine();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", "AdminMovieStatisticTable.html");
        var fileText = File.ReadAllText(filePath);
        // var template = TemplateStorage.MovieDetailsTemplate;
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<MovieStatistic>(dbConnection);
            var users = context.GetAll("MovieStatistic");
            var template = TemplateStorage.AdminMovieStatsTable;
            return Html(engine.Render(fileText,users,template));
        }
    }
    
    [Post("admin/movie-stats/delete")]
    public IHttpResponseResult DeleteMovieStatistic(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            
            var newUser = System.Text.Json.JsonSerializer.Deserialize<DeleteRequestModel>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<MovieStatistic>(dbConnection);

      
                context.Delete(newUser.Id, "MovieStatistic");

                
                return Json(new { success = true });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    
    [Post("admin/movie-stats/add")]
    public IHttpResponseResult AddMovieStatistic(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
       
            // Десериализуем JSON-строку в объект типа User
            var newMovie = System.Text.Json.JsonSerializer.Deserialize<MovieStatistic>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<MovieStatistic>(dbConnection);

                // Вставка нового пользователя и получение Id
                var createdMovie = context.Create(newMovie, "MovieStatistic");

                // Если метод Create возвращает объект User с заполненным Id, возвращаем Id
                return Json(new { success = true, id = createdMovie.Id });
            }
        }
        catch (Exception ex)
        {
            // Возвращаем false и сообщение об ошибке
            return Json(new { success = false, message = ex.Message });
        }
    }
    
    [Post("admin/movie-stats/update")]
    public IHttpResponseResult UpdateMovieStatistic(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            
            var newMovie = System.Text.Json.JsonSerializer.Deserialize<MovieStatistic>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<MovieStatistic>(dbConnection);

      
                context.Update(newMovie.Id, newMovie, "MovieStatistic");

                
                return Json(new { success = true, kp_rating = newMovie.KP_Rating, imdb_rating = newMovie.IMDB_Rating, likes_count = newMovie.Likes_Count, dislikes_count = newMovie.Dislikes_Count, movie_id = newMovie.Movie_Id });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}