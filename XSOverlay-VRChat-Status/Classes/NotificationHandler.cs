using System;
using System.Threading;
using System.Windows.Forms;
using XSNotifications;
using XSNotifications.Enum;
using XSNotifications.Helpers;

namespace XSOverlay_VRChat_Status.Classes
{
    class NotificationHandler : Program
    {
        private int currentsdkstatus = 1;
        private int currentsocialstatus = 1;
        private int currentauthstatus = 1;
        private int currentnetworkingstatus = 1;
        private int currentstatechangestatus = 1;
        public static int minimizeMode;

        public static ContextMenu menu;
        public static MenuItem mnuExit, mnuStartup, mnuMinimiseStartup, mnuChangelog, mnuForCheckUpdates, mnuSettings, mnuResetSettings;
        public static NotifyIcon notificationIcon;

        public void checkForChanges()
        {
            if(Program.Serviceinfo.AuthStatus != currentauthstatus)
            {
                if (Program.Serviceinfo.AuthStatus != 0)
                {
                    currentauthstatus = Program.Serviceinfo.AuthStatus;
                    sendAlert("Authentication", currentauthstatus);
                }
            }

            if (Program.Serviceinfo.SDKStatus != currentsdkstatus)
            {
                if (Program.Serviceinfo.SDKStatus != 0)
                {
                    currentsdkstatus = Program.Serviceinfo.SDKStatus;
                    sendAlert("SDK Uploads", currentsdkstatus);
                }
            }

            if (Program.Serviceinfo.SocialStatus != currentsocialstatus)
            {
                if (Program.Serviceinfo.SocialStatus != 0)
                {
                    currentsocialstatus = Program.Serviceinfo.SocialStatus;
                    sendAlert("Social", currentsocialstatus);
                }
            }

            if (Program.Serviceinfo.NetworkingStatus != currentnetworkingstatus)
            {
                if (Program.Serviceinfo.NetworkingStatus != 0)
                {
                    currentnetworkingstatus = Program.Serviceinfo.NetworkingStatus;
                    sendAlert("Realtime Networking", currentnetworkingstatus);
                }
            }

            if (Program.Serviceinfo.StateChangesStatus != currentstatechangestatus)
            {
                if (Program.Serviceinfo.StateChangesStatus != 0)
                {
                    currentstatechangestatus = Program.Serviceinfo.StateChangesStatus;
                    sendAlert("Player State Changes", Program.Serviceinfo.StateChangesStatus);
                }
            }
        }

        public void sendAlert(string serviceName, int status)
        {
            if (status != 1)
            {
                Log("[STATUS] Service " + serviceName + " is experiencing some difficulties.", System.ConsoleColor.Yellow);
                Notifier.SendNotification(new XSNotification
                {
                    Icon = XSGlobals.GetBuiltInIconTypeString(XSIconDefaults.Warning),
                    AudioPath = XSGlobals.GetBuiltInAudioSourceString(XSAudioDefault.Warning),
                    Title = "VRChat Status: " + serviceName,
                    Content = $"This service is currently experiencing some difficulties.",
                    Height = 110.0f
                });
            } else
            {
                Log("[STATUS] Service " + serviceName + " is back up and running.", System.ConsoleColor.Yellow);
                Notifier.SendNotification(new XSNotification
                {
                    AudioPath = XSGlobals.GetBuiltInAudioSourceString(XSAudioDefault.Default),
                    Title = "VRChat Status: " + serviceName,
                    Content = $"This service is back up and running.",
                    Height = 110.0f
                });
            }
        }

        // Windows Tray Notifications

