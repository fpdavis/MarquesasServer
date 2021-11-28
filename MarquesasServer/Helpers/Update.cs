using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MarquesasServer.Properties;

namespace CommonPluginHelper
{
    public static class PluginUpdate
    {
        public static void Check()
        {
            string sApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
            string sMyVersion = PluginAppSettings.GetString("Version");
            string[] aCurrentVersionData = GetCurrentVersionData();
            
            // The version information in the config file overrides the assembly version.
            if (string.IsNullOrEmpty(sMyVersion)) sMyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (!string.IsNullOrWhiteSpace(aCurrentVersionData[0]) && Version.Parse(aCurrentVersionData[0]) > Version.Parse(sMyVersion))
            {
                if (PluginAppSettings.GetBoolean("AutomaticUpdates") || MessageBox.Show("A newer version of " + sApplicationName + " is available (" + aCurrentVersionData[0] + "). Would you like to update?", "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string sSaveLocation = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
                    if (GetCurrentVersion(aCurrentVersionData, ref sSaveLocation))
                    {
                        try
                        {
                            var oUpdateDirectory = new FileInfo(sSaveLocation).Directory;
                            var oPreviousVersionDirectory = Directory.CreateDirectory(oUpdateDirectory.Parent.FullName + "\\PreviousVersion");

                            foreach (FileInfo oFileInfo in oPreviousVersionDirectory.GetFiles())
                            {
                                oFileInfo.Delete();
                            }

                            try
                            {
                                foreach (FileInfo oFileInfo in oPreviousVersionDirectory.Parent.GetFiles())
                                {
                                    // Move dlls to the PreviousVersion folder
                                    if (oFileInfo.Name.EndsWith(".dll"))
                                    {
                                        File.Move(oFileInfo.FullName,
                                            oFileInfo.FullName.Replace(oPreviousVersionDirectory.Parent.FullName,
                                                oPreviousVersionDirectory.FullName) + ".previousversion");
                                    }
                                }

                                Decompress(sSaveLocation, oPreviousVersionDirectory.Parent.FullName);
                            }
                            catch (Exception oException)
                            {
                                // Rollback
                                foreach (FileInfo oFileInfo in oPreviousVersionDirectory.GetFiles())
                                {
                                    // Move dlls back to the main plugin folder
                                    if (oFileInfo.Name.EndsWith(".previousversion"))
                                    {
                                        File.Move(oFileInfo.FullName,
                                            oFileInfo.FullName.Replace(oPreviousVersionDirectory.FullName,
                                                oPreviousVersionDirectory.Parent.FullName).Replace(".previousversion", string.Empty));
                                    }
                                }

                                throw;
                            }

                            PluginAppSettings.SetString("Version", aCurrentVersionData[0]);
                            PluginAppSettings.Save();

                            if (!PluginAppSettings.GetBoolean("AutomaticUpdates"))
                            {
                                MessageBox.Show(
                                    sApplicationName + " has been updated. Update will be applied on next restart of LaunchBox/BigBox",
                                    "Update Successful", MessageBoxButtons.OK);
                            }
                        }
                        catch (Exception exception)
                        {
                            if (!PluginAppSettings.GetBoolean("AutomaticUpdates"))
                            {
                                MessageBox.Show(
                                    sApplicationName + " could not be updated. Exception: " + exception.Message + Resources.UpdateError,
                                    "Update Unsuccessful", MessageBoxButtons.OK);
                            }
                        }
                    }
                }
            }
            else if (!PluginAppSettings.GetBoolean("AutomaticUpdates") && string.IsNullOrWhiteSpace(aCurrentVersionData[0]))
            {
                MessageBox.Show("A version number could not be found. " + Resources.UpdateError, "Version not fond", MessageBoxButtons.OK);
            }
            else if (!PluginAppSettings.GetBoolean("AutomaticUpdates"))
            {
                MessageBox.Show("You are up to date!", "Up to date", MessageBoxButtons.OK);
            }
        }

        public static void Decompress(string zipFilePath, string extractPath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                string sEntryPath;

                foreach (ZipArchiveEntry oEntry in archive.Entries)
                {
                    if (!oEntry.FullName.Contains(".config"))
                    {
                        sEntryPath = Path.Combine(extractPath, GetRightPartOfPath(oEntry.FullName));
                        Directory.CreateDirectory(Path.GetDirectoryName(sEntryPath));

                        oEntry.ExtractToFile(sEntryPath);
                    }
                }
            }
        }

        private static string GetRightPartOfPath(string path)
        {
            string[] pathParts;

            // use the correct seperator for the environment
            if (path.Contains(Path.DirectorySeparatorChar))
            {
                pathParts = path.Split(Path.DirectorySeparatorChar);
            }
            else
            {
                pathParts = path.Split(Path.AltDirectorySeparatorChar);
            }

            return string.Join(
                Path.DirectorySeparatorChar.ToString(),
                pathParts, 1,
                pathParts.Length - 1);
        }

        private static string[] GetCurrentVersionData()
        {
            string sVersionData = string.Empty;
            string sFileName = string.Empty;

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

                    //<span class="css-truncate-target">1.0.0</span>
                    //Match match = Regex.Match(sWebResponse, @"<span\s*class=""css-truncate-target"">(.*)<\/span>", RegexOptions.IgnoreCase);

                    // /fpdavis/MarquesasServer/releases/download/1.0.0/MarquesasServer.zip
                    // \/fpdavis\/MarquesasServer\/releases\/download\/(.*\/.*)[^""]
                    Match match = Regex.Match(sWebResponse, @"\/fpdavis\/MarquesasServer\/releases\/download\/(.*)\/([^""]*)", RegexOptions.IgnoreCase);

                    // Here we check the Match instance.
                    if (match.Success)
                    {
                        // Finally, we get the Group value representing the version number.
                        sVersionData = match.Groups[1].Value.Trim();
                        sFileName = match.Groups[2].Value.Trim();
                    }
                }
            }
            catch
            {
                // ignored
            }

            return (new[] { sVersionData, sFileName });
        }

        private static Boolean GetCurrentVersion(string[] aVersionData, ref string sSaveLocation)
        {
            Boolean bSuccess = false;
            sSaveLocation += "\\Updates\\" + aVersionData[1].Replace(".", "-" + aVersionData[0] + ".");

            try
            {
                byte[] oBytes = new WebClient().DownloadData("https://github.com/fpdavis/MarquesasServer/releases/download/" + aVersionData[0] + "/" + aVersionData[1]);

                if (oBytes != null && oBytes.Length > 20000)
                {
                    new FileInfo(sSaveLocation).Directory?.Create();

                    File.WriteAllBytes(sSaveLocation, oBytes);
                    bSuccess = true;
                }
            }
            catch
            {
                MessageBox.Show("New version could not be downloaded. " + Resources.UpdateError, "Version not fond", MessageBoxButtons.OK);
            }

            return bSuccess;
        }
    }
}