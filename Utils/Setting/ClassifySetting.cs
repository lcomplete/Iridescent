using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;
using Iridescent.Utils.Common;

namespace Iridescent.Utils.Setting
{
    /// <summary>
    /// 分类配置
    /// </summary>
    public class ClassifySetting
    {
        private static readonly Dictionary<Type, object> _syncFileRoots = new Dictionary<Type, object>();

        private static readonly object _syncRoot = new object();

        internal const string SETTING_PATH = "__settings";

        private static object GetLock(Type t)
        {
            lock (_syncRoot)
            {
                if (!_syncFileRoots.ContainsKey(t))
                    _syncFileRoots.Add(t, new object());

                return _syncFileRoots[t];
            }
        }

        public static T GetSetting<T>() where T : class ,ISetting
        {
            string filePath = GetFilePath(typeof(T));
            T setting = null;

            if (File.Exists(filePath))
            {
                string cacheKey = "common_classify_setting_" + typeof(T).Name;
                setting = HttpRuntime.Cache.Get(cacheKey) as T;

                if (setting == null)
                {
                    lock (GetLock(typeof(T)))
                    {
                        setting = XmlSerializerUtils.Deserialize<T>(filePath);
                        if (setting != null)
                        {
                            HttpRuntime.Cache.Insert(cacheKey, setting, new CacheDependency(filePath));
                        }
                    }
                }
            }

            return setting;
        }

        public static void SaveSetting(ISetting setting)
        {
            if(setting==null)
                return;

            string filePath = GetFilePath(setting.GetType());
            lock (GetLock(setting.GetType()))
            {
                string dir = GetFileDir();
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                XmlSerializerUtils.Serializer(filePath, setting);
            }
        }

        private static string GetFilePath(Type type)
        {
            return GetFileDir() + type.Name + ".config";
        }

        private static string GetFileDir()
        {
            string dir = HttpContext.Current != null
                              ? HttpContext.Current.Server.MapPath("~/" + SETTING_PATH + "/")
                              : AppDomain.CurrentDomain.BaseDirectory + "/" + SETTING_PATH + "/";
            return dir;
        }
    }
}
