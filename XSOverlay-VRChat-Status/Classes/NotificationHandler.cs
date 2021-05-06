using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSNotifications;
using XSNotifications.Enum;
using XSNotifications.Helpers;

namespace XSOverlay_VRChat_Status.Classes
{
    class NotificationHandler : Program
    {
        private int currentsdkstatus, currentsocialstatus, currentauthstatus, currentnetworkingstatus, currentstatechangestatus = 1;

        public void checkForChanges()
        {
            if(Program.Serviceinfo.AuthStatus != currentauthstatus)
            {
                currentauthstatus = Program.Serviceinfo.AuthStatus;

                sendAlert("Authentication", Program.Serviceinfo.AuthStatus);
            }

            if (Program.Serviceinfo.SDKStatus != currentsdkstatus)
            {
                currentsdkstatus = Program.Serviceinfo.SDKStatus;
                sendAlert("SDK Uploads", Program.Serviceinfo.SDKStatus);

            }

            if (Program.Serviceinfo.SocialStatus != currentsocialstatus)
            {
                currentsocialstatus = Program.Serviceinfo.SocialStatus;
                sendAlert("Social", Program.Serviceinfo.SocialStatus);

            }

            if (Program.Serviceinfo.NetworkingStatus != currentnetworkingstatus)
            {
                currentnetworkingstatus = Program.Serviceinfo.NetworkingStatus;
                sendAlert("Realtime Networking", Program.Serviceinfo.NetworkingStatus);

            }

            if (Program.Serviceinfo.StateChangesStatus != currentstatechangestatus)
            {
                currentstatechangestatus = Program.Serviceinfo.StateChangesStatus;
                sendAlert("Player State Changes", Program.Serviceinfo.StateChangesStatus);

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
