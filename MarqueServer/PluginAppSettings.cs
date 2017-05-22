using System;
using System.Configuration;
using System.Windows.Forms;

namespace MarquesasServer
{
    public class PluginAppSettings : IDisposable
    {
        private Configuration _PluginConfig = null;

        private Configuration PluginConfig
        {
            get
            {
                if (_PluginConfig != null) return _PluginConfig;

                return LoadConfiguration();
            }
        }

        private Configuration LoadConfiguration()
        {
            Configuration oConfiguration = null;
            string exeConfigPath = this.GetType().Assembly.Location;
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

        public void ReloadConfiguration()
        {
            LoadConfiguration();

        }

        public Boolean GetBoolean(String key)
        {
            Boolean bAppSetting;

            Boolean.TryParse(GetString(key), out bAppSetting);

            return bAppSetting;
        }

        public int GetInt(String key)
        {
            int iAppSetting;

            int.TryParse(GetString(key), out iAppSetting);

            return iAppSetting;
        }

        public Decimal GetDecimal(String key)
        {
            Decimal dAppSetting;

            Decimal.TryParse(GetString(key), out dAppSetting);

            return dAppSetting;
        }

        public string GetString(string key)
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

        public void SetString(string key, string value)
        {
            if (PluginConfig != null && key != null && value != null)
            {
                if (PluginConfig.AppSettings.Settings[key] != null)
                {
                    PluginConfig.AppSettings.Settings[key].Value = value;
                }
                else
                {
                    MessageBox.Show("Warning: The AppSetting Key (" + key + ") doesn't exist in " + PluginConfig.FilePath);
                }
            }
        }

        public void Save()
        {
            PluginConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void Dispose()
        {
            _PluginConfig = null;
        }
    }
}