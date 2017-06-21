using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using SimpleHttp;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using CommonPluginHelper;
using MarquesasServer.Properties;
using System.Diagnostics;

namespace MarquesasServer
{
    public static class MarquesasHttpServerInstance
    {
        public static MarquesasHttpServer RunningServer = new MarquesasHttpServer();
        public static Boolean IsInGame;
    }

    public class MarquesasHttpServer : HttpServer
    {
        private static Hashtable htCachedBinaryPaths = new Hashtable();
        private static Hashtable htCachedBinaries = new Hashtable();

        private static Hashtable htCachedImagePaths = new Hashtable();
        private static Hashtable htCachedImageHTML = new Hashtable();
        
        #region Server Methods
        public override void stopServer()
        {
            base.stopRequested = true;
        }

        public override void handleGETRequest(HttpProcessor oHttpProcessor)
        {
            if (oHttpProcessor.pathParts?.Length > 0)
            {
                switch (oHttpProcessor.pathParts[0])
                {
                    case "statemanager":
                        DoStateManager(oHttpProcessor);
                        break;
                    case "selectedgame":
                        DoSelectedGame(oHttpProcessor);
                        break;
                    case "selectedgames":
                        DoSelectedGames(oHttpProcessor);
                        break;
                    case "manual":
                        DoManual(oHttpProcessor);
                        break;
                    case "image":
                        DoImage(oHttpProcessor);
                        break;
                    case "binary":
                        DoBinary(oHttpProcessor);
                        break;
                    default:
                        DoIndexPage(oHttpProcessor);
                        break;
                }
            }
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            handleGETRequest(p);
        }

        #endregion

        public Tuple<Boolean, Boolean> ArePortsAvailable()
        {
            Boolean bIsHTTPAvailable = true;
            Boolean bIsHTTPSAvailable = true;
            
            var oActiveTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();

            foreach (var oActiveTcpListener in oActiveTcpListeners)
            {
                if (oActiveTcpListener.Port == base.port)
                {
                    bIsHTTPAvailable = false;
                }

                if (oActiveTcpListener.Port == base.secure_port)
                {
                    bIsHTTPSAvailable = false;
                }
            }

            return Tuple.Create(bIsHTTPAvailable, bIsHTTPSAvailable);
        }

        public void WarnIfPortsAreNotAvailable(Boolean bInAdmin = false)
        {
            var tPortsAvailable = ArePortsAvailable();

            if (!tPortsAvailable.Item1 || !tPortsAvailable.Item2)
            {
                string sWhatsNotAvailable = Resources.ApplicationName + " could not bind to either port. Please choose differnt ports";

                if (!tPortsAvailable.Item1 && tPortsAvailable.Item2)
                {
                    sWhatsNotAvailable = sWhatsNotAvailable.Replace("either", "HTTP").Replace("differnt ports", "a different port");
                }
                else if (!tPortsAvailable.Item2 && tPortsAvailable.Item1)
                {
                    sWhatsNotAvailable = sWhatsNotAvailable.Replace("either", "HTTPS (Secure)").Replace("differnt ports", "a different port");
                }

                if (!bInAdmin)
                {
                    sWhatsNotAvailable += " in Tools->" + Resources.ApplicationName + " Admin";
                }

                MessageBox.Show(sWhatsNotAvailable + ".", Resources.ApplicationName + " Port Binding Error", MessageBoxButtons.OK);

            }
        }

