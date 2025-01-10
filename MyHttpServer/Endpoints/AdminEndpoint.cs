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
}