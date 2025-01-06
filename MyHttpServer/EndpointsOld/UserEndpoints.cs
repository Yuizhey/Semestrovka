using System.Data.SqlClient;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyORMLibrary;
using Server.Models;

namespace MyServer.Endpoints;

public class UserEndpoints : EndpointBase
{
    [Get("user")]
    public IHttpResponseResult GetUserById(int id)
    {
        var context = new OrmContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));

        var user = context.ReadById(id);
        return Json(user);
    }

    // [Get("users")]
    // public IHttpResponseResult GetAllUsers()
    // {
    //     var context = new ORMContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));
    //
    //     var user = context.Where(u => u.Id == 3 || u.Id == 2);
    //     return Json(user);
    // }
    //
    // [Get("deleteuser")]
    // public void DeleteUser(int id)
    // {
    //     var context = new ORMContext<User>(new SqlConnection(AppConfig.GetInstance().ConnectionString));
    //
    //     context.Delete(id,"Users");
    // }
    
}    