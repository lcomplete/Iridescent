using System;
using System.Configuration;

namespace Iridescent.Utils.Setting
{
    /// <summary>
    /// AppConfig中的AppSettings中的配置
    /// </summary>
    public static class AppSetting
    {

        /// <summary>
        /// 通过setting键名获取配置，未获取到时返回默认值
        /// </summary>
        /// <param name="settingKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static T GetSetting<T>(string settingKey, T defaultValue=default(T)) where T : IConvertible
        {
            string value = ConfigurationManager.AppSettings[settingKey];

            return !string.IsNullOrEmpty(value) ? Common.ValueConverter.Parse<T>(value, defaultValue) : defaultValue;
        }

    }
}
