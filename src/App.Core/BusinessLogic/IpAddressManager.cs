using System.Web;

namespace App.Core.BusinessLogic
{
    public static class IpAddressManager
    {
        public static string GetClientComputerIpAddress()
        {
            var clientIpAddress = "";
            try
            {
                clientIpAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(clientIpAddress))
                {
                    clientIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch
            {
                clientIpAddress = "0.0.0.0.0";
            }

            return clientIpAddress;
        }
    }
}
