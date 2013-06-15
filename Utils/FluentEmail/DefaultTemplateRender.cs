namespace Iridescent.Utils.FluentEmail
{
    public class DefaultTemplateRender:ITemplateRenderer
    {
        public string Parse<T>(string template, T model, bool isHtml = true)
        {
            //TODO enhance this
            return template;
        }
    }
}
