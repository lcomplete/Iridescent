namespace Iridescent.Utils.FluentEmail
{
    public interface ITemplateRenderer
    {
        string Parse<T>(string template, T model, bool isHtml = true); 
    }
}