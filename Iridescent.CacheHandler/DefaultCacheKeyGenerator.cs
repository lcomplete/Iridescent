using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;

namespace Iridescent.CacheHandler
{
    class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        private readonly LosFormatter serializer = new LosFormatter(false, "");

        public string CreateCacheKey(MethodBase method, object[] inputs)
        {
            try
            {
                var sb = new StringBuilder();

                if (method.DeclaringType != null)
                {
                    sb.Append(method.DeclaringType.FullName);
                }
                sb.Append(':');
                sb.Append(method.Name);

                TextWriter writer = new StringWriter(sb);

                if (inputs != null)
                {
                    foreach (var input in inputs)
                    {
                        sb.Append(':');
                        if (input != null)
                        {
                            //可空DateTime序列化时存在问题 直接使用Ticks
                            var inputDateTime = input as DateTime?;
                            if (inputDateTime.HasValue)
                            {
                                sb.Append(inputDateTime.Value.Ticks);
                            }
                            else
                            {
                                //将参数序列化到stringbuilder
                                serializer.Serialize(writer, input);
                            }
                        }
                    }
                }

                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
