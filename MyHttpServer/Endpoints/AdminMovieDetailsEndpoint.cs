using System.Data.SqlClient;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyHttpServer.Helpers;
using MyORMLibrary;
using Server.Models;

namespace MyHttpServer.Endpoints;

public class AdminMovieDetailsEndpoint : EndpointBase
{
    [Get("admin/movie-details")]
    public IHttpResponseResult GetMovieDetailsTable()
    {

        var engine = new HtmlTemplateEngine();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", "AdminMovieDetailsTable.html");
        var fileText = File.ReadAllText(filePath);
        // var template = TemplateStorage.MovieDetailsTemplate;
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<MovieDetails>(dbConnection);
            var users = context.GetAll("MovieDetails");
            var template = TemplateStorage.AdminMovieDetailsTable;
            return Html(engine.Render(fileText,users,template));
        }
    }
    
    [Post("admin/movie-details/delete")]
    public IHttpResponseResult DeleteMovie(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            
            var newUser = System.Text.Json.JsonSerializer.Deserialize<DeleteRequestModel>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<Movie>(dbConnection);

      
                context.Delete(newUser.Id, "MovieDetails");

                
                return Json(new { success = true });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    
    [Post("admin/movie-details/add")]
    public IHttpResponseResult AddMovie(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
       
            // Десериализуем JSON-строку в объект типа User
            var newMovie = System.Text.Json.JsonSerializer.Deserialize<MovieDetails>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<MovieDetails>(dbConnection);

                // Вставка нового пользователя и получение Id
                var createdMovie = context.Create(newMovie, "MovieDetails");

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
    
    [Post("admin/movie-details/update")]
    public IHttpResponseResult UpdateMovie(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            
            var newMovie = System.Text.Json.JsonSerializer.Deserialize<MovieDetails>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<MovieDetails>(dbConnection);

      
                context.Update(newMovie.Id, newMovie, "MovieDetails");

                
                return Json(new { success = true, movieid = newMovie.MovieId, moviedescription = newMovie.MovieDescription, country = newMovie.Country, 
                    Quality = newMovie.Quality, engtitle = newMovie.EngTitle,  videourl = newMovie.VideoURL, producerid=newMovie.ProducerId});
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}