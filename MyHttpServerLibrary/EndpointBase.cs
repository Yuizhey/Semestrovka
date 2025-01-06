using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core.HttpResponse;
using MyHttpServer.Core.HttpResponse;
using ServerLibrary.HttpResponse;


namespace HttpServerLibrary.Core;

public abstract class EndpointBase
{
    protected HttpRequestContext Context { get; private set; }

    internal void SetContext(HttpRequestContext context)
    {
        Context = context;
    }

    protected IHttpResponseResult Html(string html) => new HtmlResult(html);

    protected IHttpResponseResult Json(object data) => new JsonResult(data);
    protected IHttpResponseResult Redirect(string location) => new RedirectResult(location);

    protected string GetResponseText(string localPath)
    {
        var filePath = AppConfig.GetInstance().Path + localPath;
        if (!File.Exists(filePath))
        {
            filePath = AppConfig.GetInstance().Path + "404.html";
            if (!File.Exists(filePath))
            {
                return "error 404 file not found";
            }
        }
        var responseText = File.ReadAllText(filePath);
        return responseText;
    }
}