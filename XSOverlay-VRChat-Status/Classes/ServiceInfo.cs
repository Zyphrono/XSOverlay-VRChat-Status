using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XSNotifications;
using XSNotifications.Enum;
using XSNotifications.Helpers;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

// This class gets all the information from the VRChat Status API https://status.vrchat.com/api/v1
namespace XSOverlay_VRChat_Status.Classes
{
    class ServiceInfo : Program
    {
        private int _sdkstatus = 1;
        private int _socialstatus = 1;
        private int _authstatus = 1;
        private int _networkingstatus = 1;
        private int _statechangestatus = 1;
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

        public static Classes.JsonStorage jsonStorage = new Classes.JsonStorage();

        public void getStatusses()
        {
                if(VRChatRunning())
                { 
                    try
                    {
                        var IsPingValid = true;
                        if (IsPingValid)
                        {
                        try
                        {
                                Task tasknetworking = ServiceInfo.GetServiceStatus(1);
                                tasknetworking.Wait();
                                if (jsonStorage.status != 0)
                                {
                                    _networkingstatus = jsonStorage.status;
                                }

                                Task taskauth = ServiceInfo.GetServiceStatus(2);
                                taskauth.Wait();
                                if (jsonStorage.status != 0) {
                                    _authstatus = jsonStorage.status;
                                }

                                Task taskSDK = ServiceInfo.GetServiceStatus(3);
                                taskSDK.Wait();
                                if (jsonStorage.status != 0)
                                {
                                    _sdkstatus = jsonStorage.status;
                                }

                                Task tasksocial = ServiceInfo.GetServiceStatus(4);
                                tasksocial.Wait();
                                if (jsonStorage.status != 0)
                                {
                                    _socialstatus = jsonStorage.status;
                                }

                                Task taskstatechange = ServiceInfo.GetServiceStatus(5);
                                taskstatechange.Wait();
                                if (jsonStorage.status != 0)
                                {
                                    _statechangestatus = jsonStorage.status;
                                }

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

        private static async Task GetServiceStatus(int CompID)
        {
            string apiurl = "https://status.vrchat.com/api/v1/components/" + CompID;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                var request = new HttpRequestMessage(HttpMethod.Get, apiurl);
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    JObject jsonObject2 = JObject.Parse(content);
                    jsonStorage.status = Convert.ToInt32(jsonObject2["data"]["status"].ToString());
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
