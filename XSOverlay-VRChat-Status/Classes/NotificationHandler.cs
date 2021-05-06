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
                Log("[STATUS] Service " + serviceName + " is experiencing some difficulties.");
                Notifier.SendNotification(new XSNotification()
                {
                    AudioPath = XSGlobals.GetBuiltInAudioSourceString(XSAudioDefault.Warning),
                    Title = "VRChat Status: " + serviceName,
                    Content = $"This service is currently experiencing some difficulties.",
                    Height = 110.0f
                });
            } else
            {
                Log("[STATUS] Service " + serviceName + " is back up and running.");
                Notifier.SendNotification(new XSNotification()
                {
                    AudioPath = XSGlobals.GetBuiltInAudioSourceString(XSAudioDefault.Default),
                    Title = "VRChat Status: " + serviceName,
                    Content = $"This service is back up and running.",
                    Height = 110.0f
                });
            }
        }
    }
}
