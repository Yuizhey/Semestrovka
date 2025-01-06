using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configurations;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;
using MyServer.services;

namespace MyServer.Endpoints;


internal class SendEmailEndpoint : EndpointBase
{
    [Post("anime")]
    public IHttpResponseResult SendEmailAnime(object? requestBody)
    {
        return Json(requestBody);
    }
    
    [Post("login")]
    public void SendEmailLogin(string email, string password)
    {
        var emailService = new EmailService(AppConfig.GetInstance().EmailServiceConfiguration);
        emailService.SendEMail(email, "ENTER", "Вы вошли в систему!");
    }
    

    [Get("anime")]
    public IHttpResponseResult GetAnimePage()
    {
        Console.WriteLine("GET: anime");
        var localPath = "index.html";
        var responseText = GetResponseText(localPath);
        return Html(responseText);
    }

    [Get("login")]
    public IHttpResponseResult GetLoginPage()
    {
        Console.WriteLine("GET: login");
        var localPath = "login.html";
        var responseText = GetResponseText(localPath);
        return Html(responseText);
    }

    
}

