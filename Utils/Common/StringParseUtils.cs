using System.Collections.Generic;

namespace Iridescent.Utils.Common
{
    public static class StringParseUtils
    {
        /// <summary>
        /// 从用符号连接的数字中获得 整型集合
        /// </summary>
        /// <param name="joinString">用符号连接的数字</param>
        /// <param name="separator">连接字符串的符号</param>
        /// <returns></returns>
        public static IList<int> ParseJoinString(string joinString,char separator)
        {
            IList<int> result=new List<int>();
            if(!string.IsNullOrEmpty(joinString))
            {
                string[] splited = joinString.Split(separator);
                foreach (var s in splited)
                {
                    int i;
                    if(int.TryParse(s,out i))
                        result.Add(i);
                }
            }
            return result;
        } 

        /// <summary>
        /// 从用逗号链接的数字中获得 整型集合
        /// </summary>
        /// <param name="joinString"></param>
        /// <returns></returns>
        public static IList<int> ParseCommaJoinString(string joinString)
        {
            return ParseJoinString(joinString, ',');
        } 
    }
}
