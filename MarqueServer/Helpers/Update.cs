using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarquesasServer
{
    class Update : IDisposable
    {
        private PluginAppSettings oPluginAppSettings;

        public Update(PluginAppSettings oPluginAppSettings)
        {
            this.oPluginAppSettings = oPluginAppSettings;
            BeginCheck();
        }

        private void BeginCheck()
        {
            string sMyVersionDate = oPluginAppSettings.GetString("VersionDate");
            string sCurrentVersionDate = GetCurrentVersionDate();

            if (!string.IsNullOrWhiteSpace(sCurrentVersionDate) && sMyVersionDate != sCurrentVersionDate)
            {
                if (oPluginAppSettings.GetBoolean("AutomaticUpdates") || MessageBox.Show("A newer version of " + Properties.Resources.ApplicationName + " is available. Would you like to update?", "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string sSaveLocation = this.GetType().Assembly.Location;
                    if (GetCurrentVersion(sSaveLocation + ".newversion"))
                    {
                        try
                        {
                            File.Delete(sSaveLocation + ".previousversion");
                            File.Move(sSaveLocation, sSaveLocation + ".previousversion");
                            File.Move(sSaveLocation + ".newversion", sSaveLocation);

                            if (!oPluginAppSettings.GetBoolean("AutomaticUpdates"))
                            {
                                MessageBox.Show(
                                    Properties.Resources.ApplicationName + " has been updated. Update will be applied on next restart of LaunchBox/BigBox",
                                    "Update Successful", MessageBoxButtons.OK);
                            }

                            oPluginAppSettings.SetString("VersionDate", sCurrentVersionDate);
                            oPluginAppSettings.Save();

                        }
                        catch (Exception exception)
                        {
                            if (!oPluginAppSettings.GetBoolean("AutomaticUpdates"))
                            {
                                MessageBox.Show(
                                    Properties.Resources.ApplicationName + " could not be updated. Exception: " + exception.Message,
                                    "Update Unsuccessful", MessageBoxButtons.OK);
                            }
                        }
                    }
                }
            }
            else if (!oPluginAppSettings.GetBoolean("AutomaticUpdates") && string.IsNullOrWhiteSpace(sCurrentVersionDate))
            {
                MessageBox.Show(
                    "A version number could not be found. This may indicate a network issue or a configuration issue. Check VersionUrl in " + Properties.Resources.ApplicationName + "'s .dll.config.",
                    "Version not fond", MessageBoxButtons.OK);

            }
            else if (!oPluginAppSettings.GetBoolean("AutomaticUpdates"))
            {
                MessageBox.Show("You are up to date.", "No Update Available", MessageBoxButtons.OK);
            }
        }

        private string GetCurrentVersionDate()
        {
            string sVersionDate = string.Empty;

            byte[] oBytes = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(oPluginAppSettings.GetString("VersionUrl")))
                {
                    oBytes = new WebClient().DownloadData(oPluginAppSettings.GetString("VersionUrl"));
                }

                if (oBytes != null)
                {
                    string sWebResponse = Encoding.UTF8.GetString(oBytes);

                    //<relative-time datetime="2017-05-17T06:59:23Z">May 17, 2017</relative-time>
                    Match match = Regex.Match(sWebResponse, @"<relative-time datetime=""([^""]*)", RegexOptions.IgnoreCase);

                    // Here we check the Match instance.
                    if (match.Success)
                    {
                        // Finally, we get the Group value and display it.
                        sVersionDate = match.Groups[1].Value;
                    }
                }
            }
            catch
            {
            }

            return (sVersionDate);
        }

        private Boolean GetCurrentVersion(string sSaveLocation)
        {
            Boolean bSuccess = false;

            try
            {
                byte[] oBytes = new WebClient().DownloadData(oPluginAppSettings.GetString("VersionUrl").Replace("/blob/", "/raw/"));

                if (oBytes != null && oBytes.Length > 20000)
                {
                    File.WriteAllBytes(sSaveLocation, oBytes);
                    bSuccess = true;
                }
            }
            catch
            {
            }

            return bSuccess;
        }

        public void Dispose()
        {
        }
    }
}
