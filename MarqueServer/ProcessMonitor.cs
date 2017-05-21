using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.ComponentModel;

namespace MarqueServer
{
    class ProcessMonitor
    {
        ManagementEventWatcher startWatch;
        ManagementEventWatcher stopWatch;
        public GameObject oGameObject;

        public ProcessMonitor()
        {
        }

        public void WaitForProcess()
        {
            startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
            
            stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopWatch.EventArrived += new EventArrivedEventHandler(stopWatch_EventArrived);
            
            startWatch.Start();
            stopWatch.Start();

        }

        private void stopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            stopWatch.Stop();

            try
            {
                Console.WriteLine("Process stopped: {0}", e.NewEvent.Properties["ProcessName"].Value);
            }
            catch
            {
                Console.WriteLine("Failed to read process information");
            }
            finally
            {
                stopWatch.Start();
            }
        }

        private void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            startWatch.Stop();

            try
            {
                Console.WriteLine("Process started: {0}", e.NewEvent.Properties["ProcessName"].Value);

                int iProcessId;
                if (int.TryParse(e.NewEvent.Properties["ProcessId"].Value.ToString(), out iProcessId))
                {
                    string sCommandLine = GetCommandLine(iProcessId);
                    Console.WriteLine("Command Line Args: {0}", sCommandLine);

                    var RomMatch = XMLSettings.RomPaths.FirstOrDefault(RomPath => sCommandLine.Contains(RomPath));
                    var PlatformMatch = XMLSettings.Platforms.FirstOrDefault(RomPath => sCommandLine.Contains(RomPath));
                    var sMarquePath = XMLSettings.GetPlatformFolder(PlatformMatch, "Arcade - Marquee");

                    // If there is a match we need to record the ProcessId and then look for it to exit.
                }
            }
            catch
            {
                Console.WriteLine("Failed to read process information");
            }
            finally
            {
                startWatch.Start();
            }
        }

        // Define an extension method for type System.Process that returns the command 
        // line via WMI.
        private static string GetCommandLine(int iProcessId)
        {
            string cmdLine = string.Empty;
            try
            {                
                using (var searcher = new ManagementObjectSearcher(
                  $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {iProcessId}"))
                {
                    // By definition, the query returns at most 1 match, because the process 
                    // is looked up by ID (which is unique by definition).
                    var matchEnum = searcher.Get().GetEnumerator();
                    if (matchEnum.MoveNext()) // Move to the 1st item.
                    {
                        cmdLine = matchEnum.Current["CommandLine"]?.ToString();
                    }
                }
            }
            catch { }
            
            return cmdLine;
        }
    }
}
