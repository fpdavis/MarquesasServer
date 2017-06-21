using System;
using CommonPluginHelper;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace MarquesasServer
{
    class MarqueServer_ISystemEventsPlugin : ISystemEventsPlugin, IDisposable
    {
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
                    MarquesasHttpServerInstance.RunningServer.port = PluginAppSettings.GetBoolean("PortEnabled") ? PluginAppSettings.GetInt("Port") : -1;
                    MarquesasHttpServerInstance.RunningServer.secure_port = PluginAppSettings.GetBoolean("SecurePortEnabled") ? PluginAppSettings.GetInt("SecurePort") : -1;
                    MarquesasHttpServerInstance.RunningServer.Initialize();
                    MarquesasHttpServerInstance.RunningServer.WarnIfPortsAreNotAvailable();
                    MarquesasHttpServerInstance.RunningServer.Start();
                    MarquesasHttpServerInstance.RunningServer.FirstTimeRunCheck();
                    break;
                case SystemEventTypes.BigBoxShutdownBeginning:
                case SystemEventTypes.LaunchBoxShutdownBeginning:
                    MarquesasHttpServerInstance.RunningServer.Stop();
                    break;
            }
        }

        public void Dispose()
        {
        }
    }
}
