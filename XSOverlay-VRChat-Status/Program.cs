using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using XSNotifications;
using XSNotifications.Enum;
using XSNotifications.Helpers;

namespace XSOverlay_VRChat_Status
{
    class Program
    {
        public static XSNotifier Notifier { get; set; }
        public static System.Timers.Timer checkTimer;
        public static Classes.ServiceInfo Serviceinfo;
        public static Classes.NotificationHandler notificationHandler;

        public static int noConnectionamount { get; set; }
        public static string errorMessageTitle = Properties.Resources.errorMessage_Title;
        public static string errorMessageDefaultMessage = Properties.Resources.errorMessage_DefaultMessage;

        static void Main(string[] args)
        {
            try
            {
                Notifier = new XSNotifier();
                Serviceinfo = new Classes.ServiceInfo();
                notificationHandler = new Classes.NotificationHandler();
                Notifier.SendNotification(new XSNotification()
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