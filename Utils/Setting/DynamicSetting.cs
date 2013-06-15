using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Iridescent.Utils.Common;

namespace Iridescent.Utils.Setting
{
    /// <summary>
    /// 动态配置
    /// </summary>
    public class DynamicSetting : IDisposable
    {
        public class ItemPair
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        internal const string SETTING_PATH = "__settings";

        private IList<ItemPair> _innerSettings;

        private string _filePath;

        private string _fileDir;

        private bool _changed;

        private static object _syncRoot = new object();

        public DynamicSetting()
        {
            string fileName = this.GetType().Name;
            _fileDir = HttpContext.Current != null
                             ? HttpContext.Current.Server.MapPath("~/" + SETTING_PATH + "/")
                             : AppDomain.CurrentDomain.BaseDirectory + "/" + SETTING_PATH + "/";
            _filePath = _fileDir + fileName + ".config";

            if (File.Exists(_filePath))
            {
                string cacheKey = "common_dynamic_setting";
                _innerSettings = HttpRuntime.Cache.Get(cacheKey) as List<ItemPair>;

                if (_innerSettings == null)
                {
                    lock (_syncRoot)
                    {
                        _innerSettings = XmlSerializerUtils.Deserialize<List<ItemPair>>(_filePath);

                        if (_innerSettings != null)
                        {
                            HttpRuntime.Cache.Insert(cacheKey, _innerSettings, new CacheDependency(_filePath));
                        }
                    }
                }
            }

            if (_innerSettings == null)
            {
                _innerSettings = new List<ItemPair>();
            }
        }

        public T GetSetting<T>(string key, T defaultValue = default(T)) where T : IConvertible
        {
            string value = this[key];

            return !string.IsNullOrEmpty(value) ? Common.ValueConverter.Parse<T>(value, defaultValue) : defaultValue;
        }

        public string this[string key]
        {
            get
            {
                lock (_syncRoot)
                {
                    ItemPair item = _innerSettings.FirstOrDefault(t => t.Key == key);
                    if (item != null)
                        return item.Value;

                    return null;
                }
            }
            set
            {
                lock (_syncRoot)
                {
                    ItemPair item = _innerSettings.FirstOrDefault(t => t.Key == key);
                    if (item != null)
                        item.Value = value;
                    else
                        _innerSettings.Add(new ItemPair() {Key = key, Value = value});

                    _changed = true;
                }
            }
        }

        public void Dispose()
        {
            Save();
        }

        private void Save()
        {
            if (_changed)
            {
                lock (_syncRoot)
                {
                    if (!Directory.Exists(_fileDir))
                        Directory.CreateDirectory(_fileDir);

                    XmlSerializerUtils.Serializer(_filePath, _innerSettings);
                }
            }
        }
    }
}
