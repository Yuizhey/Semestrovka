using System.Data.SqlClient;
using System.Net;
using System.Runtime.InteropServices.JavaScript;
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
}