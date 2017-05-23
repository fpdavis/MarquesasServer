using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using SimpleHttp;

namespace MarquesasServer
{
    public class MarquesasHttpServer : HttpServer
    {
        private static string sCachedMarqueHTML;
        private static string sCachedMarqueFilename;
        public GameObject oGameObject;

        public override void handleGETRequest(HttpProcessor p)
        {
            // Cache the response in the browser using the Etag and If-None-Match
            if (p.pathParts != null && p.httpHeaders.ContainsKey("if-none-match")
                && p.httpHeaders["if-none-match"] == p.pathParts[0].ToLower() + "-" + oGameObject.Title)
            {
                p.writeNotModified();
                return;
            }

            string sHTMLResponse = string.Empty;

            switch (p.request_url.ToString().ToLower())
            {
                case "/manual":
                    p.writeFailure();
                    break;
                //case "/marque":
                default:
                    sHTMLResponse = DoMarque(p);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(sHTMLResponse))
            {
                List<KeyValuePair<string, string>> additionalHeaders =
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Etag",
                            p.pathParts[0].ToLower() + "-" + oGameObject.Title)
                    };

                p.writeSuccess("text/html", sHTMLResponse.Length, "200 OK", additionalHeaders);

                p.outputStream.WriteLine(sHTMLResponse);
            }
            else
            {
                p.writeFailure();
            }
        }

        private string DoMarque(HttpProcessor p)
        {
            if (sCachedMarqueFilename == oGameObject.Marque) return sCachedMarqueHTML;

            sCachedMarqueFilename = oGameObject.Marque;
            sCachedMarqueHTML = Properties.Resources.HTML_Marque.Replace("<!-- Base64Image -->", MakeImageSrcData(oGameObject.Marque));

            return (sCachedMarqueHTML);

        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            handleGETRequest(p);
        }

        string MakeImageSrcData(string sFilename)
        {
            if (File.Exists(sFilename))
            {
                return @"data:image/" + Path.GetExtension(sFilename).Substring(1) + ";base64," + Convert.ToBase64String(File.ReadAllBytes(sFilename));
            }

            return string.Empty;
        }

        public override void stopServer()
        {
            base.stopRequested = true;
        }
    }
}
