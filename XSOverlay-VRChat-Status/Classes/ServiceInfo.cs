using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cachet.NET;
using XSNotifications;
using XSNotifications.Enum;
using XSNotifications.Helpers;

// This class gets all the information from the VRChat Status API https://status.vrchat.com/api/v1
namespace XSOverlay_VRChat_Status.Classes
{
    class ServiceInfo : Program
    {
        private int _sdkstatus, _socialstatus, _authstatus, _networkingstatus, _statechangestatus;
        public int SDKStatus
        {
            get { return _sdkstatus; }
        }
        public int SocialStatus
        {
            get { return _socialstatus; }
        }
        public int AuthStatus
        {
            get { return _authstatus; }
        }
        public int NetworkingStatus
        {
            get { return _networkingstatus; }
        }
        public int StateChangesStatus
        {
            get { return _statechangestatus; }
        }

        public bool VRChatRunning()
        {
            if (Process.GetProcessesByName("VRChat").Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void getStatusses()
        {
                if(VRChatRunning())
                { 
                    var Cachet = new Cachet.NET.Cachet("https://status.vrchat.com/api/v1/");
                    try
                    {
                        var IsPingValid = Cachet.Ping();
                        if (IsPingValid)
                        {
                            try
                            {
                                var networkingstatus = Cachet.GetComponent(1);
                                _networkingstatus = Convert.ToInt16(networkingstatus.Component.Status);

                                var authentication = Cachet.GetComponent(2);
                                _authstatus = Convert.ToInt16(authentication.Component.Status);

                                var SDK = Cachet.GetComponent(3);
                                _sdkstatus = Convert.ToInt16(SDK.Component.Status);

                                var Social = Cachet.GetComponent(4);
                                _socialstatus = Convert.ToInt16(Social.Component.Status);

                                var Statechanges = Cachet.GetComponent(5);
                                _statechangestatus = Convert.ToInt16(Statechanges.Component.Status);

                                notificationHandler.checkForChanges();
                            } catch (Exception e)
                            {
                                Log("[ERROR] Failed to check for VRChat API status. Error: " + e);
                                noConnection();
                            }
                        }
                        else
                        {
                            Log("[ERROR] Failed to check for VRChat API status. Error: Access denied or the server isn't responding. It is recommended to close the program and wait for 10 minutes.");
                            noConnection();
                        }
                    }
                    catch (Exception e)
                    {
                        Log("[ERROR] Failed to check for VRChat API status. Error: " + e);
                        noConnection();
                    }
                }
        }

        public void noConnection()
        {
            _networkingstatus = 1;
            _authstatus = 1;
            _sdkstatus = 1;
            _socialstatus = 1;
            _statechangestatus = 1;

            if(noConnectionamount >= 2)
            {
                Log("[ERROR] Tried connecting to VRChat API for over 2 times. The software will now be in an inactive state. Restart to reconnect.");
                Notifier.SendNotification(new XSNotification()
                {
                    AudioPath = XSGlobals.GetBuiltInAudioSourceString(XSAudioDefault.Error),
                    Title = Program.errorMessageTitle,
                    Content = $"Tried 2 times... Program will now be closed to prevent any spam.",
                    Height = 110.0f
                });

                Program.prepareShutdown();
            } else
            {
                Notifier.SendNotification(new XSNotification()
                {
                    AudioPath = XSGlobals.GetBuiltInAudioSourceString(XSAudioDefault.Error),
                    Title = Program.errorMessageTitle,
                    Content = Program.errorMessageDefaultMessage,
                    Height = 110.0f
                });
                noConnectionamount++;
            }
        }
    }
}
