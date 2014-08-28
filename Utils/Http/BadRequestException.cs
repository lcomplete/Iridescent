using System;

namespace Iridescent.Utils.Http
{
    /// <summary>
    /// 请求得到错误结果时引发的异常
    /// </summary>
    public class BadRequestException:ApplicationException
    {
        public string Response { get; private set; }

        public BadRequestException(string response)
        {
            Response = response;
        }

        public BadRequestException(string message,string response):base(message)
        {
            Response = response;
        }

        public BadRequestException(string message,string response,Exception innerException):base(message,innerException)
        {
            Response = response;
        }
    }
}
