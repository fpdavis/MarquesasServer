using System;
using System.Configuration;
using System.Reflection;
using System.Windows.Forms;

namespace CommonPluginHelper
{
    public static class PluginAppSettings
    {
        private static Configuration _PluginConfig = null;

        private static Configuration PluginConfig
        {
            get
            {
                if (_PluginConfig != null) return _PluginConfig;

                return LoadConfiguration();
            }
        }

        private static Configuration LoadConfiguration()
        {
            Configuration oConfiguration = null;
            string exeConfigPath = Assembly.GetCallingAssembly().Location;

            try
            {
                oConfiguration = ConfigurationManager.OpenExeConfiguration(exeConfigPath);
            }
            catch (Exception ex)
            {
                //handle errror here.. means DLL has no sattelite configuration file.
                MessageBox.Show("Error while trying to load Plugin Configuration file for Quote of the Day. " +
                                ex.Message);
            }

            _PluginConfig = oConfiguration;

            return _PluginConfig;
        }

        public static void ReloadConfiguration()
        {
            LoadConfiguration();

        }

        public static Boolean GetBoolean(String key)
        {
            Boolean.TryParse(GetString(key), out bool bAppSetting);

            return bAppSetting;
        }

        public static int GetInt(String key)
        {
            int.TryParse(GetString(key), out int iAppSetting);

            return iAppSetting;
        }

        public static Decimal GetDecimal(String key)
        {
            Decimal.TryParse(GetString(key), out decimal dAppSetting);

            return dAppSetting;
        }

        public static string GetString(string key)
        {
            // We will use the Plugin's App.config file if it exists AND it contains a key/value pair
            KeyValueConfigurationElement element = PluginConfig?.AppSettings.Settings[key];
            if (!string.IsNullOrEmpty(element?.Value))
                return element.Value;

            // Fall back to the application's app.config file
            if (ConfigurationManager.AppSettings[key] != null)
            {
                return ConfigurationManager.AppSettings[key];
            }

            return string.Empty;
        }

        public static void SetString(string key, string value)
        {
            if (PluginConfig != null && key != null && value != null)
            {
                if (PluginConfig.AppSettings.Settings[key] != null)
                {
                    PluginConfig.AppSettings.Settings[key].Value = value;
                }
                else
                {
                    PluginConfig.AppSettings.Settings.Add(key, value);
                }
            }
        }

        public static void Save()
        {
            PluginConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

    }
}