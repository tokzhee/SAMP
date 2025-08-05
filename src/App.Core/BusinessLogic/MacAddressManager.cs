using System.Net.NetworkInformation;

namespace App.Core.BusinessLogic
{
    public static class MacAddressManager
    {
        public static string GetClientComputerMacAddress()
        {
            var clientMacAddress = "";

            try
            {
                foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (networkInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        clientMacAddress += networkInterface.GetPhysicalAddress().ToString();
                        break;
                    }
                }
            }
            catch
            {
                clientMacAddress = "";
            }

            return clientMacAddress;
        }
    }
}
