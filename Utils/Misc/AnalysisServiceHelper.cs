using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.AnalysisServices.AdomdClient;

namespace Iridescent.Utils.Misc
{
    /// <summary>
    /// 分析服务查询帮助类
    /// </summary>
    public class AnalysisServiceHelper
    {

        /// <summary>
        /// 通过查询读取CellSet
        /// </summary>
        /// <param name="connectionString">分析服务连接字符串</param>
        /// <param name="command">查询命令</param>
        /// <returns></returns>
        public static CellSet ExecuteCellSet(string connectionString, string command)
        {
            using (AdomdConnection conn = new AdomdConnection(connectionString))
            {
                conn.Open();
                AdomdCommand cmd = new AdomdCommand(command, conn);
                CellSet cs = cmd.ExecuteCellSet();
                return cs;
            }
        }

        /// <summary>
        /// 从分析服务读取数据流 (使用后，一定要关闭DataReader)
        /// </summary>
        /// <param name="connectionString">分析服务连接字符串</param>
        /// <param name="command">查询命令</param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(string connectionString, string command)
        {
            using (AdomdConnection conn = new AdomdConnection(connectionString))
            {
                conn.Open();
                AdomdCommand cmd = new AdomdCommand(command, conn);
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
        }

        /// <summary>
        /// 通过查询读取List列表
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="connectionString">分析服务连接字符串</param>
        /// <param name="command">查询命令</param>
        /// <returns></returns>
        public static IList<T> ExecuteList<T>(string connectionString, string command) where T : new()
        {
            CellSet cs = ExecuteCellSet(connectionString, command);
            return ReadEntityList<T>(cs);
        }

        /// <summary>
        /// 执行，获取数量
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="command">查询命令</param>
        /// <returns></returns>
        public static int ExecuteCount(string connectionString, string command)
        {
            CellSet cs = ExecuteCellSet(connectionString, command);
            int count = (int)cs.Cells[0].Value;

            return count;
        }

        /// <summary>
        /// 从CellSet中读取出实体列表
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cs">CellSet</param>
        /// <returns></returns>
        public static IList<T> ReadEntityList<T>(CellSet cs) where T : new()
        {
            Dictionary<CellMemberAttribute, PropertyInfo> memberProperties = GetMemberProperties(typeof (T));
            Dictionary<string, int> captionIndex = new Dictionary<string, int>();
            TupleCollection columnTuples = cs.Axes[0].Set.Tuples;
            TupleCollection rowTuples = cs.Axes[1].Set.Tuples;
            IList<T> datalist = new List<T>();

            //获取表头对应的列索引
            for (int i = 0; i < columnTuples.Count; i++)
            {
                string caption = columnTuples[i].Members[0].Caption;
                CellMemberAttribute member =
                    memberProperties.Keys.FirstOrDefault(t => t.Caption == caption);
                if (member != null && !captionIndex.ContainsKey(member.Caption))
                {
                    captionIndex.Add(member.Caption, i);
                }
            }

            for (int row = 0; row < rowTuples.Count; row++)
            {
                var data = new T();
                foreach (var memberProperty in memberProperties)
                {
                    string caption = memberProperty.Key.Caption;
                    int index = memberProperty.Key.Index;
                    object value = null;
                    if (string.IsNullOrEmpty(caption))
                    {
                        value = rowTuples[row].Members[index].Caption; //处理row轴
                    }
                    else if (captionIndex.ContainsKey(caption))
                    {
                        value = cs.Cells[captionIndex[caption], row].Value;//处理column轴
                    }

                    if (value != null)
                    {
                        if (memberProperty.Value.PropertyType != typeof (string))
                        {
                            value = Convert.ChangeType(value, memberProperty.Value.PropertyType);
                        }
                        memberProperty.Value.SetValue(data, value, null);
                    }
                }

                datalist.Add(data);
            }

            return datalist;
        }

        /// <summary>
        /// 获取实体类型的属性信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Dictionary<CellMemberAttribute, PropertyInfo> GetMemberProperties(Type type)
        {
            var memberProperties = new Dictionary<CellMemberAttribute, PropertyInfo>();
            PropertyInfo[] properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                CellMemberAttribute member =
                    propertyInfo.GetCustomAttributes(typeof (CellMemberAttribute), false).FirstOrDefault() as
                        CellMemberAttribute;
                if (member != null)
                    memberProperties.Add(member, propertyInfo);
            }

            return memberProperties;
        }
    }
}