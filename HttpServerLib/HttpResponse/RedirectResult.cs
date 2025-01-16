using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;

namespace ServerLibrary.HttpResponse;

public class RedirectResult: IHttpResponseResult
{
    private readonly string _location;
    public RedirectResult(string location)
    {
        _location = location;
    }
 
    public void Execute(HttpRequestContext context)
    {
        var response = context.Response;
        response.StatusCode = 302;
        response.Headers.Add("Location", _location);
        response.Close();// Заголовок для указания пути
    }
}