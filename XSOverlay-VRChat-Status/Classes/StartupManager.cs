using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XSOverlay_VRChat_Status.Classes
{
    static class StartupManager
    {
        /// <summary>
        /// Adds this executable to the startup list.
        /// </summary>
        public static bool RunOnStartup()
        {
            return RunOnStartup(Application.ProductName, Application.ExecutablePath);
        }

        /// <summary>
        /// Adds the specified executable to the startup list.
        /// </summary>
        /// <param name="AppTitle">Registry key title.</param>
        /// <param name="AppPath">Path of executable to run on startup.</param>
        public static bool RunOnStartup(string AppTitle, string AppPath)
        {
            RegistryKey rk;
            try
            {
                rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                rk.SetValue(AppTitle, AppPath);
                return true;
            }
            catch (Exception)
            {
            }

            try
            {
                rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                rk.SetValue(AppTitle, AppPath);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Removes this executable from the startup list.
        /// </summary>
        public static bool RemoveFromStartup()
        {
            return RemoveFromStartup(Application.ProductName, Application.ExecutablePath);
        }

        /// <summary>
        /// Removes the specified executable from the startup list.
        /// </summary>
        /// <param name="AppTitle">Registry key title.</param>
        public static bool RemoveFromStartup(string AppTitle)
        {
            return RemoveFromStartup(AppTitle, null);
        }

        /// <summary>
        /// Removes the specified executable from the startup list.
        /// </summary>
        /// <param name="AppTitle">Registry key title.</param>
        /// <param name="AppPath">Path of executable in the registry that's being run on startup.</param>
        public static bool RemoveFromStartup(string AppTitle, string AppPath)
        {
            RegistryKey rk;
            try
            {
                rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (AppPath == null)
                {
                    rk.DeleteValue(AppTitle);
                }
                else
                {
                    if (rk.GetValue(AppTitle).ToString().ToLower() == AppPath.ToLower())
                    {
                        rk.DeleteValue(AppTitle);
                    }
                }
                return true;
            }
            catch (Exception)
            {
            }

            try
            {
                rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (AppPath == null)
                {
                    rk.DeleteValue(AppTitle);
                }
                else
                {
                    if (rk.GetValue(AppTitle).ToString().ToLower() == AppPath.ToLower())
                    {
                        rk.DeleteValue(AppTitle);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if this executable is in the startup list.
        /// </summary>
        /// <returns></returns>
        public static bool IsInStartup()
        {
            return IsInStartup(Application.ProductName, Application.ExecutablePath);
        }

        /// <summary>
        /// Checks if specified executable is in the startup list.
        /// </summary>
        /// <param name="AppTitle">Registry key title.</param>
        /// <param name="AppPath">Path of the executable.</param>
        /// <returns></returns>
        public static bool IsInStartup(string AppTitle, string AppPath)
        {
            RegistryKey rk;
            string value;

            try
            {
                rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                value = rk.GetValue(AppTitle).ToString();
                if (value == null)
                {
                    return false;
                }
                else if (!value.ToLower().Equals(AppPath.ToLower()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                log()
            }

            try
            {
                rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                value = rk.GetValue(AppTitle).ToString();
                if (value == null)
                {
                    return false;
                }
                else if (!value.ToLower().Equals(AppPath.ToLower()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }

            return false;
        }
    }
}
