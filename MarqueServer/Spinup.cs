using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MarquesasServer
{
    class Spinup : IDisposable
    {
        private GameObject oGameObject;

        public Spinup(List<MarquesasHttpServer> oaMarquesasHttpServers, PluginAppSettings oPluginAppSettings, GameObject oGameObject)
        {
            // WMIC path win32_process get Commandline | grep EnableMarqueServer
            // "D:\Emulators\RocketLauncher\RocketLauncher.exe" -EnableMarqueServer -s "Atari 7800" -r "D:\Emulators\Roms\Atari 7800\3D Asteroids (1987) (Atari) (Prototype).a78"

            this.oGameObject = oGameObject;
            int iPort = oPluginAppSettings.GetInt("Port") == 0 ? 8080 : oPluginAppSettings.GetInt("Port");

            //XMLSettings.LoadPlatformXML();

            Console.WriteLine("Available IP Addresses");
            foreach (var i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            foreach (var ua in i.GetIPProperties().UnicastAddresses)
            {
                if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine("   " + ua.Address);
                }
            }

            Console.WriteLine();

        //    if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IPAddress"]))
          //  {
                //Console.WriteLine("Using IP Address bindings from App.config...");

                //var saConfigIPAddresses = ConfigurationManager.AppSettings["IPAddress"].Split(',');
                //IPAddress oIPAddress;

                //foreach (string sConfigIPAddress in saConfigIPAddresses)
                //{
                //    if (IPAddress.TryParse(sConfigIPAddress, out oIPAddress))
                //    {
                //        Console.WriteLine("   Binding to " + oIPAddress.ToString() + " on port " + iPort);

                     //   oaMarquesasHttpServers.Add(new MarquesasHttpServer(iPort));
                  //      oaMarquesasHttpServers[0].port = iPort;
                  //      oaMarquesasHttpServers[0].oIPAddress = oIPAddress;
                //        oaMarquesasHttpServers[0].oGameObject = oGameObject;
            //oaMarquesasHttpServers[0].Start();
                  //      Thread thread = new Thread(new ThreadStart(oaMarquesasHttpServers[0].listen));
                  //      thread.Start();
                  //    }
                  //}
                  //  }
                  //else
                  //{
                  //    Console.WriteLine("Binding to all available IP Addresses...");

            //    foreach (var i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            //        foreach (var oIPAddress in i.GetIPProperties().UnicastAddresses)
            //        {
            //            if (oIPAddress.Address.AddressFamily == AddressFamily.InterNetwork)
            //            {
            //                Console.WriteLine("   Binding to " + oIPAddress.Address.ToString() + " on port " + iPort);

            //                try
            //                {
            //                    oaMarquesasHttpServers.Add(new MarquesasHttpServer());
            //                    oaMarquesasHttpServers[oaMarquesasHttpServers.Count - 1].port = iPort;
            //                    oaMarquesasHttpServers[oaMarquesasHttpServers.Count - 1].oIPAddress = oIPAddress.Address;
            //                    oaMarquesasHttpServers[oaMarquesasHttpServers.Count - 1].oGameObject = oGameObject;
            //                    Thread thread = new Thread(new ThreadStart(oaMarquesasHttpServers[oaMarquesasHttpServers.Count - 1].listen));
            //                    thread.Start();
            //                }
            //                catch
            //                {
            //                    Console.WriteLine("Could not bind to " + oIPAddress.Address.ToString());
            //                }
            //            }
            //        }
            //}

        }

        public void Dispose()
        {
        }
    }
}