using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using System.Runtime.InteropServices;
using System.Management;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace winlock
{
    public partial class winlock : ServiceBase
    {
        private static readonly object _lock = new object();
        private static DateTime _lastEventTime = DateTime.MinValue;
        private static readonly TimeSpan _debounceInterval = TimeSpan.FromSeconds(1);
        private static Timer checkTimer;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        private ManagementEventWatcher watcher;

        public winlock(string[] args)
        {
            InitializeComponent();
            string eventSourceName = "MySource";
            string logName = "MyNewLog";

            if (args.Length > 0)
            {
                eventSourceName = args[0];
            }

            if (args.Length > 1)
            {
                logName = args[1];
            }

            eventLog1 = new EventLog();

            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }

            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;
            SetupTimer();
        }

        private void SetupTimer()
        {
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(TimerElapsedEvent);
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private static bool DetectUsbKeyboard() 
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    @"SELECT * FROM Win32_Keyboard WHERE Description LIKE '%USB%'"))
                {
                    var keyboards = searcher.Get();
                    foreach (ManagementObject keyboard in keyboards)
                    {
                        string description = keyboard["Description"]?.ToString() ?? "";
                        if (description.Contains("USB"))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        private static bool DetectUsbMouse() 
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    @"SELECT * FROM Win32_PointingDevice WHERE Description LIKE '%USB%'"))
                {
                    var mice = searcher.Get();
                    foreach (ManagementObject mouse in mice)
                    {
                        string description = mouse["Description"]?.ToString() ?? "";
                        if (description.Contains("USB"))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static string Detectusbi() 
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    return drive.Name;
                }
            }
            return null;
        }

        private void TimerElapsedEvent(object sender, ElapsedEventArgs e) 
        {
            lock (_lock)

            {
                try
                {
                    string usbi = Detectusbi();
                    bool hasUsbKeyboard = DetectUsbKeyboard();
                    bool hasUsbMouse = DetectUsbMouse();

                    if (!string.IsNullOrEmpty(usbi)) 
                    {
                        string tiedosto = "bypassthelock.txt"; 
                        bool found = Etsitiedosto(usbi, tiedosto);

                        if (!found) 
                        {
                            //eventLog1.WriteEntry("USB-Stick, without bypass file DETECTED!, LOCKING DOWN!.");
                            testaustaski();
                            return;
                        }
                      
                        return;
                    }

                
                    if (hasUsbKeyboard)
                    {
                        //eventLog1.WriteEntry("USB-KEYBOARD DETECTED, LOCKING DOWN!");
                        testaustaski();
                        return;
                    }
                    if (hasUsbMouse)
                    {
                        //eventLog1.WriteEntry("USB-MOUSE DETECTED, LOCKING DOWN!");
                        testaustaski();
                        return;
                    }

      
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry($"Timer error: {ex.Message}", EventLogEntryType.Error);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 5000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            //ListAllDevices();

        }

        protected override void OnStop()
        {
            if (watcher != null)
            {
                watcher.Stop();
                watcher.Dispose();
            }
        }

        private void testaustaski() // Run the task that will lock your workstation
        {
            string taskName = "winlockTask"; // task name that lock the workstation
            Process.Start("schtasks.exe", $"/Run /TN \"{taskName}\"");
            //eventLog1.WriteEntry("System Lock Task Runned!");
        }

        private static bool Etsitiedosto(string usbi, string fileName) // Sniffing bypass file.
        {
            try
            {
                foreach (string file in Directory.EnumerateFiles(usbi, "*", SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(file).Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }




        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
        }
    }
}