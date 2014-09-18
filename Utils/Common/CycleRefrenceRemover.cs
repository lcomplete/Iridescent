using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Iridescent.Utils.Common
{
    /// <summary>
    /// 去除循环引用
    /// </summary>
    public class CycleRefrenceRemover
    {
        /// <summary>
        /// 实体
        /// </summary>
        public Object Entity { get; private set; }

        public CycleRefrenceRemover(object entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// 移除循环引用
        /// </summary>
        public void RemoveCycle()
        {
            RemoveCycle(Entity);
        }

        /// <summary>
        /// 移除循环引用
        /// </summary>
        /// <param name="topEntity">顶层实体</param>
        private void RemoveCycle(object topEntity)
        {
            var enumEntity = topEntity as IEnumerable;
            if (enumEntity != null)
            {
                //若顶层实体为列表对象，则依次针对子对象移除循环引用
                foreach (var entity in enumEntity)
                {
                    RemoveCycle(entity);
                }
            }
            else
            {
                var pathContainer = new HashSet<object>() { topEntity };
                BreakCycle(pathContainer, topEntity);
            }
        }

        /// <summary>
        /// 消除路径中的循环引用
        /// </summary>
        /// <param name="pathContainer"></param>
        /// <param name="lastNode"></param>
        private void BreakCycle(HashSet<object> pathContainer, object lastNode)
        {
            Dictionary<PropertyInfo, IList<object>> childNodes = GetChildNodes(lastNode);
            foreach (KeyValuePair<PropertyInfo, IList<object>> childNode in childNodes)
            {
                PropertyInfo propInfo = childNode.Key;
                IList<object> nodes = childNode.Value;

                foreach (object node in nodes)
                {
                    if (pathContainer.Contains(node))
                    {
                        RemoveChild(lastNode, node, propInfo);
                    }
                    else
                    {
                        HashSet<object> newPath = ShallowCopyHashSet(pathContainer);
                        newPath.Add(node);
                        BreakCycle(newPath, node);
                    }
                }
            }
        }

        /// <summary>
        /// 浅拷贝hashset
        /// </summary>
        /// <param name="hashSet"></param>
        /// <returns></returns>
        private HashSet<object> ShallowCopyHashSet(HashSet<object> hashSet)
        {
            var newHashSet = new HashSet<object>(hashSet);
            return newHashSet;
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNode"></param>
        /// <param name="childPropInfo"></param>
        private void RemoveChild(object parentNode, object childNode, PropertyInfo childPropInfo)
        {
            object directChild = childPropInfo.GetValue(parentNode, null);
            if (childNode == directChild)
            {
                childPropInfo.SetValue(parentNode, null, null);
            }
            else
            {
                var listChild = directChild as IEnumerable;
                if (listChild != null)
                {
                    MethodInfo removeMethod = directChild.GetType().GetMethod("Remove");
                    if (removeMethod != null)
                    {
                        removeMethod.Invoke(directChild, new object[] { childNode });
                    }
                }

            }
        }

        /// <summary>
        /// 获取子节点（属性对应子节点）
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Dictionary<PropertyInfo, IList<object>> GetChildNodes(object node)
        {
            Type type = node.GetType();
            PropertyInfo[] propInfos = type.GetProperties();

            var childNodes = new Dictionary<PropertyInfo, IList<object>>();
            foreach (PropertyInfo propInfo in propInfos)
            {
                if (IsPossibleCycleRefrenceType(propInfo.PropertyType))
                {
                    Object childNode = propInfo.GetValue(node, null);
                    if (childNode == null)
                        continue;

                    childNodes.Add(propInfo, new List<object>());

                    var listChild = childNode as IEnumerable;
                    if (listChild != null)
                    {
                        foreach (object singleNode in listChild)
                        {
                            if (IsPossibleCycleRefrenceType(singleNode.GetType()))
                            {
                                childNodes[propInfo].Add(singleNode);
                            }
                        }
                    }
                    else
                    {
                        childNodes[propInfo].Add(childNode);
                    }
                }
            }

            return childNodes;
        }

        /// <summary>
        /// 类型是否可能产生循环引用
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsPossibleCycleRefrenceType(Type type)
        {
            return !type.IsValueType && type != typeof(string);
        }
    }
}
