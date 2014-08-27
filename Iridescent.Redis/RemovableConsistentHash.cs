using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ServiceStack.Redis.Support;

namespace Iridescent.Redis
{
    /// <summary>
    /// 功能：可移除结点的一致性哈希对象
    /// 作者：娄晨
    /// 日期：2014-5-20
    /// </summary>
    public class RemovableConsistentHash<T>
    {
        /// <summary>
        /// hash函数
        /// </summary>
        private readonly Func<string, ulong> hashFunction = ConsistentHash<T>.Md5Hash;

        /// <summary>
        /// 环形Hash空间
        /// </summary>
        private readonly SortedDictionary<ulong, T> circle = new SortedDictionary<ulong, T>();

        /// <summary>
        /// 虚拟结点（为使缓存对象分布更均匀）复制个数 
        /// </summary>
        private const int Replicas = 200;

        public RemovableConsistentHash()
        {
        }

        public RemovableConsistentHash(IEnumerable<KeyValuePair<T, int>> nodes)
            : this(nodes, null)
        {
        }

        public RemovableConsistentHash(IEnumerable<KeyValuePair<T, int>> nodes, Func<string, ulong> hashFunction)
        {
            if (hashFunction != null)
                this.hashFunction = hashFunction;
            foreach (KeyValuePair<T, int> keyValuePair in nodes)
                AddTarget(keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>
        /// 获取结点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetTarget(string key)
        {
            if (circle.Count == 0)
                return default(T);

            if (circle.Count == 1)
                return circle[0];

            return circle[ConsistentHash<T>.ModifiedBinarySearch(circle.Keys.ToArray(), hashFunction(key))];
        }

        /// <summary>
        /// 添加一个结点到环形Hash空间
        /// </summary>
        /// <param name="node">结点</param>
        /// <param name="weight">权重, 可设置访问到结点的几率 </param>
        public void AddTarget(T node, int weight)
        {
            int num = weight > 0 ? weight * Replicas : Replicas;
            for (int index = 0; index < num; ++index)
                circle.Add(hashFunction(node.GetHashCode().ToString() + "-" + index.ToString()), node);
        }

        /// <summary>
        /// 从环形hash空间中移除一个结点
        /// </summary>
        /// <param name="node">结点</param>
        /// <param name="weight">权重</param>
        public void RemoveTarget(T node, int weight)
        {
            int num = weight > 0 ? weight * Replicas : Replicas;
            for (int i = 0; i < num; i++)
            {
                string identifier = node.GetHashCode().ToString() + "-" + i.ToString();
                ulong hashCode = hashFunction(identifier);
                if (circle.ContainsKey(hashCode))
                {
                    circle.Remove(hashCode);
                }
            }
        }

        /// <summary>
        /// 二分查找算法的变形. 
        /// 通过给定的数字，从已排序数组中匹配到下一个更大的数字，如果不存在这样的数字，则返回数组中第一个数字
        /// </summary>
        /// <param name="sortedArray">用于执行查找的已排序数组</param>
        /// <param name="val">给定的数字，用于从已排序数组中匹配到下一个更大的数字</param>
        /// <returns>
        /// 下一个更大的数字
        /// </returns>
        public static ulong ModifiedBinarySearch(ulong[] sortedArray, ulong val)
        {
            int lowBound = 0;
            int upBound = sortedArray.Length - 1;
            if (val < sortedArray[lowBound] || val > sortedArray[upBound])
                return sortedArray[0];
            while (upBound - lowBound > 1)
            {
                int middle = (upBound + lowBound) / 2;
                if (sortedArray[middle] >= val)
                    upBound = middle;
                else
                    lowBound = middle;
            }
            return sortedArray[upBound];
        }

        /// <summary>
        /// 使用md5，通过key生成无符号64位hash code
        /// </summary>
        /// <param name="key"/>
        /// <returns/>
        public static ulong Md5Hash(string key)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                return BitConverter.ToUInt64(hash, 0) ^ BitConverter.ToUInt64(hash, 8);
            }
        }

    }
}