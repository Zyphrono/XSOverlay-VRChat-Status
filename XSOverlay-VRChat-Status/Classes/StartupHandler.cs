using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XSOverlay_VRChat_Status.Classes
{
    class StartupHandler
    {
        public static void startupPrompt()
        {
            if(Properties.Settings.Default.firstTimeLaunch == true)
            {
                if ((Properties.Settings.Default.launchMinimised || Properties.Settings.Default.windowsStartup) != true)
                {
                    if (MessageBox.Show("Thank you for installing XSOverlay VRChat Status. Would you like to run this software on Windows startup (hidden)? This software needs to be active in order to receive notifications.", "Welcome!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Properties.Settings.Default.launchMinimised = true;
                        Classes.NotificationHandler.mnuMinimiseStartup.Checked = true;
                        Classes.StartupManager.RunOnStartup();
                        if (Classes.StartupManager.IsInStartup() == true)
                        {
                            Classes.NotificationHandler.mnuStartup.Checked = true;
                            Properties.Settings.Default.windowsStartup = true;
                        }
                        else
                        {
                            MessageBox.Show("An error occoured while enabling launch on Windows startup. Try restarting the application as an administrator.", "VRChat Status Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        Properties.Settings.Default.firstTimeLaunch = false;
                        Properties.Settings.Default.Save();
                    } else
                    {
                        Properties.Settings.Default.firstTimeLaunch = false;
                        Properties.Settings.Default.Save();
                    }
                }
            }
        }
    }
}
