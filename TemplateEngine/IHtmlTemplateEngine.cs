namespace TemplateEngine
{
    public interface IHtmlTemplateEngine 
    {
        string Render(string template, string placeholder, string data);

        string Render(string template, object obj);
        string Render<T>(string template, T obj);
    }
}
