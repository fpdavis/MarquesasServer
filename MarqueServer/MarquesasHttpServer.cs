using System;
using System.IO;
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
            switch (p.http_url.ToLower())
            {
                case "/manual":
                    p.writeSuccess();
                    break;
                //case "/marque":
                default:
                    p.writeSuccess();
                    p.outputStream.WriteLine(Properties.Resources.HTML_Marque.Replace("<!-- Base64Image -->", MakeImageSrcData(oGameObject.Marque)));
                    break;
            }
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            handleGETRequest(p);
        }

        string MakeImageSrcData(string sFilename)
        {
            if (sFilename == sCachedFilename)
            {
                return sCachedMakeImageSrcData;
            }

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
