using System.Data.SqlClient;
using System.Net;
using System.Text.Json;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyHttpServer.Helpers;
using MyORMLibrary;
using Server.Models;

namespace MyHttpServer.Endpoints;

public class AdminEndpoint : EndpointBase
{
    [Get("admin")]
    public IHttpResponseResult GetAdminPage()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", "admin.html");
        var fileText = File.ReadAllText(filePath);
        return Html(fileText);
    }
    
    [Get("admin/users")]
    public IHttpResponseResult GetUsersTable()
    {

        var engine = new HtmlTemplateEngine();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", "AdminUsersTable.html");
        var fileText = File.ReadAllText(filePath);
        // var template = TemplateStorage.MovieDetailsTemplate;
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<User>(dbConnection);
            var users = context.GetAll("Users");
            var template = TemplateStorage.AdminUsersTable;
            return Html(engine.Render(fileText,users,template));
        }
    }
    
    [Get("admin/producer")]
    public IHttpResponseResult GetProducerTable()
    {

        var engine = new HtmlTemplateEngine();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", "AdminProducerTable.html");
        var fileText = File.ReadAllText(filePath);
        // var template = TemplateStorage.MovieDetailsTemplate;
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<Producer>(dbConnection);
            var users = context.GetAll("Producer");
            var template = TemplateStorage.AdminProducerTable;
            return Html(engine.Render(fileText,users,template));
        }
    }
    
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
    
    // [Post("admin/users/add")]
    // public IHttpResponseResult AddUser(HttpListenerRequest request)
    // {
    //     try
    //     {
    //         using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
    //         {
    //             var body = reader.ReadToEnd();
    //             var newUser = JsonSerializer.Deserialize<User>(body);
    //
    //             using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
    //             {
    //                 var context = new OrmContext<User>(dbConnection);
    //                 context.Create(newUser, "Users");
    //             }
    //         }
    //
    //         return Json(new { message = "Запись успешно добавлена!" });
    //     }
    //     catch (Exception ex)
    //     {
    //         return Json(new { message = "Ошибка добавления записи: " + ex.Message });
    //     }
    // }
    
    [Post("admin/users/add")]
    public IHttpResponseResult AddUser(HttpListenerRequest request)
    {
        try
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                var body = reader.ReadToEnd();

                if (string.IsNullOrEmpty(body))
                {
                    return Json(new { message = "Тело запроса не может быть пустым" });
                }

                var newUser = JsonSerializer.Deserialize<User>(body);

                if (newUser == null)
                {
                    return Json(new { message = "Ошибка десериализации данных пользователя" });
                }

                using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
                {
                    var context = new OrmContext<User>(dbConnection);
                    context.Create(newUser, "Users");
                }
            }

            return Json(new { message = "Запись успешно добавлена!" });
        }
        catch (Exception ex)
        {
            return Json(new { message = "Ошибка добавления записи: " + ex.Message });
        }
    }

}