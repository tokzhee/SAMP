using App.Core.Utilities;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace App.Core.BusinessLogic
{
    public class RequestResponseEntryManager
    {
        private static RestClient GetRestClient(string requestUrl) => new RestClient(requestUrl);
        public static async Task<RestResponse> PushOutRequestAsync(string requestUrl, RestRequest restRequest, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            RestResponse restResponse = null;

            try
            {
                var restClient = GetRestClient(requestUrl);
                BypassManager.BypassCertificateError();
                restResponse = await restClient.ExecuteAsync(restRequest);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return restResponse;
        }
    }
}
