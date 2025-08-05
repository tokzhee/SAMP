using System.Configuration;

namespace App.Core.Utilities
{
    public static class ConfigurationUtility
    {
        public static string GetAppSettingValue(string appSettingsKey)
        {
            return ConfigurationManager.AppSettings[appSettingsKey];
        }
        public static string GetConnectionStringValue(string connectionStringName)
        {
            return ConfigurationManager.ConnectionStrings[connectionStringName].ToString();
        }
    }
}