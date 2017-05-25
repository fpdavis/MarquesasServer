using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace MarquesasServer
{
    class MarqueServer_ISystemEventsPlugin : ISystemEventsPlugin, IDisposable
    {
        private PluginAppSettings oPluginAppSettings = new PluginAppSettings();
        
        public void OnEventRaised(string eventType)
        {
            switch (eventType)
            {
                case SystemEventTypes.GameStarting:
                    MarquesasHttpServerInstance.IsInGame = true;
                    break;
                case SystemEventTypes.GameExited:
                    MarquesasHttpServerInstance.IsInGame = false;
                    break;
                case SystemEventTypes.BigBoxStartupCompleted:
                case SystemEventTypes.LaunchBoxStartupCompleted:
                    MarquesasHttpServerInstance.RunningServer.port = oPluginAppSettings.GetBoolean("PortEnabled") ? oPluginAppSettings.GetInt("Port") : -1;
                    MarquesasHttpServerInstance.RunningServer.secure_port = oPluginAppSettings.GetBoolean("SecurePortEnabled") ? oPluginAppSettings.GetInt("SecurePort") : -1;
                    MarquesasHttpServerInstance.RunningServer.Initialize();
                    MarquesasHttpServerInstance.RunningServer.Start();
                    break;
                case SystemEventTypes.BigBoxShutdownBeginning:
                case SystemEventTypes.LaunchBoxShutdownBeginning:
                    MarquesasHttpServerInstance.RunningServer.Stop();
                    break;
            }
        }

        public void Dispose()
        {
            oPluginAppSettings?.Dispose();
        }
    }
}
