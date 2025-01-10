using System.Data.SqlClient;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyHttpServer.Helpers;
using MyORMLibrary;
using Server.Models;

namespace MyHttpServer.Endpoints;

public class AdminMoviesEndpoint : EndpointBase
{
    [Get("admin/movies")]
    public IHttpResponseResult GetMoviesTable()
    {

        var engine = new HtmlTemplateEngine();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", "AdminMoviesTable.html");
        var fileText = File.ReadAllText(filePath);
        // var template = TemplateStorage.MovieDetailsTemplate;
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<Movie>(dbConnection);
            var users = context.GetAll("Movies");
            var template = TemplateStorage.AdminMoviesTable;
            return Html(engine.Render(fileText,users,template));
        }
    }
    
    [Post("admin/movies/delete")]
    public IHttpResponseResult DeleteMovie(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            
            var newUser = System.Text.Json.JsonSerializer.Deserialize<DeleteRequestModel>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<Movie>(dbConnection);

      
                context.Delete(newUser.Id, "Movies");

                
                return Json(new { success = true });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    
    [Post("admin/movies/add")]
    public IHttpResponseResult AddMovie(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
       
            // Десериализуем JSON-строку в объект типа User
            var newMovie = System.Text.Json.JsonSerializer.Deserialize<Movie>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<Movie>(dbConnection);

                // Вставка нового пользователя и получение Id
                var createdMovie = context.Create(newMovie, "Movies");

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
    
    [Post("admin/movies/update")]
    public IHttpResponseResult UpdateMovie(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            
            var newMovie = System.Text.Json.JsonSerializer.Deserialize<Movie>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<Movie>(dbConnection);

      
                context.Update(newMovie.Id, newMovie, "Movies");

                
                return Json(new { success = true, ruTitle = newMovie.RuTitle, imageSource = newMovie.ImageSource, releaseYear = newMovie.ReleaseYear, status = newMovie.Status });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}