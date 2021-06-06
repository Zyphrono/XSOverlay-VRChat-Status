using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace XSOverlay_VRChat_Status.Classes
{
    class Debug
    {
        public static void resetSettings()
        {
            Properties.Settings.Default.launchMinimised = false;
            Classes.NotificationHandler.mnuMinimiseStartup.Checked = false;
            Properties.Settings.Default.firstTimeLaunch = true;
            Classes.StartupManager.RemoveFromStartup();
            if (Classes.StartupManager.IsInStartup() == false)
            {
                Classes.NotificationHandler.mnuStartup.Checked = false;
                Properties.Settings.Default.windowsStartup = false;
            }
            Properties.Settings.Default.Save();
            MessageBox.Show("Settings have been reset. The application will now close.", "Settings Reset", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            Environment.Exit(0);
        }
    }
}
