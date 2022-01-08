using System;
using System.Threading;
using System.Timers;
using XSNotifications;
using XSNotifications.Enum;
using XSNotifications.Helpers;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace XSOverlay_VRChat_Status
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        public static XSNotifier Notifier { get; set; }
        public static System.Timers.Timer checkTimer;
        public static Classes.ServiceInfo Serviceinfo;
        public static Classes.Update updater;
        public static Classes.NotificationHandler notificationHandler;
        public static int noConnectionamount { get; set; }
        public readonly string errorMessageTitle = Properties.Resources.errorMessage_Title;
        public readonly string errorMessageDefaultMessage = Properties.Resources.errorMessage_DefaultMessage;

        protected Program()
        {
        }
        static void Main(string[] args)
        {
            bool isMutedActive = false;
            settingsCheck();
            Classes.NotificationHandler.createNotifyMenu();
            minimisedCheck();
            Classes.StartupHandler.startupPrompt();
            try
            {
                Mutex applicationMutex = new Mutex(true, "XSOVRCStatus", out isMutedActive);
                applicationMutex.WaitOne(TimeSpan.Zero);
                if (!isMutedActive)
                {
                    MessageBox.Show("Looks like another instance is already running. This instance will now be closed.");
                    Environment.Exit(0);
                }
            }
            catch
            {
                MessageBox.Show("Looks like another instance is already running. This instance will now be closed.");
                Environment.Exit(0);
            }
            updater = new Classes.Update();
            checkForUpdates();
            try
            {
                Notifier = new XSNotifier();
                Serviceinfo = new Classes.ServiceInfo();
                notificationHandler = new Classes.NotificationHandler();
                Notifier.SendNotification(new XSNotification
                {
                    AudioPath = XSGlobals.GetBuiltInAudioSourceString(XSAudioDefault.Warning),
                    Title = "VRChat Status started!",
                    Content = $"You're now receiving notifications on VRChat's status update. (If game is running)",
                    Height = 110.0f
                });
            }
            catch (Exception ex)
            {
                Log("XSNotifier couldn't be started. Please try again.");
                Log(Convert.ToString(ex));
                prepareShutdown();
            }

            checkTimer = new System.Timers.Timer();
            checkTimer.Elapsed += new ElapsedEventHandler(startCheck);
            checkTimer.Interval = 20000;
            checkTimer.Enabled = true;

            if(!Serviceinfo.VRChatRunning())
            {
                Log("[NOTICE] VRChat isn't running. The application will now sleep until VRChat becomes active.");
            } else
            {
                Log("[NOTICE] VRChat is running. You're now receiving updates about their server status.");
            }
            if(Properties.Settings.Default.launchMinimised == false)
            {
                Log("HINT: You can leftclick the tray icon to hide and unhide this console window. You can also rightclick this tray icon for some extra settings.");
            }
            Thread.Sleep(Timeout.Infinite);
        }

        static void cancelCheckTimer()
        {
            checkTimer.Stop();
            checkTimer.Dispose();
        }
        public static void checkForUpdates()
        {
            Log("Checking for updates...");
            Log("");
            try
            {
                var updateversion = updater.CheckForUpdatesAsync().GetAwaiter().GetResult();
                if (updateversion != null)
                {
                    toggleWindow(1);
                    if (MessageBox.Show("Looks like there's an update available (" + updater.LastVersion + "). Would you like to update and restart the application?", "Update available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Log("An update is currently being downloaded and will be installed automatically. ");
                        updater.PrepareUpdateAsync(updateversion).Wait();

                        Log("Update has been downloaded. The program will now install and restart.");
                        updater.FinalizeUpdate(true);
                    }
                    else
                    {
                        if (Properties.Settings.Default.launchMinimised == true)
                        {
                            toggleWindow(0);
                        }
                        Log("[WARNING] Keeping your software up-to-date is essential to fix any issues. Pre-released versions will NEVER be installed to provide stability.");
                    }
                }
                else
                {
                    Log("[UPDATER] You're currently using the latest stable build: " + updater.LatestAvailableVersion);
                }
            }
            catch
            {
                Log("[WARNING] Couldn't connect to GitHub to check for updates.");
            }
        }
        public static void prepareShutdown() // Doesn't do anything besides cancelling everything while still running the console.
        {
            toggleWindow(1);
            Log("");
            Log("The application is now inactive. Please restart to reactivate.");
            Notifier.Dispose();
            cancelCheckTimer();
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void startCheck(Object source, ElapsedEventArgs e) // Start the status check when the timer runs out.
        {
            if (Serviceinfo.VRChatRunning())
            {
                Serviceinfo.getStatusses();
            }
            Thread.Sleep(Timeout.Infinite);
        }

        public static void SendNotification(XSNotification notification)
        {
            try
            {
                Notifier.SendNotification(notification);
            }
            catch (Exception ex)
            {
                Log("Error sending message to XSOverlay. Is it running?");
                Log(Convert.ToString(ex));
            }
        }

        public static void Log(string message, ConsoleColor color = ConsoleColor.White) //Log to the console that is running.
        {
            switch (message)
            {
                case string a when a.Contains("[WARNING]"):
                    color = ConsoleColor.Yellow;
                    break;
                case string a when a.Contains("[ERROR]"):
                    color = ConsoleColor.Red;
                    break;
                case string a when a.Contains("[NOTICE]"):
                    color = ConsoleColor.Magenta;
                    break;
            }
            string currentTime = DateTime.Now.ToString("T");
            Console.ForegroundColor = color;
            if (message == "")
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("[{0}] " + message, currentTime);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void settingsCheck()
        {
            if(Classes.StartupManager.IsInStartup() == true)
            {
                Properties.Settings.Default.windowsStartup = true;
            } else
            {
                Properties.Settings.Default.windowsStartup = false;
            }

            Properties.Settings.Default.Save();
        }

        public static void minimisedCheck()
        {
            if (Properties.Settings.Default.launchMinimised == true)
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE);
                Classes.NotificationHandler.minimizeMode = 1;
            } else
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_SHOW);
                Classes.NotificationHandler.minimizeMode = 0;
            }
        }
        
        public static void toggleWindow(int type)
        {
            if(type == 0)
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE);
            } else if(type == 1)
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_SHOW);
                SetForegroundWindow(handle);
            }
        }
    }
}