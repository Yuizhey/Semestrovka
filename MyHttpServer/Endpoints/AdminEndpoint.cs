using System.Data.SqlClient;
using System.Net;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyHttpServer.Helpers;

using Server.Models;

namespace MyHttpServer.Endpoints;

public class AdminEndpoint : EndpointBase
{
    [Get("admin")]
    public IHttpResponseResult GetAdminPage()
    {
        if (AuthorizedHelper.IsAuthorized(Context))
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", "admin.html");
            var fileText = File.ReadAllText(filePath);
            return Html(fileText);
        }

        return Redirect("/films?error=UnauthorizedAccess");
    }
}