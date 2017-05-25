using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace MarquesasServer
{
    class XMLSettings
    {
        public static string[] RomPaths { get; private set; }
        public static string[] Platforms { get; private set; }

        /// <summary>
        /// The platform settings document.
        /// </summary>
        private static XDocument PlatformList { get; set; }
        public static void LoadPlatformXML()
        {
            string sPlatformXMLPath = ConfigurationManager.AppSettings["PlatformXMLPath"];
            
            PlatformList = XDocument.Load(sPlatformXMLPath + ".xml");
            Platforms = PlatformList.Root.Descendants("Name").Select(x => x.Value).ToArray();
            RomPaths = new string[] { };

            string[] files = Directory.GetFiles(sPlatformXMLPath).Where(p => p.EndsWith(".xml")).ToArray();
            foreach (var path in files)
            {
                XDocument xDoc = XDocument.Load(path);

                // read Xml file
                RomPaths = RomPaths.Concat( xDoc.Root.Descendants("ApplicationPath").Select(x => x.Value.Replace("..\\", "")).ToArray() ).ToArray();
            }

        }

        /// <summary>
        /// Gets the Folder Path for the given Platform and Media Type.
        /// 
        /// Users can override the default image paths for platforms. This information is stored in the Data/Platforms.xml file in PlatfromFolder nodes.
        /// </summary>
        public static string GetPlatformFolder(string platform, string mediaType)
        {
            XElement platformFolder = (from SettingsXml in PlatformList.Descendants("PlatformFolder")
                                       where SettingsXml.Element("Platform").Value == platform
                                          && SettingsXml.Element("MediaType").Value == mediaType
                                       select SettingsXml).FirstOrDefault();

            return platformFolder != null ? platformFolder.Element("FolderPath").Value : "";
        }
    }
}