        public static void createNotifyMenu()
        {
            Thread notifyThread = new Thread(
            delegate ()
            {
                menu = new ContextMenu();
                mnuExit = new MenuItem("Exit");
                mnuSettings = new MenuItem("Settings");
                mnuStartup = new MenuItem("Run on Windows startup");
                mnuMinimiseStartup = new MenuItem("Launch Minimised");
                mnuResetSettings = new MenuItem("Reset Settings");
                mnuChangelog = new MenuItem("Open Changelog");
                mnuForCheckUpdates = new MenuItem("Check for updates");

                mnuSettings.MenuItems.Add(0, mnuStartup);
                mnuSettings.MenuItems.Add(1, mnuMinimiseStartup);
                mnuSettings.MenuItems.Add(2, mnuResetSettings);
                menu.MenuItems.Add(0, mnuSettings);
                menu.MenuItems.Add(1, mnuChangelog);
                menu.MenuItems.Add(2, mnuForCheckUpdates);
                menu.MenuItems.Add(3, mnuExit);

                notificationIcon = new NotifyIcon
                {
                    Icon = Properties.Resources.softwareIcon,
                    ContextMenu = menu,
                    Text = "VRChat Status"
                };
                if(Properties.Settings.Default.windowsStartup == true)
                {
                    mnuStartup.Checked = true;
                }
                if (Properties.Settings.Default.launchMinimised == true)
                {
                    mnuMinimiseStartup.Checked = true;
                }
                notificationIcon.MouseClick += notificationIcon_MouseClick;
                mnuExit.Click += mnuExit_Click;
                mnuChangelog.Click += mnuChangelog_Click;
                mnuStartup.Click += mnuStartup_Click;
                mnuMinimiseStartup.Click += mnuMinimiseStartup_Click;
                mnuForCheckUpdates.Click += mnuCheckForUpdates_Click;
                mnuResetSettings.Click += mnuResetSettings_Click;
                mnuForCheckUpdates.Enabled = false;
                notificationIcon.Visible = true;
                Application.Run();
            }
        );

            notifyThread.Start();
        }
        static void notificationIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (minimizeMode == 1) { 
                    toggleWindow(1);
                    Log("");
                    Log("The console window is now unhidden. You can simply rehide it by reclicking on the tray icon.");
                    minimizeMode = 0;
                } else
                {
                    toggleWindow(0);
                    minimizeMode = 1;
                }
            }
        }
        static void mnuChangelog_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/KnuffelBeestje/XSOverlay-VRChat-Status/releases/");
        }

        static void mnuResetSettings_Click(object sender, EventArgs e)
        {
            mnuResetSettings.Enabled = false;
            mnuForCheckUpdates.Enabled = false;
            mnuExit.Enabled = false;
            mnuChangelog.Enabled = false;
            mnuStartup.Enabled = false;
            mnuSettings.Enabled = false;
            Debug.resetSettings();

        }

        static void mnuCheckForUpdates_Click(object sender, EventArgs e)
        {
            checkForUpdates();
        }
        static void mnuMinimiseStartup_Click(object sender, EventArgs e)
        {
            if(Properties.Settings.Default.launchMinimised == false) {
                Properties.Settings.Default.launchMinimised = true;
                mnuMinimiseStartup.Checked = true;
            } else
            {
                Properties.Settings.Default.launchMinimised = false;
                mnuMinimiseStartup.Checked = false;
            }
            Properties.Settings.Default.Save();
        }

        static void mnuStartup_Click(object sender, EventArgs e)
        {
            if(Properties.Settings.Default.windowsStartup == false)
            {
                Classes.StartupManager.RunOnStartup();
                if (Classes.StartupManager.IsInStartup() == true)
                {
                    mnuStartup.Checked = true;
                    Properties.Settings.Default.windowsStartup = true;
                } else
                {
                    MessageBox.Show("An error occoured while enabling launch on Windows startup. Try restarting the application as an administrator.", "VRChat Status Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else
            {
                Classes.StartupManager.RemoveFromStartup();
                if (Classes.StartupManager.IsInStartup() == false)
                {
                    mnuStartup.Checked = false;
                    Properties.Settings.Default.windowsStartup = false;
                }
                else
                {
                    MessageBox.Show("An error occoured while disabling launch on Windows startup. Try restarting the application as an administrator.", "VRChat Status Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Properties.Settings.Default.Save();
        }
        static void mnuExit_Click(object sender, EventArgs e)
        {
            notificationIcon.Dispose();
            checkTimer.Dispose();
            Notifier.Dispose();
            Environment.Exit(0);
        }
    }
}
