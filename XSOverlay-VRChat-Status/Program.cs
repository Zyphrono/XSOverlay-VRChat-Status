using System;
using System.Threading;
using System.Timers;
using XSNotifications;
using XSNotifications.Enum;
using XSNotifications.Helpers;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace XSOverlay_VRChat_Status
{
    class Program
    {
        public static XSNotifier Notifier { get; set; }
        public static System.Timers.Timer checkTimer;
        public static Classes.ServiceInfo Serviceinfo;
        public static Classes.Update updater;
        public static Classes.NotificationHandler notificationHandler;
        private static Mutex applicationMutex;
        private static bool isMutedActive = false;
        public static int noConnectionamount { get; set; }
        public static string errorMessageTitle = Properties.Resources.errorMessage_Title;
        public static string errorMessageDefaultMessage = Properties.Resources.errorMessage_DefaultMessage;

        static void Main(string[] args)
        {
            try
            {
                applicationMutex = new Mutex(true, "XSOVRCStatus", out isMutedActive);
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

            try
            {
                var updateversion = updater.CheckForUpdatesAsync().GetAwaiter().GetResult();

                if (updateversion != null)
                {
                    if (MessageBox.Show("Looks like there's an update available. Would you like to update and restart the application?", "Update available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Log("An update is currently being downloaded and will be installed automatically. ");
                        updater.PrepareUpdateAsync(updateversion).Wait();

                        Log("Update has been downloaded. The program will now install and restart.");
                        updater.FinalizeUpdate(true);
                    }
                }
                else
                {
                    Log("[UPDATER] You're currently using the latest stable build: " + updater.LatestAvailableVersion);
                }
            } catch(Exception e)
            {
                Log("[WARNING] Couldn't connect to GitHub to check for updates.");
            }

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
            checkTimer.Interval = 10000;
            checkTimer.Enabled = true;

            if(!Serviceinfo.VRChatRunning())
            {
                Log("[NOTICE] VRChat isn't running. The application will now sleep until VRChat becomes active.");
            } else
            {
                Log("[NOTICE] VRChat is running. You're now receiving updates about their server status.");
            }
            Thread.Sleep(Timeout.Infinite);
        }

        static void cancelCheckTimer()
        {
            checkTimer.Stop();
            checkTimer.Dispose();
        }

        public static void prepareShutdown() // Doesn't do anything besides cancelling everything while still running the console.
        {
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

        public static void Log(string message) //Log to the console that is running.
        {
            Console.WriteLine(message);
        }
    }
}