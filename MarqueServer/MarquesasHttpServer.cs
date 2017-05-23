using System;
using System.IO;
using System.Linq;
using System.Net;

namespace MarquesasServer
{
    public class MarquesasHttpServer : HttpServer
    {
        private static string sCachedFilename;
        private static string sCachedMakeImageSrcData;

        public MarquesasHttpServer()
        {
        }
        public MarquesasHttpServer(IPAddress oIPAddress, int port)
            : base(oIPAddress, port)
        {
        }
        public override void handleGETRequest(HttpProcessor p)
        {
            // Cache the response in the browser using the Etag and If-None-Match
            if (p.httpHeaders["If-None-Match"]?.ToString() == p.http_url.ToLower() + "-" + oGameObject.Title)
            {
                p.writeNotModified();
                return;
            }

            string sHTMLResponse = string.Empty;

            switch (p.http_url.ToLower())
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
                p.writeSuccess(p.http_url.ToLower() + "-" + oGameObject.Title);
                p.outputStream.WriteLine(sHTMLResponse);
            }
            else
            {
                p.writeFailure();
            }
        }

        private string DoMarque(HttpProcessor p)
        {
            return (Properties.Resources.HTML_Marque.Replace("<!-- Base64Image -->", MakeImageSrcData(oGameObject.Marque)));


        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            handleGETRequest(p);
        }

        string MakeImageSrcData(string sFilename)
        {
            if (File.Exists(sFilename))
            {
                sCachedFilename = sFilename;
                sCachedMakeImageSrcData = @"data:image/" + Path.GetExtension(sFilename).Substring(1) + ";base64," +
                                          Convert.ToBase64String(File.ReadAllBytes(sFilename));
                return sCachedMakeImageSrcData;
            }

            return string.Empty;
        }
    }
}
