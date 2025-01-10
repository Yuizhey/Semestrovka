using System.Data.SqlClient;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyHttpServer.Helpers;
using MyORMLibrary;
using Server.Models;

namespace MyHttpServer.Endpoints;

public class AdminProducerEndpoint : EndpointBase
{
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
    
    [Post("admin/producer/delete")]
    public IHttpResponseResult DeleteProducer(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            
            var newUser = System.Text.Json.JsonSerializer.Deserialize<DeleteRequestModel>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<Producer>(dbConnection);

      
                context.Delete(newUser.Id, "Producer");

                
                return Json(new { success = true });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    
    [Post("admin/producer/add")]
    public IHttpResponseResult AddProducer(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
       
            // Десериализуем JSON-строку в объект типа User
            var newProducer = System.Text.Json.JsonSerializer.Deserialize<Producer>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<Producer>(dbConnection);

                // Вставка нового пользователя и получение Id
                var createdProducer = context.Create(newProducer, "Producer");

                // Если метод Create возвращает объект User с заполненным Id, возвращаем Id
                return Json(new { success = true, id = createdProducer.Id });
            }
        }
        catch (Exception ex)
        {
            // Возвращаем false и сообщение об ошибке
            return Json(new { success = false, message = ex.Message });
        }
    }
    
    [Post("admin/producer/update")]
    public IHttpResponseResult UpdateUser(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            
            var newProducer = System.Text.Json.JsonSerializer.Deserialize<Producer>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<Producer>(dbConnection);

      
                context.Update(newProducer.Id, newProducer, "Producer");

                
                return Json(new { success = true, name = newProducer.Name, birthCountry = newProducer.BirthCountry, birthYear = newProducer.BirthYear, directedFilmsCount = newProducer.DirectedFilmsCount });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}