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
        var template = TemplateStorage.MovieDetailsTemplate;
        using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
        {
            var context = new OrmContext<Movie>(dbConnection);
            var movie = context.ReadById(id); // Получение всех записей из таблицы
            renderedHtml = engine.Render(fileText, movie);
        }
        if (!AuthorizedCheck.IsAuthorized(Context))
        {
            return Html(engine.Render(renderedHtml, "{data}", "ВОЙТИ"));
        }
    
        return Html(engine.Render(renderedHtml, "{data}", "КАБИНЕТ"));
    }
}