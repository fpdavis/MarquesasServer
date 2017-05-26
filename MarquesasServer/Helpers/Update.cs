using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonPluginHelper
{
    public static class PluginUpdate
    {
        public static void Check()
        {
            string sApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
            string sMyVersionDate = PluginAppSettings.GetString("VersionDate");
            string sCurrentVersionDate = GetCurrentVersionDate();

            if (!string.IsNullOrWhiteSpace(sCurrentVersionDate) && sMyVersionDate != sCurrentVersionDate)
            {
                if (PluginAppSettings.GetBoolean("AutomaticUpdates") || MessageBox.Show("A newer version of " + sApplicationName + " is available. Would you like to update?", "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string sSaveLocation = Assembly.GetCallingAssembly().Location;
                    if (GetCurrentVersion(sSaveLocation + ".newversion"))
                    {
                        try
                        {
                            File.Delete(sSaveLocation + ".previousversion");
                            File.Move(sSaveLocation, sSaveLocation + ".previousversion");
                            File.Move(sSaveLocation + ".newversion", sSaveLocation);

                            if (!PluginAppSettings.GetBoolean("AutomaticUpdates"))
                            {
                                MessageBox.Show(
                                    sApplicationName + " has been updated. Update will be applied on next restart of LaunchBox/BigBox",
                                    "Update Successful", MessageBoxButtons.OK);
                            }

                            PluginAppSettings.SetString("VersionDate", sCurrentVersionDate);
                            PluginAppSettings.Save();

                        }
                        catch (Exception exception)
                        {
                            if (!PluginAppSettings.GetBoolean("AutomaticUpdates"))
                            {
                                MessageBox.Show(
                                    sApplicationName + " could not be updated. Exception: " + exception.Message,
                                    "Update Unsuccessful", MessageBoxButtons.OK);
                            }
                        }
                    }
                }
            }
            else if (!PluginAppSettings.GetBoolean("AutomaticUpdates") && string.IsNullOrWhiteSpace(sCurrentVersionDate))
            {
                MessageBox.Show(
                    "A version number could not be found. This may indicate a network issue or a configuration issue. Check VersionUrl in " + sApplicationName + "'s .dll.config.",
                    "Version not fond", MessageBoxButtons.OK);

            }
            else if (!PluginAppSettings.GetBoolean("AutomaticUpdates"))
            {
                MessageBox.Show("You are up to date.", "No Update Available", MessageBoxButtons.OK);
            }
        }

        private static string GetCurrentVersionDate()
        {
            string sVersionDate = string.Empty;

            byte[] oBytes = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(PluginAppSettings.GetString("VersionUrl")))
                {
                    oBytes = new WebClient().DownloadData(PluginAppSettings.GetString("VersionUrl"));
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

        private static Boolean GetCurrentVersion(string sSaveLocation)
        {
            Boolean bSuccess = false;

            try
            {
                byte[] oBytes = new WebClient().DownloadData(PluginAppSettings.GetString("VersionUrl").Replace("/blob/", "/raw/"));

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
    }
}
