using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using SimpleHttp;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using CommonPluginHelper;

namespace MarquesasServer
{
    public static class MarquesasHttpServerInstance
    {
        public static MarquesasHttpServer RunningServer = new MarquesasHttpServer();
        public static Boolean IsInGame;
    }

    public class MarquesasHttpServer : HttpServer
    {
        private static Hashtable htCachedImagePaths = new Hashtable();
        private static Hashtable htCachedImageHTML = new Hashtable();
        
        public override void handleGETRequest(HttpProcessor oHttpProcessor)
        {
            if (oHttpProcessor.pathParts?.Length > 0)
            {
                switch (oHttpProcessor.pathParts[0])
                {
                    case "statemanager":
                        DoStateManager(oHttpProcessor);
                        break;
                    case "selectedgames":
                        DoSelectedGames(oHttpProcessor);
                        break;
                    case "manual":
                        oHttpProcessor.writeFailure();
                        break;
                    case "image":
                        DoImage(oHttpProcessor);
                        break;
                    default:
                        DoDefaultPage(oHttpProcessor);
                        break;
                }
            }

        }

        private static void WriteJSON(HttpProcessor oHttpProcessor, string sJSONResponse)
        {
            oHttpProcessor.writeSuccess("application/json", sJSONResponse.Length);
            oHttpProcessor.outputStream.WriteLine(sJSONResponse);
        }

        private static void WriteImageHTML(HttpProcessor oHttpProcessor, string sHTMLResponse)
        {
            if (!string.IsNullOrWhiteSpace(sHTMLResponse))
            {
                List<KeyValuePair<string, string>> additionalHeaders =
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Etag",
                            oHttpProcessor.request_url + "/" + PluginHelper.StateManager.GetAllSelectedGames()[0].Id)
                    };

                oHttpProcessor.writeSuccess("text/html", sHTMLResponse.Length, "200 OK", additionalHeaders);

                oHttpProcessor.outputStream.WriteLine(sHTMLResponse);
            }
            else
            {
                oHttpProcessor.writeFailure();
            }
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
        private void DoSelectedGames(HttpProcessor oHttpProcessor)
        {
            string sJSONResponse = string.Empty;

            if (oHttpProcessor.pathParts.Length == 1)
            {
                sJSONResponse = new JavaScriptSerializer().Serialize(PluginHelper.StateManager.GetAllSelectedGames());
            }
            else if (oHttpProcessor.pathParts.Length == 2 && PluginHelper.StateManager.GetAllSelectedGames().Length == 1)
            {
                PropertyInfo[] oProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IGame).GetProperties();

                // A linq statement would look totally sexy here
                foreach (PropertyInfo oProperty in oProperties)
                {
                    if (oProperty.Name.ToLower() == oHttpProcessor.pathParts[1].ToLower())
                    {
                        sJSONResponse = new JavaScriptSerializer().Serialize(oProperty.GetValue(PluginHelper.StateManager.GetAllSelectedGames()[0]));
                    }
                }
            }
 
            WriteJSON(oHttpProcessor, sJSONResponse);
        }

        private void DoImage(HttpProcessor oHttpProcessor)
        {
            // Cache the response in the browser using the Etag and If-None-Match
            if (oHttpProcessor.pathParts != null && oHttpProcessor.httpHeaders.ContainsKey("if-none-match")
               && PluginHelper.StateManager.GetAllSelectedGames().Length > 0
               && oHttpProcessor.httpHeaders["if-none-match"] == oHttpProcessor.request_url + "/" + PluginHelper.StateManager.GetAllSelectedGames()[0].Id)
            {
                oHttpProcessor.writeNotModified();
                return;
            }

            if (oHttpProcessor.pathParts.Length > 1 && PluginHelper.StateManager.GetAllSelectedGames().Length == 1)
            {
                string sImagePath = null;
                
                PropertyInfo[] oProperties = typeof(IGame).GetProperties();

                // A linq statement would look totally sexy here
                foreach (PropertyInfo oProperty in oProperties)
                {
                    if (oProperty.Name.ToLower() == oHttpProcessor.pathParts[1] + "imagepath")
                    {
                        sImagePath = oProperty.GetValue(PluginHelper.StateManager.GetAllSelectedGames()[0]).ToString();
                    }
                }

                if (!string.IsNullOrWhiteSpace(sImagePath)
                    && (htCachedImagePaths[oHttpProcessor.pathParts[1].ToLower()]?.ToString() != sImagePath)
                    && File.Exists(sImagePath))
                {
                    htCachedImagePaths[oHttpProcessor.pathParts[1]] = sImagePath;

                    htCachedImageHTML[oHttpProcessor.pathParts[1]] =
                        Properties.Resources.HTML_Marque.Replace("<!-- SecondsBetweenRefresh -->", PluginAppSettings.GetString("SecondsBetweenRefresh")).Replace("<!-- Base64Image -->", MakeImageSrcData(sImagePath));
                }
            }

            WriteImageHTML(oHttpProcessor, htCachedImageHTML[oHttpProcessor.pathParts[1]].ToString());

        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            handleGETRequest(p);
        }

        string MakeImageSrcData(string sImagePath)
        {
            return @"data:image/" + Path.GetExtension(sImagePath).Substring(1) + ";base64," + Convert.ToBase64String(File.ReadAllBytes(sImagePath));
        }

        public override void stopServer()
        {
            base.stopRequested = true;
        }

        public void DoDefaultPage(HttpProcessor oHttpProcessor)
        {
            StringBuilder sbDefaultPage = new StringBuilder();

            PropertyInfo[] oProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IGame).GetProperties();

            foreach (PropertyInfo oProperty in oProperties)
            {
                if (oProperty.Name.Contains("ImagePath"))
                {
                    sbDefaultPage.AppendLine("<li/><a href='/image/" + oProperty.Name.Replace("ImagePath", "").ToLower() + "'>Image/" +
                                             oProperty.Name.Replace("ImagePath", "") + "</a>");
                }
            }

            sbDefaultPage.AppendLine("<br/><br/>");

            sbDefaultPage.AppendLine("<li/><a href='/statemanager'>StateManager</a>");
            sbDefaultPage.AppendLine("<li/><a href='/statemanager/isingame'>StateManager/IsInGame</a>");

            oProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IStateManager).GetProperties();

            foreach (PropertyInfo oProperty in oProperties)
            {
                sbDefaultPage.AppendLine("<li/><a href='/statemanager/" + oProperty.Name.ToLower() + "'>StateManager/" + oProperty.Name + "</a>");
            }

            sbDefaultPage.AppendLine("<br/><br/>");

            sbDefaultPage.AppendLine("<li/><a href='/selectedgames'>SelectedGames</a>");

            oProperties = typeof(Unbroken.LaunchBox.Plugins.Data.IGame).GetProperties();

            foreach (PropertyInfo oProperty in oProperties)
            {
                    sbDefaultPage.AppendLine("<li/><a href='/selectedgames/" + oProperty.Name.ToLower() + "'>SelectedGames/" + oProperty.Name + "</a>");
            }

            List<KeyValuePair<string, string>> additionalHeaders =
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Etag",
                        oHttpProcessor.request_url + "/" + ComputeHash(sbDefaultPage))
                };

            oHttpProcessor.writeSuccess("text/html", sbDefaultPage.ToString().Length, "200 OK", additionalHeaders);

            oHttpProcessor.outputStream.Write(sbDefaultPage);
        }

        private static byte[] ComputeHash(StringBuilder sbDefaultPage)
        {
            return new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.ASCII.GetBytes(sbDefaultPage.ToString()));
        }
    }
}