        public void FirstTimeRunCheck()
        {
            if ((IsRunning.Item1 || port == -1) &&
                (IsRunning.Item2 || secure_port == -1) &&
                !PluginAppSettings.GetBoolean("NotFirstTimeRun"))
            {
                PluginAppSettings.SetString("NotFirstTimeRun", "True");
                PluginAppSettings.Save();

                if (IsRunning.Item1 || IsRunning.Item2)
                {
                    if (MessageBox.Show(Resources.FirstTimeRun, Resources.ApplicationName + " First Time Run",
                        MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        LaunchIndexPage();
                    }
                }
            }
        }

        public void LaunchIndexPage(Boolean bUseHTTPSProtocal = false)
        {
            if (IsRunning.Item1 && !bUseHTTPSProtocal)
            {
                Process.Start(
                    "http://" + string.Join(".", MarquesasHttpServerInstance.RunningServer.localIPv4Addresses[0]) +
                    (MarquesasHttpServerInstance.RunningServer.port != 80
                        ? ":" + MarquesasHttpServerInstance.RunningServer.port
                        : ""));
            }
            else if (IsRunning.Item2)
            {
                Process.Start("https://" + string.Join(".", MarquesasHttpServerInstance.RunningServer.localIPv4Addresses[0]) + (MarquesasHttpServerInstance.RunningServer.secure_port != 443 ? ":" + MarquesasHttpServerInstance.RunningServer.port : ""));
            }
        }

        public void DoIndexPage(HttpProcessor oHttpProcessor)
        {
            StringBuilder sbIndexPage = new StringBuilder(Resources.HTML_Index);
            sbIndexPage.Replace("<!-- ApplicationName -->", Resources.ApplicationName);

            StringBuilder sbTempHTML = new StringBuilder();

            #region Image Pages
            PropertyInfo[] oProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IGame).GetProperties();

            sbTempHTML.AppendLine("<li/><a target='_blank' href='/manual'>Manual</a>");

            foreach (PropertyInfo oProperty in oProperties)
            {
                if (oProperty.Name.Contains("ImagePath"))
                {
                    sbTempHTML.AppendLine("<li/><a target='_blank' href='/image/" + oProperty.Name.Replace("ImagePath", "").ToLower() + "'>Image/" + oProperty.Name.Replace("ImagePath", "") + "</a>");
                }
            }

            sbIndexPage.Replace("<!-- Auto Refreshing Web Pages -->", sbTempHTML.ToString());

            #endregion

            #region Binary Requests

            oProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IGame).GetProperties();
            sbTempHTML.Clear();

            foreach (PropertyInfo oProperty in oProperties)
            {
                if (oProperty.Name.Contains("Path"))
                {
                    sbTempHTML.AppendLine("<li/><a target='_blank' href='/binary/" + oProperty.Name.Replace("Path", "").ToLower() + "'>Binary/" + oProperty.Name.Replace("Path", "") + "</a>");
                }
            }

            sbIndexPage.Replace("<!-- Binary Requests -->", sbTempHTML.ToString());

            #endregion

            #region State Manager

            sbTempHTML.Clear();
            sbTempHTML.AppendLine("<li/><a target='_blank' href='/statemanager'>StateManager</a>");
            sbTempHTML.AppendLine("<li/><a target='_blank' href='/statemanager/isingame'>StateManager/IsInGame</a>");

            oProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IStateManager).GetProperties();

            foreach (PropertyInfo oProperty in oProperties)
            {
                sbTempHTML.AppendLine("<li/><a target='_blank' href='/statemanager/" + oProperty.Name.ToLower() + "'>StateManager/" + oProperty.Name + "</a>");
            }

            sbIndexPage.Replace("<!-- State Manager -->", sbTempHTML.ToString());

            #endregion

            #region Selected Game Information
            sbTempHTML.Clear();

            sbTempHTML.AppendLine("<li/><a target='_blank' href='/selectedgame'>SelectedGame</a>");

            oProperties = typeof(IGame).GetProperties();

            foreach (PropertyInfo oProperty in oProperties)
            {
                sbTempHTML.AppendLine("<li/><a target='_blank' href='/selectedgame/" + oProperty.Name.ToLower() + "'>SelectedGame/" + oProperty.Name + "</a>");
            }

            sbIndexPage.Replace("<!-- Selected Game Information -->", sbTempHTML.ToString());

            #endregion

            #region Selected Game(s) Information
            sbTempHTML.Clear();
            sbTempHTML.AppendLine("<li/><a target='_blank' href='/selectedgames'>SelectedGames</a>");

            oProperties = typeof(IGame).GetProperties();

            foreach (PropertyInfo oProperty in oProperties)
            {
                sbTempHTML.AppendLine("<li/><a target='_blank' href='/selectedgames/" + oProperty.Name.ToLower() + "'>SelectedGames/" + oProperty.Name + "</a>");
            }

            sbIndexPage.Replace("<!-- Selected Game(s) Information -->", sbTempHTML.ToString());

            #endregion

