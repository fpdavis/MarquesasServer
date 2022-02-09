using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using SimpleHttp;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using CommonPluginHelper;
using MarquesasServer.Properties;
using System.Diagnostics;
using System.Linq;

namespace MarquesasServer
{
    public static class MarquesasHttpServerInstance
    {
        public static MarquesasHttpServer RunningServer = new MarquesasHttpServer();
        public static Boolean IsInGame;        
    }

    public class GameInfo
    {
        public string Title { get; set; }
        public string Publisher { get; set; }
        public string Platform { get; set; }
        public string Id { get; set; }
        public int? LaunchBoxDbId { get; set; }
    }

    public class MarquesasHttpServer : HttpServer
    {
        private static Hashtable htCachedBinaryPaths = new Hashtable();
        private static Hashtable htCachedBinaries = new Hashtable();

        private static Hashtable htCachedImagePaths = new Hashtable();
        private static Hashtable htCachedImageHTML = new Hashtable();

        private static string gsEtagSeperator = ":";
        private static int giGetAllGamesLimit;

        #region Server Methods

        public override void stopServer()
        {
            base.stopRequested = true;
        }

        public override void handleGETRequest(HttpProcessor oHttpProcessor)
        {
            if (oHttpProcessor.pathParts?.Length > 0)
            {
                switch (oHttpProcessor.pathParts[0].ToLower())
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

                    case "selectedgamemethods":
                        SelectedGameMethods(oHttpProcessor);
                        break;

                    case "playgame":
                        PlayGame(oHttpProcessor);
                        break;

                    case "getallgames":
                        GetAllGames(oHttpProcessor);
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

        #endregion Server Methods

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
                string sWhatsNotAvailable = Resources.ApplicationName +
                                            " could not bind to either port. Please choose differnt ports";

                if (!tPortsAvailable.Item1 && tPortsAvailable.Item2)
                {
                    sWhatsNotAvailable = sWhatsNotAvailable.Replace("either", "HTTP")
                        .Replace("differnt ports", "a different port");
                }
                else if (!tPortsAvailable.Item2 && tPortsAvailable.Item1)
                {
                    sWhatsNotAvailable = sWhatsNotAvailable.Replace("either", "HTTPS (Secure)")
                        .Replace("differnt ports", "a different port");
                }

                if (!bInAdmin)
                {
                    sWhatsNotAvailable += " in Tools->" + Resources.ApplicationName + " Admin";
                }

                MessageBox.Show(sWhatsNotAvailable + ".", Resources.ApplicationName + " Port Binding Error",
                    MessageBoxButtons.OK);
            }
        }
        new public void Initialize()
        {
            if (!int.TryParse(PluginAppSettings.GetString("GetAllGamesLimit"), out giGetAllGamesLimit))
            {
                giGetAllGamesLimit = 50;
            }

            base.Initialize();
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
                Process.Start(new ProcessStartInfo(
                    "http://" + string.Join(".", MarquesasHttpServerInstance.RunningServer.localIPv4Addresses[0]) +
                     (MarquesasHttpServerInstance.RunningServer.port != 80
                        ? ":" + MarquesasHttpServerInstance.RunningServer.port
                        : "")
                        ) { UseShellExecute = true }
                        );
            }
            else if (IsRunning.Item2)
            {
                Process.Start(new ProcessStartInfo(
                    "https://" + string.Join(".", MarquesasHttpServerInstance.RunningServer.localIPv4Addresses[0]) +
                     (MarquesasHttpServerInstance.RunningServer.secure_port != 443
                                  ? ":" + MarquesasHttpServerInstance.RunningServer.port
                                  : "")
                                  ) { UseShellExecute = true }
                        );
            }
        }

        public void DoIndexPage(HttpProcessor oHttpProcessor)
        {
            if (DoCacheResponseIfWarranted(oHttpProcessor)) return;

            StringBuilder sbIndexPage = new StringBuilder(Resources.HTML_Index);
            sbIndexPage.Replace("<!-- ApplicationName -->", Resources.ApplicationName);

            StringBuilder sbTempHTML = new StringBuilder();

            #region Image Pages

            PropertyInfo[] oGameProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IGame).GetProperties();

