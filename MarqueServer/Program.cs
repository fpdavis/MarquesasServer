using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Configuration;


namespace MarquesasServer
{
    class Program
    {
        static void Main(string[] args)
        {
            PluginAppSettings oPluginAppSettings = new PluginAppSettings();
            List<MarquesasHttpServer> oaMarquesasHttpServers = new List<MarquesasHttpServer>();
            GameObject oGameObject = new GameObject();
            
            oGameObject.Title = "SelectedGame";
            oGameObject.Marque = @"C:\OneDrive\Data\Source\MarqueServer\ScreenShots\WindowsSecurityAlert.PNG";
            //oGameObject.Marque = @"C:\Users\pdavis.UMPH\Desktop\T0K631M3L-U0K63PQRW-d63ccb8475c7-512.jpg.e5c305a8468794ca6218e7d7b31eb355.jpg";
            new Spinup(oaMarquesasHttpServers, oPluginAppSettings, oGameObject);
            
            //var oProcessMonitor = new ProcessMonitor();
            //oProcessMonitor.oGameObject = oGameObject;
            //oProcessMonitor.WaitForProcess();
        }
    }
}