            List<KeyValuePair<string, string>> additionalHeaders =
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Etag", oHttpProcessor.request_url + "/" + ComputeHash(sbIndexPage))
                };

            oHttpProcessor.writeSuccess("text/html", sbIndexPage.ToString().Length, "200 OK", additionalHeaders);

            oHttpProcessor.outputStream.Write(sbIndexPage);
        }

        #region Web APIs
        private static void WriteJSON(HttpProcessor oHttpProcessor, string sJSONResponse)
        {
            oHttpProcessor.writeSuccess("application/json", sJSONResponse.Length);
            oHttpProcessor.outputStream.WriteLine(sJSONResponse);
        }

        private void DoStateManager(HttpProcessor oHttpProcessor)
        {
            string sJSONResponse = string.Empty;

            if (oHttpProcessor.pathParts.Length == 2)
            {
                switch (oHttpProcessor.pathParts[1])
                {
                    case "isingame":
                        sJSONResponse = new JavaScriptSerializer().Serialize(MarquesasHttpServerInstance.IsInGame);
                        break;
                    default:
                        PropertyInfo[] oProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IStateManager).GetProperties();
                        // A linq statement would look totally sexy here
                        foreach (PropertyInfo oProperty in oProperties)
                        {
                            if (oProperty.Name.ToLower() == oHttpProcessor.pathParts[1])
                            {
                                sJSONResponse =
                                    new JavaScriptSerializer().Serialize(oProperty.GetValue(PluginHelper.StateManager));
                            }
                        }
                        break;
                }
            }
            else
            {
                sJSONResponse = new JavaScriptSerializer().Serialize(PluginHelper.StateManager);
            }

            WriteJSON(oHttpProcessor, sJSONResponse);
        }

        private void DoSelectedGame(HttpProcessor oHttpProcessor)
        {
            string sJSONResponse = string.Empty;

            if (PluginHelper.StateManager.GetAllSelectedGames().Length == 1)
            {
                if (oHttpProcessor.pathParts.Length == 1)
                {
                    sJSONResponse =
                        new JavaScriptSerializer().Serialize(PluginHelper.StateManager.GetAllSelectedGames());
                }
                else if (oHttpProcessor.pathParts.Length == 2)
                {
                    PropertyInfo[] oProperties = typeof(IGame).GetProperties();

                    // A linq statement would look totally sexy here
                    foreach (PropertyInfo oProperty in oProperties)
                    {
                        if (oProperty.Name.ToLower() == oHttpProcessor.pathParts[1].ToLower())
                        {
                            sJSONResponse =
                                new JavaScriptSerializer().Serialize(oProperty.GetValue(PluginHelper.StateManager.GetAllSelectedGames()[0]));
                        }
                    }
                }
            }

            WriteJSON(oHttpProcessor, sJSONResponse);
        }

        private void DoSelectedGames(HttpProcessor oHttpProcessor)
        {
            string sJSONResponse = string.Empty;
            
            if (oHttpProcessor.pathParts.Length == 1)
            {
                sJSONResponse = new JavaScriptSerializer().Serialize(PluginHelper.StateManager.GetAllSelectedGames());
            }
            else if (oHttpProcessor.pathParts.Length == 2)
            {
                PropertyInfo[] oProperties = typeof(IGame).GetProperties();
                Hashtable oHashtable = new Hashtable();

                // A linq statement would look totally sexy here
                foreach (PropertyInfo oProperty in oProperties)
                {
                    if (oProperty.Name.ToLower() == oHttpProcessor.pathParts[1].ToLower())
                    {
                        foreach (var oGame in PluginHelper.StateManager.GetAllSelectedGames())
                        {
                            oHashtable.Add(oGame.Id, oProperty.GetValue(oGame));
                        }
                    }
                }

                sJSONResponse = new JavaScriptSerializer().Serialize(oHashtable);
            }

            WriteJSON(oHttpProcessor, sJSONResponse);
        }
        #endregion

        #region Image Page Methods
        private void DoImage(HttpProcessor oHttpProcessor)
        {
            if (DoCacheResponseIfWarranted(oHttpProcessor) || !OneAndOnlyOneGameSelected(oHttpProcessor)) return;

            LoadBinary(oHttpProcessor);

            string sImagePath = htCachedBinaryPaths[oHttpProcessor.pathParts[1].ToLower()]?.ToString();

            if (!string.IsNullOrWhiteSpace(sImagePath)
                && (htCachedImagePaths[oHttpProcessor.pathParts[1].ToLower()]?.ToString() != sImagePath)
                && File.Exists(sImagePath))
            {
                htCachedImagePaths[oHttpProcessor.pathParts[1]] = sImagePath;

                htCachedImageHTML[oHttpProcessor.pathParts[1]] =
                    Properties.Resources.HTML_Marque
                    .Replace("<!-- SecondsBetweenRefresh -->", PluginAppSettings.GetString("SecondsBetweenRefresh"))
                    .Replace("<!-- Base64Image -->", MakeBinarySrcData(sImagePath, (byte[])htCachedBinaries[oHttpProcessor.pathParts[1]]));
            }
            else
            {
                htCachedImageHTML[oHttpProcessor.pathParts[1]] = Properties.Resources.HTML_NoResource
                        .Replace("<!-- SecondsBetweenRefresh -->", (PluginAppSettings.GetInt("SecondsBetweenRefresh") * 1000).ToString())
                        .Replace("<!-- Path -->", sImagePath)
                        .Replace("<!-- Title -->", PluginHelper.StateManager.GetAllSelectedGames()[0].Title)
                    ;

                htCachedImagePaths[oHttpProcessor.pathParts[1]] = "";
            }

            WriteImageHTML(oHttpProcessor, htCachedImageHTML[oHttpProcessor.pathParts[1]]?.ToString());
        }

        string MakeBinarySrcData(string sImagePath, byte[] oBianary, bool bIsPdf = false)
        {
            if (!bIsPdf)
            {
                return @"data:image/" + Path.GetExtension(sImagePath).Substring(1) + ";base64," + Convert.ToBase64String(oBianary);
            }
            else
            {
                return @"data:application/pdf;base64," + Convert.ToBase64String(oBianary);
            }
        }

        private static void WriteImageHTML(HttpProcessor oHttpProcessor, string sHTMLResponse)
        {
            if (!string.IsNullOrWhiteSpace(sHTMLResponse))
            {
                List<KeyValuePair<string, string>> additionalHeaders = null;

                if (PluginHelper.StateManager.GetAllSelectedGames().Length > 0)
                {
                    additionalHeaders = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Etag",
                            oHttpProcessor.request_url + "/" + PluginHelper.StateManager.GetAllSelectedGames()[0].Id)
                    };
                }

                oHttpProcessor.writeSuccess("text/html", sHTMLResponse.Length, "200 OK", additionalHeaders);

                oHttpProcessor.outputStream.WriteLine(sHTMLResponse);
            }
            else
            {
                oHttpProcessor.writeFailure();
            }
        }

        #endregion

        #region Binary

        private void DoBinary(HttpProcessor oHttpProcessor)
        {
            if (DoCacheResponseIfWarranted(oHttpProcessor)) return;

            LoadBinary(oHttpProcessor);
            WriteBinary(oHttpProcessor);
        }

        private void LoadBinary(HttpProcessor oHttpProcessor)
        {
            if (oHttpProcessor.pathParts != null && PluginHelper.StateManager.GetAllSelectedGames().Length == 1)
            {
                string sType;
                if (oHttpProcessor.pathParts.Length > 1)
                {
                    sType = oHttpProcessor.pathParts[1].ToLower();
                }
                else
                {
                    sType = oHttpProcessor.pathParts[0].ToLower();
                }

                string sBinaryPath = null;

                PropertyInfo[] oProperties = typeof(IGame).GetProperties();

                // A linq statement would look totally sexy here
                foreach (PropertyInfo oProperty in oProperties)
                {
                    if (oProperty.Name.ToLower() == sType + "path" || oProperty.Name.ToLower() == sType + "imagepath")
                    {
                     sBinaryPath = oProperty.GetValue(PluginHelper.StateManager.GetAllSelectedGames()[0])?.ToString();
                     break;
                    }
                }
                
                if (!string.IsNullOrWhiteSpace(sBinaryPath)
                    && (htCachedBinaryPaths[sType]?.ToString() != sBinaryPath))
                {
                    if (!File.Exists(sBinaryPath))
                    {
                        sBinaryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + sBinaryPath;
                    }

                    if (File.Exists(sBinaryPath))
                    {
                        htCachedBinaryPaths[sType] = sBinaryPath;
                        htCachedBinaries[sType] = File.ReadAllBytes(sBinaryPath);
                    }
                    else
                    {
                        htCachedBinaryPaths[sType] = "";
                        htCachedBinaries[sType] = "";
                    }
                }
                else if (string.IsNullOrWhiteSpace(sBinaryPath))
                {
                    htCachedBinaryPaths[sType] = "";
                    htCachedBinaries[sType] = "";
                }
            }
        }

        private static void WriteBinary(HttpProcessor oHttpProcessor)
        {

            if (!string.IsNullOrWhiteSpace(htCachedBinaries[oHttpProcessor.pathParts[1]].ToString()))
            {
                List<KeyValuePair<string, string>> additionalHeaders =
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Etag", oHttpProcessor.request_url + "/" + PluginHelper.StateManager.GetAllSelectedGames()[0].Id),
                        new KeyValuePair<string, string>("Content-disposition", "inline; filename=\"" + Path.GetFileName(PluginHelper.StateManager.GetAllSelectedGames()[0].GetManualPath()) + "\""),
                        new KeyValuePair<string, string>("Content-Transfer-Encoding", "binary"),
                        new KeyValuePair<string, string>("Accept-Ranges", "none")
                    };

                string sExtension = Path.GetExtension(htCachedBinaryPaths[oHttpProcessor.pathParts[1]].ToString());
                if (sExtension.Length > 1)
                {
                    sExtension = sExtension.Substring(1);
                }

                oHttpProcessor.writeSuccess("application/" + sExtension, ((byte[])htCachedBinaries[oHttpProcessor.pathParts[1]]).Length, "200 OK", additionalHeaders);

                oHttpProcessor.rawOutputStream.Write((byte[])htCachedBinaries[oHttpProcessor.pathParts[1]], 0, ((byte[])htCachedBinaries[oHttpProcessor.pathParts[1]]).Length);
            }
            else
            {
                oHttpProcessor.writeFailure();
            }
        }

        #endregion

        private void DoManual(HttpProcessor oHttpProcessor)
        {
            if (DoCacheResponseIfWarranted(oHttpProcessor) || !OneAndOnlyOneGameSelected(oHttpProcessor)) return;

            LoadBinary(oHttpProcessor);

            string sManualPath = htCachedBinaryPaths["manual"]?.ToString();

            if (!string.IsNullOrWhiteSpace(sManualPath)
                && (htCachedImagePaths["manual"]?.ToString() != sManualPath)
                && File.Exists(sManualPath))
            {
                htCachedImagePaths["manual"] = sManualPath;

                htCachedImageHTML["manual"] =
                    Properties.Resources.HTML_Manual
                    .Replace("<!-- SecondsBetweenRefresh -->", (PluginAppSettings.GetInt("SecondsBetweenRefresh") * 1000).ToString())
                    .Replace("<!-- BinaryPath -->", "/binary/manual")
                    ;
            }
            else
            {
                htCachedImageHTML["manual"] = Properties.Resources.HTML_NoResource
                        .Replace("<!-- SecondsBetweenRefresh -->", (PluginAppSettings.GetInt("SecondsBetweenRefresh")).ToString())
                        .Replace("<!-- Path -->", sManualPath)
                        .Replace("<!-- Title -->", PluginHelper.StateManager.GetAllSelectedGames()[0].Title)
                    ;

                htCachedImagePaths["manual"] = "";
            }

            WriteImageHTML(oHttpProcessor, htCachedImageHTML["manual"].ToString());
        }

        private static bool DoCacheResponseIfWarranted(HttpProcessor oHttpProcessor)
        {
            // Cache the response in the browser using the Etag and If-None-Match
            if (oHttpProcessor.pathParts != null && oHttpProcessor.httpHeaders.ContainsKey("if-none-match")
                && PluginHelper.StateManager.GetAllSelectedGames().Length == 1
                && oHttpProcessor.httpHeaders["if-none-match"] == oHttpProcessor.request_url + "/" +
                PluginHelper.StateManager.GetAllSelectedGames()[0].Id)
            {
                oHttpProcessor.writeNotModified();
                return true;
            }
            
            return false;
        }

        private static bool OneAndOnlyOneGameSelected(HttpProcessor oHttpProcessor)
        {
            if (PluginHelper.StateManager.GetAllSelectedGames().Length == 1)
            {
                return true;
            }
            else
            {
                WriteImageHTML(oHttpProcessor, Properties.Resources.HTML_NoGame.Replace("<!-- SecondsBetweenRefresh -->", (PluginAppSettings.GetInt("SecondsBetweenRefresh")).ToString()));
                return false;
            }
        }

        private static byte[] ComputeHash(StringBuilder sbDefaultPage)
        {
            return new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.ASCII.GetBytes(sbDefaultPage.ToString()));
        }
 
   }
}
