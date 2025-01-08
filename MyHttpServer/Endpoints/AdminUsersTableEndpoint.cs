using System.Data.SqlClient;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyHttpServer.Helpers;
using MyORMLibrary;
using Server.Models;

namespace MyHttpServer.Endpoints;

public class AdminUsersTableEndpoint : EndpointBase
{
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
    
    [Post("admin/users/add")]
    public IHttpResponseResult AddUser(Object user)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(user);
       
            // Десериализуем JSON-строку в объект типа User
            var newUser = System.Text.Json.JsonSerializer.Deserialize<User>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<User>(dbConnection);

                // Вставка нового пользователя и получение Id
                var createdUser = context.Create(newUser, "Users");

                // Если метод Create возвращает объект User с заполненным Id, возвращаем Id
                return Json(new { success = true, id = createdUser.Id });
            }
        }
        catch (Exception ex)
        {
            // Возвращаем false и сообщение об ошибке
            return Json(new { success = false, message = ex.Message });
        }
    }
    
    [Post("admin/users/delete")]
    public IHttpResponseResult DeleteUser(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            
            var newUser = System.Text.Json.JsonSerializer.Deserialize<DeleteRequestModel>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<User>(dbConnection);

      
                context.Delete(int.Parse(newUser.Id), "Users");

                
                return Json(new { success = true });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    
    [Post("admin/users/update")]
    public IHttpResponseResult UpdateUser(Object obj)
    {
        try
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            
            var newUser = System.Text.Json.JsonSerializer.Deserialize<User>(jsonString);

            using (var dbConnection = new SqlConnection(AppConfig.GetInstance().ConnectionString))
            {
                var context = new OrmContext<User>(dbConnection);

      
                context.Update(newUser.Id, newUser, "Users");

                
                return Json(new { success = true, login = newUser.Login, password = newUser.Password, email = newUser.Email });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}