

using HttpServerLibrary.Attributes;
using HttpServerLibrary.Core;
using HttpServerLibrary.Core.HttpResponse;

namespace MyServer.Endpoints;

internal class SendHomeworkEndpoint : EndpointBase
{
    [Get("homework")]
    public IHttpResponseResult GetHomeworkPage()
    {
        Console.WriteLine("GET:homework");
        var localPath = "homework.html";
        var responseText = GetResponseText(localPath);
        return Html(responseText);
    }
    
    // [Post("homework")]
    // public IHttpResponseResult SendEmailHomework(string date, string email)
    // {
    //     Console.WriteLine($"{date} {email}");
    //
    //     if (HomeworkLinksProcessor.Links is not null && HomeworkLinksProcessor.Links.ContainsKey(date))
    //     {
    //         var emailService = new EmailService(AppConfig.GetInstance().EmailServiceConfiguration);
    //         emailService.SendEmail(email, "Homework", $"You can find homework on {date} here: {HomeworkLinksProcessor.Links[date]}");
    //         Console.WriteLine("Message sent");
    //     }
    //
    //     else
    //     {
    //         return Html("No such homework file");
    //         Console.WriteLine("No such homework file");
    //     }
    //     return GetHomeworkPage();
    // }
}