            sbTempHTML.AppendLine("<li/><a target='_blank' href='/manual'>Manual</a>");

            foreach (PropertyInfo oProperty in oGameProperties)
            {
                if (oProperty.Name.Contains("ImagePath"))
                {
                    sbTempHTML.AppendLine("<li/><a target='_blank' href='/image/" +
                                          oProperty.Name.Replace("ImagePath", "").ToLower() + "'>Image/" +
                                          oProperty.Name.Replace("ImagePath", "") + "</a>");
                }
            }

            sbIndexPage.Replace("<!-- Auto Refreshing Web Pages -->", sbTempHTML.ToString());

            #endregion Image Pages

            #region Binary Requests

            //oGameProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IGame).GetProperties();
            sbTempHTML.Clear();

            foreach (PropertyInfo oProperty in oGameProperties)
            {
                if (oProperty.Name.Contains("Path"))
                {
                    sbTempHTML.AppendLine("<li/><a target='_blank' href='/binary/" +
                                          oProperty.Name.Replace("Path", "").ToLower() + "'>Binary/" +
                                          oProperty.Name.Replace("Path", "") + "</a>");
                }
            }

            sbIndexPage.Replace("<!-- Binary Requests -->", sbTempHTML.ToString());

            #endregion Binary Requests

            #region State Manager

            sbTempHTML.Clear();
            sbTempHTML.AppendLine("<li/><a target='_blank' href='/statemanager'>StateManager</a>");
            sbTempHTML.AppendLine("<li/><a target='_blank' href='/statemanager/isingame'>StateManager/IsInGame</a>");

            oGameProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IStateManager).GetProperties();

            foreach (PropertyInfo oProperty in oGameProperties)
            {
                sbTempHTML.AppendLine("<li/><a target='_blank' href='/statemanager/" + oProperty.Name.ToLower() +
                                      "'>StateManager/" + oProperty.Name + "</a>");
            }

            sbIndexPage.Replace("<!-- State Manager -->", sbTempHTML.ToString());

            #endregion State Manager

            #region Selected Game Information

            sbTempHTML.Clear();

            sbTempHTML.AppendLine("<li/><a target='_blank' href='/selectedgame'>SelectedGame</a>");

            oGameProperties = typeof(IGame).GetProperties();
            Array.Sort(oGameProperties, new IGamePropertiesComparer());

            foreach (PropertyInfo oProperty in oGameProperties)
            {
                sbTempHTML.AppendLine("<li/><a target='_blank' href='/selectedgame/" + oProperty.Name.ToLower() +
                                      "'>SelectedGame/" + oProperty.Name + "</a>");
            }

            sbIndexPage.Replace("<!-- Selected Game Information -->", sbTempHTML.ToString());

            #endregion Selected Game Information

            #region Selected Game(s) Information

            sbTempHTML.Clear();
            sbTempHTML.AppendLine("<li/><a target='_blank' href='/selectedgames'>SelectedGames</a>");

            oGameProperties = typeof(IGame).GetProperties();
            Array.Sort(oGameProperties, new IGamePropertiesComparer());

            foreach (PropertyInfo oProperty in oGameProperties)
            {
                sbTempHTML.AppendLine("<li/><a target='_blank' href='/selectedgames/" + oProperty.Name.ToLower() +
                                      "'>SelectedGames/" + oProperty.Name + "</a>");
            }

            sbIndexPage.Replace("<!-- Selected Game(s) Information -->", sbTempHTML.ToString());

            #endregion Selected Game(s) Information

            #region Selected Game Methods

            sbTempHTML.Clear();
                        
            MethodInfo[] oGameMethods = typeof(IGame).GetMethods();
            Array.Sort(oGameMethods, new IGameMethodComparer());

            string sParameters = string.Empty;
            foreach (var oMethod in oGameMethods)
            {
                if (oMethod.GetParameters().Length > 0)
                {
                    sParameters = "/";
                    foreach (var oParameter in oMethod.GetParameters())
                    {                        
                        sParameters += $"{oParameter.ParameterType.ToString().Replace("System.", "")} {{{oParameter.Name}";

                        if (oParameter.IsOptional)
                        {
                            sParameters += " optional";
                        }
                        if (oParameter.HasDefaultValue)
                        {
                            sParameters += $" default={oParameter.DefaultValue}";
                        }

                        sParameters += "}/";
                    }

                    sParameters = sParameters.TrimEnd('/');

                    if (oMethod.GetParameters().Length > 1)
                    {
                        sParameters += "<span style='color: red'> - Not Supported</span>";
                    }
                }
                else
                {
                    sParameters = string.Empty;
                }

                if (oMethod.Name.ToLower().StartsWith("get") && oMethod.GetParameters().Length <= 1)
                {
                    sbTempHTML.Append(" <li/><a target='_blank' href='/selectedgamemethods/" + oMethod.Name.ToLower() +
                           "'>SelectedGameMethods/" + oMethod.Name + sParameters + "</a>");
                }
                else
                {
                    sbTempHTML.Append(" <li/>" + "SelectedGameMethods/" + oMethod.Name + sParameters);
                }

            }

            sbIndexPage.Replace("<!-- Selected Game Methods -->", sbTempHTML.ToString());

            #endregion Selected Game Methods

            List<KeyValuePair<string, string>> additionalHeaders =
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Etag",
                        oHttpProcessor.request_url + gsEtagSeperator + ComputeHash(sbIndexPage.ToString()))
                };

            oHttpProcessor.writeSuccess("text/html", sbIndexPage.ToString().Length, "200 OK", additionalHeaders);

            oHttpProcessor.outputStream.Write(sbIndexPage);
        }
        
        class IGamePropertiesComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return (new CaseInsensitiveComparer()).Compare(((PropertyInfo)x).Name, ((PropertyInfo)y).Name);
            }
        }
        class IGameMethodComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return (new CaseInsensitiveComparer()).Compare(((MethodInfo)x).Name, ((MethodInfo)y).Name);
            }
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
                        sJSONResponse = JsonSerializer.Serialize(MarquesasHttpServerInstance.IsInGame);
                        break;

                    default:
                        PropertyInfo[] oProperties =
                            typeof(Unbroken.LaunchBox.Plugins.Data.IStateManager).GetProperties();
                        // A linq statement would look totally sexy here
                        foreach (PropertyInfo oProperty in oProperties)
                        {
                            if (oProperty.Name.ToLower() == oHttpProcessor.pathParts[1])
                            {
                                sJSONResponse = JsonSerializer.Serialize(oProperty.GetValue(PluginHelper.StateManager));
                            }
                        }
                        break;
                }
            }
            else
            {
                sJSONResponse = JsonSerializer.Serialize(PluginHelper.StateManager);
            }

            WriteJSON(oHttpProcessor, sJSONResponse);
        }

        private void DoSelectedGame(HttpProcessor oHttpProcessor)
        {
            string sJSONResponse = string.Empty;

            if (PluginHelper.StateManager.GetAllSelectedGames()?.Length == 1)
            {
                if (oHttpProcessor.pathParts.Length == 1)
                {
                    sJSONResponse = JsonSerializer.Serialize(PluginHelper.StateManager.GetAllSelectedGames());
                }
                else if (oHttpProcessor.pathParts.Length == 2)
                {
                    PropertyInfo[] oProperties = typeof(IGame).GetProperties();

                    // A linq statement would look totally sexy here
                    foreach (PropertyInfo oProperty in oProperties)
                    {
                        if (oProperty.Name.ToLower() == oHttpProcessor.pathParts[1].ToLower())
                        {
                            sJSONResponse = JsonSerializer.Serialize(oProperty.GetValue(PluginHelper.StateManager.GetAllSelectedGames()[0]));
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
                sJSONResponse = JsonSerializer.Serialize(PluginHelper.StateManager.GetAllSelectedGames());
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

                sJSONResponse = JsonSerializer.Serialize(oHashtable);
            }

            WriteJSON(oHttpProcessor, sJSONResponse);
        }

        private void PlayGame(HttpProcessor oHttpProcessor)
        {
            if (MarquesasHttpServerInstance.IsInGame)
            {
                oHttpProcessor.writeFailure("503 Service Unavailable", "A game is currently being played. Try again once the game has been closed.");
                return;
            }

            IGame oGame = null;

            if (oHttpProcessor.pathParts.Length == 2)
            {
                // Go by Title
                oGame = PluginHelper.DataManager.GetAllGames().FirstOrDefault(x => x.Title.ToLower() == oHttpProcessor.pathParts[1].ToLower());
            }
            else if (oHttpProcessor.pathParts.Length > 2)
            {
                switch (oHttpProcessor.pathParts[1].ToLower())
                {
                    case "title":
                        oGame = PluginHelper.DataManager.GetAllGames().FirstOrDefault(x => x.Title.ToLower() == oHttpProcessor.pathParts[2].ToLower());
                        break;
                    case "publisherandtitle":
                        if (oHttpProcessor.pathParts.Length == 3)
                        {
                            oGame = PluginHelper.DataManager.GetAllGames().FirstOrDefault(x => x.Platform == oHttpProcessor.pathParts[2].ToLower() && x.Title.ToLower() == oHttpProcessor.pathParts[3].ToLower());
                        }
                        break;
                    case "id":
                        oGame = PluginHelper.DataManager.GetAllGames().FirstOrDefault(x => x.Id == oHttpProcessor.pathParts[2]);
                        break;
                    case "launchboxdbid":
                        if (int.TryParse(oHttpProcessor.pathParts[2], out int iId))
                        {
                            oGame = PluginHelper.DataManager.GetAllGames().FirstOrDefault(x => x.LaunchBoxDbId == iId);
                        }
                        break;
                    default:
                        oGame = PluginHelper.DataManager.GetAllGames().FirstOrDefault(x => x.Title.ToLower() == oHttpProcessor.pathParts[2].ToLower());
                        break;
                }
            }

            if (oGame != null)
            {
                oGame.Play();

                string sJSONResponse = JsonSerializer.Serialize(oGame);
                WriteJSON(oHttpProcessor, sJSONResponse);
            }
            else
            {
                oHttpProcessor.writeFailure("404 Not Found", "No game match could be found. Valid lookups include Title, PublisherAndTitle, Id, and LaunchBoxDbId. Examples: /PlayGame/Title/EXACT_TITLE, /PlayGame/PublisherAndTitle/EXACT_PUBLISHERNAME/EXACT_TITLE, /PlayGame/LaunchBoxDbId/123456");
            }
        }

        private void GetAllGames(HttpProcessor oHttpProcessor)
        {
            string sJSONResponse = string.Empty;

            if (oHttpProcessor.pathParts.Length == 2 && oHttpProcessor.pathParts[1].ToLower() == "allproperties")
            {
                var oaGames = PluginHelper.DataManager.GetAllGames()
                    .Where(x => x.Title.IndexOf(oHttpProcessor.GetQSParam("title"), StringComparison.CurrentCultureIgnoreCase) != -1
                         && x.Publisher.IndexOf(oHttpProcessor.GetQSParam("publisher"), StringComparison.CurrentCultureIgnoreCase) != -1
                         && x.Platform.IndexOf(oHttpProcessor.GetQSParam("platform"), StringComparison.CurrentCultureIgnoreCase) != -1
                        ).Take(giGetAllGamesLimit);                

                sJSONResponse = JsonSerializer.Serialize(oaGames);
            }            
            else
            {
                var oaGames = PluginHelper.DataManager.GetAllGames()
                    .Where(x => x.Title.IndexOf(oHttpProcessor.GetQSParam("title"), StringComparison.CurrentCultureIgnoreCase) != -1
                         && x.Publisher.IndexOf(oHttpProcessor.GetQSParam("publisher"), StringComparison.CurrentCultureIgnoreCase) != -1
                         && x.Platform.IndexOf(oHttpProcessor.GetQSParam("platform"), StringComparison.CurrentCultureIgnoreCase) != -1
                         )
                         .Select
                         (g => new GameInfo
                         {
                             Title = g.Title,
                             Publisher = g.Publisher,
                             Platform = g.Platform,
                             Id = g.Id,
                             LaunchBoxDbId = g.LaunchBoxDbId
                         });

                sJSONResponse = JsonSerializer.Serialize(oaGames);
            }

            WriteJSON(oHttpProcessor, sJSONResponse);
        }
        #endregion Web APIs

        #region Image Page Methods

        private void DoImage(HttpProcessor oHttpProcessor)
        {
            if (DoCacheResponseIfWarranted(oHttpProcessor)) return;

            LoadBinary(oHttpProcessor);

            string sImagePath = htCachedBinaryPaths[oHttpProcessor.pathParts[1].ToLower()]?.ToString();

            if (string.IsNullOrWhiteSpace(sImagePath) || !File.Exists(sImagePath))
            {
                htCachedImageHTML[oHttpProcessor.pathParts[1]] = Properties.Resources.HTML_NoResource
                        .Replace("<!-- SecondsBetweenRefresh -->",
                            (PluginAppSettings.GetInt("SecondsBetweenRefresh")).ToString())
                        .Replace("<!-- Path -->", sImagePath)
                        .Replace("<!-- Title -->", PluginHelper.StateManager.GetAllSelectedGames()[0].Title)
                    ;

                htCachedImagePaths[oHttpProcessor.pathParts[1]] = "";
            }
            else if (htCachedImagePaths[oHttpProcessor.pathParts[1].ToLower()]?.ToString() != sImagePath)
            {
                htCachedImagePaths[oHttpProcessor.pathParts[1]] = sImagePath;

                htCachedImageHTML[oHttpProcessor.pathParts[1]] =
                    Properties.Resources.HTML_Marque
                        .Replace("<!-- SecondsBetweenRefresh -->", PluginAppSettings.GetString("SecondsBetweenRefresh"))
                        .Replace("<!-- Base64Image -->",
                            MakeBinarySrcData(sImagePath, (byte[])htCachedBinaries[oHttpProcessor.pathParts[1]]));
            }

            WriteImageHTML(oHttpProcessor, htCachedImageHTML[oHttpProcessor.pathParts[1]]?.ToString());
        }

        private string MakeBinarySrcData(string sImagePath, byte[] oBianary, bool bIsPdf = false)
        {
            if (!bIsPdf)
            {
                return @"data:image/" + Path.GetExtension(sImagePath).Substring(1) + ";base64," +
                       Convert.ToBase64String(oBianary);
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

                if (PluginHelper.StateManager.GetAllSelectedGames()?.Length > 0)
                {
                    additionalHeaders = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Etag",
                            oHttpProcessor.request_url + gsEtagSeperator + PluginHelper.StateManager.GetAllSelectedGames()[0].Id)
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

        #endregion Image Page Methods

        #region Binary

        private void DoBinary(HttpProcessor oHttpProcessor)
        {
            if (DoCacheResponseIfWarranted(oHttpProcessor)) return;

            LoadBinary(oHttpProcessor);
            WriteBinary(oHttpProcessor);
        }

        private void LoadBinary(HttpProcessor oHttpProcessor)
        {
            if (oHttpProcessor.pathParts != null)
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

                if (PluginHelper.StateManager.GetAllSelectedGames()?.Length == 1)
                {
                    PropertyInfo[] oProperties = typeof(IGame).GetProperties();

                    // A linq statement would look totally sexy here
                    foreach (PropertyInfo oProperty in oProperties)
                    {
                        if (oProperty.Name.ToLower() == sType + "path" || oProperty.Name.ToLower() == sType + "imagepath")
                        {
                            sBinaryPath = oProperty.GetValue(PluginHelper.StateManager.GetAllSelectedGames()[0])
                                ?.ToString();
                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(sBinaryPath))
                    {
                        MethodInfo[] oGameMethods = typeof(IGame).GetMethods();
                        foreach (MethodInfo oMethod in oGameMethods)
                        {
                            if (oMethod.Name.ToLower() == "get" + sType + "path")
                            {
                                if (oMethod.GetParameters().Length == 0)
                                {
                                    sBinaryPath = oMethod.Invoke(PluginHelper.StateManager.GetAllSelectedGames()[0], new object[] { })
                                        ?.ToString();
                                }
                                else
                                {
                                    sBinaryPath = oMethod.Invoke(PluginHelper.StateManager.GetAllSelectedGames()[0], new object[] { false })
                                        ?.ToString();
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // Get an image for the platform
                    IPlatform oSelectedPlatform = PluginHelper.StateManager.GetSelectedPlatform();

                    PropertyInfo[] oProperties = typeof(IPlatform).GetProperties();
                    sBinaryPath = oSelectedPlatform.BannerImagePath;

                    // A linq statement would look totally sexy here
                    foreach (PropertyInfo oProperty in oProperties)
                    {
                        if (oProperty.Name.ToLower() == sType + "path" || oProperty.Name.ToLower() == sType + "imagepath")
                        {
                            sBinaryPath = oProperty.GetValue(oProperties)?.ToString();
                            break;
                        }
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

        private static void SelectedGameMethods(HttpProcessor oHttpProcessor)
        {
            string sJSONResponse = string.Empty;
            string sMethodName = oHttpProcessor.pathParts[1].ToLower();

            if (!PluginAppSettings.GetBoolean("WriteEnabled") && !sMethodName.ToLower().StartsWith("get"))
            {
                oHttpProcessor.writeFailure("Marquesas Server in Read Only Mode.");
                return;
            }

            string[] oBooleans = { "true", "false", "1", "0", "yes", "no" };
            string sParamater = string.Empty;
            if (oHttpProcessor.pathParts.Length > 2)
            {
                sParamater = oHttpProcessor.pathParts[2].ToLower();
            }

            MethodInfo[] oGameMethods = typeof(IGame).GetMethods();
            foreach (MethodInfo oMethod in oGameMethods)
            {
                if (oMethod.Name.ToLower() == sMethodName)
                {
                    if (sParamater == String.Empty && oMethod.GetParameters().Length == 0)
                    {
                        sJSONResponse = JsonSerializer.Serialize(oMethod.Invoke(PluginHelper.StateManager.GetAllSelectedGames()[0], new object[] { }));
                        break;
                    }
                    else if (sParamater != String.Empty && oMethod.GetParameters().Length > 0)
                    {
                        if (oMethod.GetParameters()[0].ParameterType.ToString() == "System.Boolean")
                        {
                            if (Boolean.TryParse(sParamater, out bool bParamater) && oBooleans.Contains(sParamater.ToLower()))
                            {
                                sJSONResponse = JsonSerializer.Serialize(oMethod.Invoke(PluginHelper.StateManager.GetAllSelectedGames()[0], new object[] { bParamater }));
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            sJSONResponse = JsonSerializer.Serialize(oMethod.Invoke(PluginHelper.StateManager.GetAllSelectedGames()[0], new object[] { sParamater }));
                            break;
                        }
                    }
                }
            }

            WriteJSON(oHttpProcessor, sJSONResponse);
        }

        private static void WriteBinary(HttpProcessor oHttpProcessor)
        {
            if (!string.IsNullOrWhiteSpace(htCachedBinaries[oHttpProcessor.pathParts[1]].ToString()))
            {
                List<KeyValuePair<string, string>> additionalHeaders =
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Etag",
                            oHttpProcessor.request_url + gsEtagSeperator + PluginHelper.StateManager.GetAllSelectedGames()[0].Id),
                        new KeyValuePair<string, string>("Content-disposition",
                            "inline; filename=\"" +
                            Path.GetFileName(PluginHelper.StateManager.GetAllSelectedGames()[0]
                                .GetManualPath()) + "\""),
                        new KeyValuePair<string, string>("Content-Transfer-Encoding", "binary"),
                        new KeyValuePair<string, string>("Accept-Ranges", "none")
                    };

                string sExtension = Path.GetExtension(htCachedBinaryPaths[oHttpProcessor.pathParts[1]].ToString());
                if (sExtension.Length > 1)
                {
                    sExtension = sExtension.Substring(1);
                }

                oHttpProcessor.writeSuccess("application/" + sExtension,
                    ((byte[])htCachedBinaries[oHttpProcessor.pathParts[1]]).Length, "200 OK", additionalHeaders);

                oHttpProcessor.rawOutputStream.Write((byte[])htCachedBinaries[oHttpProcessor.pathParts[1]], 0,
                    ((byte[])htCachedBinaries[oHttpProcessor.pathParts[1]]).Length);
            }
            else
            {
                oHttpProcessor.writeFailure();
            }
        }

        #endregion Binary

        private void DoManual(HttpProcessor oHttpProcessor)
        {
            if (DoCacheResponseIfWarranted(oHttpProcessor) || !OneAndOnlyOneGameSelected(oHttpProcessor)) return;

            LoadBinary(oHttpProcessor);

            string sManualPath = htCachedBinaryPaths["manual"]?.ToString();

            if (string.IsNullOrWhiteSpace(sManualPath) || !File.Exists(sManualPath))
            {
                htCachedImageHTML["manual"] = Properties.Resources.HTML_NoResource
                        .Replace("<!-- SecondsBetweenRefresh -->",
                            (PluginAppSettings.GetInt("SecondsBetweenRefresh")).ToString())
                        .Replace("<!-- Path -->", sManualPath)
                        .Replace("<!-- Title -->", PluginHelper.StateManager.GetAllSelectedGames()[0].Title)
                    ;

                htCachedImagePaths["manual"] = "";
            }
            else if (htCachedImagePaths["manual"]?.ToString() != sManualPath)
            {
                htCachedImagePaths["manual"] = sManualPath;

                htCachedImageHTML["manual"] =
                    Properties.Resources.HTML_Manual
                        .Replace("<!-- SecondsBetweenRefresh -->",
                            (PluginAppSettings.GetInt("SecondsBetweenRefresh") * 1000).ToString())
                        .Replace("<!-- BinaryPath -->", "/binary/manual")
                    ;
            }

            WriteImageHTML(oHttpProcessor, htCachedImageHTML["manual"].ToString());
        }

        private static bool DoCacheResponseIfWarranted(HttpProcessor oHttpProcessor)
        {
            if (PluginAppSettings.GetBoolean("CacheDisabled")) return false;

            // Cache the response in the browser using the Etag and If-None-Match
            if (oHttpProcessor.pathParts != null && oHttpProcessor.httpHeaders.ContainsKey("if-none-match")
                && PluginHelper.StateManager.GetAllSelectedGames()?.Length == 1
                && oHttpProcessor.httpHeaders["if-none-match"] == oHttpProcessor.request_url + gsEtagSeperator +
                PluginHelper.StateManager.GetAllSelectedGames()[0].Id)
            {
                oHttpProcessor.writeNotModified();
                return true;
            }

            return false;
        }

        private static bool OneAndOnlyOneGameSelected(HttpProcessor oHttpProcessor)
        { 
            if (PluginHelper.StateManager.GetAllSelectedGames()?.Length == 1)
            {
                return true;
            }
            else
            {
                WriteImageHTML(oHttpProcessor,
                    Properties.Resources.HTML_NoGame.Replace("<!-- SecondsBetweenRefresh -->",
                        (PluginAppSettings.GetInt("SecondsBetweenRefresh")).ToString()));
                return false;
            }
        }

        private static string ComputeHash(string sStringToHash)
        {
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(sStringToHash)))
                .Replace("-", String.Empty);
        }
    }
}