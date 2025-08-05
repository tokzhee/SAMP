using App.Core.Utilities;
using Newtonsoft.Json;
using System;

namespace App.Core.BusinessLogic
{
    public static class ActiveDirectoryManager
    {
        public static bool AuthenticateUser(string username, string password, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var result = false;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableAd").ToUpper().Equals("Y"))
                {
                    var serviceUrl = ConfigurationUtility.GetAppSettingValue("AdServiceUrl");
                    var service = new AdServiceClient.Service();
                    if (!string.IsNullOrEmpty(serviceUrl))
                    {
                        service.Url = serviceUrl;
                    }

                    BypassManager.BypassCertificateError();
                    var response = service.ADValidateUser(username, password);

                    //
                    LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Active Directory Authentication for User >> {username} >> | Response >> {JsonConvert.SerializeObject(response)} | Result >> {result}");

                    result = Convert.ToBoolean(response.Split('|').GetValue(0));

                    //
                    LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Active Directory Authentication for User >> {username} >> | Result >> {result}");
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static string GetUserDetails(string username, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var result = "";

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableAd").ToUpper().Equals("Y"))
                {
                    var serviceUrl = ConfigurationUtility.GetAppSettingValue("AdServiceUrl");
                    var service = new AdServiceClient.Service();
                    if (!string.IsNullOrEmpty(serviceUrl))
                    {
                        service.Url = serviceUrl;
                    }

                    BypassManager.BypassCertificateError();
                    var response = service.ADUserDetails(username);

                    //
                    LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Active Directory Get Details for User >> {username} >> | Response >> {JsonConvert.SerializeObject(response)} | Result >> {result}");

                    result = response;
                }
                else
                {
                    result = $"{username} {username}|{username}@firstbanknigeria.com";
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
    }
}
