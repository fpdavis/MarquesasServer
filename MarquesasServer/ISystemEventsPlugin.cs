using System;
using System.Reflection;
using System.Windows.Forms;
using CommonPluginHelper;
using MarquesasServer.Properties;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace MarquesasServer
{
    internal class MarqueServer_ISystemEventsPlugin : ISystemEventsPlugin, IDisposable
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

                    StartHttpServices();
                    //StartSuperSockets();
                    break;

                case SystemEventTypes.BigBoxShutdownBeginning:
                case SystemEventTypes.LaunchBoxShutdownBeginning:
                    MarquesasHttpServerInstance.RunningServer.Stop();
                    //SuperSocketAppServerInstance.RunningServer.Stop();
                    break;
            }
        }

        public void StartHttpServices()
        {
            MarquesasHttpServerInstance.RunningServer.port = PluginAppSettings.GetBoolean("PortEnabled") ? PluginAppSettings.GetInt("Port") : -1;
            MarquesasHttpServerInstance.RunningServer.secure_port = PluginAppSettings.GetBoolean("SecurePortEnabled") ? PluginAppSettings.GetInt("SecurePort") : -1;
            MarquesasHttpServerInstance.RunningServer.Initialize();
            MarquesasHttpServerInstance.RunningServer.WarnIfPortsAreNotAvailable();
            MarquesasHttpServerInstance.RunningServer.Start();
            MarquesasHttpServerInstance.RunningServer.FirstTimeRunCheck();
        }

        //public void StartSuperSockets()
        //{
        //    //Setup the appServer
        //    if (!SuperSocketAppServerInstance.RunningServer.Setup(PluginAppSettings.GetInt("SuperSocketPort"))) //Setup with listening port
        //    {
        //        // Failed to setup!
        //        MessageBox.Show("Failed setup for Socket Server.", Resources.ApplicationName, MessageBoxButtons.OK);
        //        return;
        //    }

        //    //Try to start the appServer
        //    if (!SuperSocketAppServerInstance.RunningServer.Start())
        //    {
        //        // Failed to start!
        //        MessageBox.Show("Failed to start Socket Server on port " + PluginAppSettings.GetInt("SuperSocketPort") + ".", Resources.ApplicationName, MessageBoxButtons.OK);
        //        return;
        //    }
        //}

        public void Dispose()
        {
        }
    }
}