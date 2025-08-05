using App.Core.Utilities;
using Newtonsoft.Json;
using System;

namespace App.Core.BusinessLogic
{
    public static class TokenManager
    {
        public static bool AuthenticateUserToken(string username, string token, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var result = false;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableToken").ToUpper().Equals("N"))
                {
                    result = true;
                    return result;
                }

                var entrustServiceClient = new EntrustServiceClient.AuthWrapper();
                var entrustServiceUrl = ConfigurationUtility.GetAppSettingValue("EntrustServiceUrl");
                if (!string.IsNullOrEmpty(entrustServiceUrl)) entrustServiceClient.Url = entrustServiceUrl;

                BypassManager.BypassCertificateError();
                var response = entrustServiceClient.AuthMethod(new EntrustServiceClient.AuthRequest
                {
                    CustID = username,
                    PassCode = token
                });

                if (response.Authenticated) result = true;

                //
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Token Authentication for User >> {username} >> | Response >> {JsonConvert.SerializeObject(response)} | Result >> {result}");
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
    }
}
