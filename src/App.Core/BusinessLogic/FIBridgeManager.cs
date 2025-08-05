using App.Core.DataTransferObjects;
using App.Core.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.BusinessLogic
{
    public class FIBridgeManager : RequestResponseEntryManager
    {
        public static async Task<GetAccountDetailsResponse> GetAccountDetailsAsync(GetAccountDetailsRequest getAccountDetailsRequest, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            GetAccountDetailsResponse getAccountDetailsResponse = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableFIBridge").ToUpper().Equals("N"))
                {
                    getAccountDetailsResponse = new GetAccountDetailsResponse
                    {
                        CustomerId = "396012",
                        AccountNumber = "3075666588",
                        AccountName = "JAMIU ISMAIL",
                        AccountType = "SAVINGS",
                        FreezeCode = "",
                        ProductCode = "SA301",
                        Product = "SAVINGS A/C-PERSONAL",
                        AccountStatus = "A",
                        CurrencyCode = "NGN",
                        BranchCode = "375",
                        Branch = "ZURU BRANCH",
                        BookBalance = "1600515.23",
                        AvailableBalance = "1600115.23",
                        LienAmount = "400",
                        UnclearedBalance = "0",
                        MobileNo = "2348031338899",
                        Email = "",
                        RelationshipManagerId = "",
                        RequestId = "7d4558135efb4fedb524ba1a5127a093",
                        ResponseCode = "00",
                        ResponseMessage = "Successful"
                    };

                    return getAccountDetailsResponse;
                }

                var requestUrl = $"{ConfigurationUtility.GetAppSettingValue("FIBridgeServiceUrl")}/api/v1/account/get-account-details";

                var restRequest = new RestRequest
                {
                    Method = Method.Post
                };


                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("AppId", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppId")}");
                restRequest.AddHeader("AppKey", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppKey")}");
                restRequest.AddBody(JsonConvert.SerializeObject(getAccountDetailsRequest), "application/json");

                //log request
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Account Details Call with request id >> {getAccountDetailsRequest?.RequestId} | Request >> {JsonConvert.SerializeObject(getAccountDetailsRequest)}");

                //
                var response = await PushOutRequestAsync(requestUrl, restRequest, callerFormName, callerFormMethod, callerIpAddress);

                //log response
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Account Details Call with request id >> {getAccountDetailsRequest?.RequestId} | Response >> {JsonConvert.SerializeObject(response)}");

                //get result
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        getAccountDetailsResponse = new GetAccountDetailsResponse();
                        getAccountDetailsResponse = JsonConvert.DeserializeObject<GetAccountDetailsResponse>(response.Content);
                    }
                    catch (Exception ex)
                    {
                        LogUtility.LogError(callerFormName, callerFormMethod + "|" + "JsonConvert.DeserializeObject", callerIpAddress, ex);
                    }
                }
                else
                {
                    LogUtility.LogError(callerFormName, callerFormMethod + "|" + "response.StatusCode != HttpStatusCode.OK", callerIpAddress, JsonConvert.SerializeObject(response));
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return getAccountDetailsResponse;
        }
        public static async Task<GetCustomerDetailsResponse> GetCustomerDetailsAsync(GetCustomerDetailsRequest getCustomerDetailsRequest, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            GetCustomerDetailsResponse getCustomerDetailsResponse = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableFIBridge").ToUpper().Equals("N"))
                {
                    getCustomerDetailsResponse = new GetCustomerDetailsResponse
                    {
                        CustomerId = "6672100",
                        Title = "MR.",
                        Gender = "M",
                        FirstName = "SAMUEL",
                        MiddleName = "TOKE",
                        LastName = "DAVID",
                        CustomerCategory = "",
                        Address = "12 HOLLWAY WAY",
                        DateOfBirth = "1950-01-11T00:00:00Z",
                        MobileNo = "2348023000000",
                        Email = "joe@wast.com",
                        State = "LAGOS",
                        Country = "NIGERIA",
                        RequestId = "34a1e36461f44a1d9fdd21e63cfc3083",
                        ResponseCode = "00",
                        ResponseMessage = "Successful"
                    };

                    return getCustomerDetailsResponse;
                }

                var requestUrl = $"{ConfigurationUtility.GetAppSettingValue("FIBridgeServiceUrl")}/api/v1/customer/get-customer-details-with-account-number";

                var restRequest = new RestRequest
                {
                    Method = Method.Post
                };

                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("AppId", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppId")}");
                restRequest.AddHeader("AppKey", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppKey")}");
                restRequest.AddBody(JsonConvert.SerializeObject(getCustomerDetailsRequest), "application/json");

                //log request
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Customer Details Call with request id >> {getCustomerDetailsRequest?.RequestId} | Request >> {JsonConvert.SerializeObject(getCustomerDetailsRequest)}");

                //
                var response = await PushOutRequestAsync(requestUrl, restRequest, callerFormName, callerFormMethod, callerIpAddress);

                //log response
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Customer Details Call with request id >> {getCustomerDetailsRequest?.RequestId} | Response >> {JsonConvert.SerializeObject(response)}");

                //get result
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        getCustomerDetailsResponse = new GetCustomerDetailsResponse();
                        getCustomerDetailsResponse = JsonConvert.DeserializeObject<GetCustomerDetailsResponse>(response.Content);
                    }
                    catch (Exception ex)
                    {
                        LogUtility.LogError(callerFormName, callerFormMethod + "|" + "JsonConvert.DeserializeObject", callerIpAddress, ex);
                    }
                }
                else
                {
                    LogUtility.LogError(callerFormName, callerFormMethod + "|" + "response.StatusCode != HttpStatusCode.OK", callerIpAddress, JsonConvert.SerializeObject(response));
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return getCustomerDetailsResponse;
        }
        public static async Task<GetAccountRelationshipManagerResponse> GetAccountRelationshipManagerAsync(GetAccountRelationshipManagerRequest getAccountRelationshipManagerRequest, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            GetAccountRelationshipManagerResponse getAccountRelationshipManagerResponse = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableFIBridge").ToUpper().Equals("N"))
                {
                    getAccountRelationshipManagerResponse = new GetAccountRelationshipManagerResponse
                    {
                        RelationshipManagerId = "SN023750",
                        RelationshipManagerEmail = "SN023750@firstbanknigeria.com",
                        CustomerAccountName = "DURU SUNDAY",
                        CustomerAccountNumber = "2011815255",
                        RequestId = "212121229823734",
                        ResponseCode = "00",
                        ResponseMessage = "Successful"
                    };

                    return getAccountRelationshipManagerResponse;
                }

                var requestUrl = $"{ConfigurationUtility.GetAppSettingValue("FIBridgeServiceUrl")}/api/v1/account/get-account-relationship-manager";

                var restRequest = new RestRequest
                {
                    Method = Method.Post
                };

                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("AppId", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppId")}");
                restRequest.AddHeader("AppKey", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppKey")}");
                restRequest.AddBody(JsonConvert.SerializeObject(getAccountRelationshipManagerRequest), "application/json");

                //log request
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Account Relationship Manager Call with request id >> {getAccountRelationshipManagerRequest?.RequestId} | Request >> {JsonConvert.SerializeObject(getAccountRelationshipManagerRequest)}");

                //
                var response = await PushOutRequestAsync(requestUrl, restRequest, callerFormName, callerFormMethod, callerIpAddress);

                //log response
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Account Relationship Manager Call with request id >> {getAccountRelationshipManagerRequest?.RequestId} | Response >> {JsonConvert.SerializeObject(response)}");

                //get result
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        getAccountRelationshipManagerResponse = new GetAccountRelationshipManagerResponse();
                        getAccountRelationshipManagerResponse = JsonConvert.DeserializeObject<GetAccountRelationshipManagerResponse>(response.Content);
                    }
                    catch (Exception ex)
                    {
                        LogUtility.LogError(callerFormName, callerFormMethod + "|" + "JsonConvert.DeserializeObject", callerIpAddress, ex);
                    }
                }
                else
                {
                    LogUtility.LogError(callerFormName, callerFormMethod + "|" + "response.StatusCode != HttpStatusCode.OK", callerIpAddress, JsonConvert.SerializeObject(response));
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return getAccountRelationshipManagerResponse;
        }
        public static async Task<GetCustomerAccountsResponse> GetCustomerAccountAsync(GetCustomerAccountsRequest getCustomerAccountsRequest, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            GetCustomerAccountsResponse getCustomerAccountsResponse = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableFIBridge").ToUpper().Equals("N"))
                {
                    getCustomerAccountsResponse = new GetCustomerAccountsResponse();
                    getCustomerAccountsResponse = JsonConvert.DeserializeObject<GetCustomerAccountsResponse>(@"
                    {
                    'Accounts': 
                    [
                        {
                            'CustomerId': '23041***',
                            'AccountNumber': '313267***',
                            'AccountName': 'JOHN PAUL',
                            'AccountType': 'SAVINGS',
                            'IsCreditFrozen': false,
                            'ProductCode': 'SA302',
                            'Product': 'SAVINGS A/C-STAFF',
                            'AccountStatus': 'D',
                            'CurrencyCode': 'NGN',
                            'BranchCode': '230',
                            'Branch': null,
                            'BookBalance': 0,
                            'AvailableBalance': 0,
                            'LienAmount': 0,
                            'UnclearedBalance': 0,
                            'IsAccountActive': false,
                            'IsCardAccount': false,
                            'MobileNo': '2348023*********',
                            'Email': 'test@yahoo.com',
                            'RelationshipManagerId': 'SN024247'
                        },
                        {
                            'CustomerId': '23041******',
                            'AccountNumber': '2033*******',
                            'AccountName': 'JOHN PAUL',
                            'AccountType': 'CURRENT',
                            'IsCreditFrozen': false,
                            'ProductCode': 'CA202',
                            'Product': 'CURRENT A/C - STAFF',
                            'AccountStatus': 'A',
                            'CurrencyCode': 'NGN',
                            'BranchCode': '230',
                            'Branch': null,
                            'BookBalance': 409642.22,
                            'AvailableBalance': 409642.22,
                            'LienAmount': 0,
                            'UnclearedBalance': 0,
                            'IsAccountActive': true,
                            'IsCardAccount': false,
                            'MobileNo': '2348023*********',
                            'Email': 'test@yahoo.com',
                            'RelationshipManagerId': 'SN026475'
                        }
                     ],
                    'RequestId': '2121212c12',
                    'ResponseCode': '00',
                    'ResponseMessage': 'Successful'
                    }");

                    return getCustomerAccountsResponse;
                }

                var requestUrl = $"{ConfigurationUtility.GetAppSettingValue("FIBridgeServiceUrl")}/api/v1/customer/get-customer-accounts";

                var restRequest = new RestRequest
                {
                    Method = Method.Post
                };

                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("AppId", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppId")}");
                restRequest.AddHeader("AppKey", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppKey")}");
                restRequest.AddBody(JsonConvert.SerializeObject(getCustomerAccountsRequest), "application/json");
                
                //log request
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Customer Accounts Call with request id >> {getCustomerAccountsRequest?.RequestId} | Request >> {JsonConvert.SerializeObject(getCustomerAccountsRequest)}");

                //
                var response = await PushOutRequestAsync(requestUrl, restRequest, callerFormName, callerFormMethod, callerIpAddress);

                //log response
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Customer Accounts Call with request id >> {getCustomerAccountsRequest?.RequestId} | Response >> {JsonConvert.SerializeObject(response)}");

                //get result
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        getCustomerAccountsResponse = new GetCustomerAccountsResponse();
                        getCustomerAccountsResponse = JsonConvert.DeserializeObject<GetCustomerAccountsResponse>(response.Content);
                    }
                    catch (Exception ex)
                    {
                        LogUtility.LogError(callerFormName, callerFormMethod + "|" + "JsonConvert.DeserializeObject", callerIpAddress, ex);
                    }
                }
                else
                {
                    LogUtility.LogError(callerFormName, callerFormMethod + "|" + "response.StatusCode != HttpStatusCode.OK", callerIpAddress, JsonConvert.SerializeObject(response));
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return getCustomerAccountsResponse;
        }
        public static async Task<GetAccountNameResponse> GetAccountNameAsync(GetAccountNameRequest getAccountNameRequest, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            GetAccountNameResponse getAccountNameResponse = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableFIBridge").ToUpper().Equals("N"))
                {
                    getAccountNameResponse = new GetAccountNameResponse
                    {
                        AccountNumber = $"{getAccountNameRequest?.AccountNumber}",
                        AccountName = "JOHN BULL",
                        RequestId = "7d4558135efb4fedb524ba1a5127a093",
                        ResponseCode = "00",
                        ResponseMessage = "Successful"
                    };

                    return getAccountNameResponse;
                }

                var requestUrl = $"{ConfigurationUtility.GetAppSettingValue("FIBridgeServiceUrl")}/api/v1/enquiry/name-enquiry";

                var restRequest = new RestRequest
                {
                    Method = Method.Post
                };


                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("AppId", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppId")}");
                restRequest.AddHeader("AppKey", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppKey")}");
                restRequest.AddBody(JsonConvert.SerializeObject(getAccountNameRequest), "application/json");

                //log request
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Account Name Call with request id >> {getAccountNameRequest?.RequestId} | Request >> {JsonConvert.SerializeObject(getAccountNameRequest)}");

                //
                var response = await PushOutRequestAsync(requestUrl, restRequest, callerFormName, callerFormMethod, callerIpAddress);

                //log response
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Account Name Call with request id >> {getAccountNameRequest?.RequestId} | Response >> {JsonConvert.SerializeObject(response)}");

                //get result
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        getAccountNameResponse = new GetAccountNameResponse();
                        getAccountNameResponse = JsonConvert.DeserializeObject<GetAccountNameResponse>(response?.Content);
                    }
                    catch (Exception ex)
                    {
                        LogUtility.LogError(callerFormName, callerFormMethod + "|" + "JsonConvert.DeserializeObject", callerIpAddress, ex);
                    }
                }
                else
                {
                    LogUtility.LogError(callerFormName, callerFormMethod + "|" + "response.StatusCode != HttpStatusCode.OK", callerIpAddress, JsonConvert.SerializeObject(response));
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return getAccountNameResponse;
        }
        public static async Task<GetBvnWithAccountNumberResponse> GetBvnWithAccountNumberAsync(GetBvnWithAccountNumberRequest getBvnWithAccountNumberRequest, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            GetBvnWithAccountNumberResponse getBvnWithAccountNumberResponse = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableFIBridge").ToUpper().Equals("N"))
                {
                    getBvnWithAccountNumberResponse = new GetBvnWithAccountNumberResponse
                    {
                        AccountNumber = $"{getBvnWithAccountNumberRequest?.AccountNumber}",
                        CifId = "",
                        CustomerId = "230410694",
                        Bvn = "22142784129",
                        RequestId = "7d4558135efb4fedb524ba1a5127a093",
                        ResponseCode = "00",
                        ResponseMessage = "Successful"
                    };

                    return getBvnWithAccountNumberResponse;
                }

                var requestUrl = $"{ConfigurationUtility.GetAppSettingValue("FIBridgeServiceUrl")}/api/v1/account/get-bvn-with-account-number";

                var restRequest = new RestRequest
                {
                    Method = Method.Post
                };


                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("AppId", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppId")}");
                restRequest.AddHeader("AppKey", $"{ConfigurationUtility.GetAppSettingValue("FIBridgeAppKey")}");
                restRequest.AddBody(JsonConvert.SerializeObject(getBvnWithAccountNumberRequest), "application/json");

                //log request
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Bvn with Account Number Call with request id >> {getBvnWithAccountNumberRequest?.RequestId} | Request >> {JsonConvert.SerializeObject(getBvnWithAccountNumberRequest)}");

                //
                var response = await PushOutRequestAsync(requestUrl, restRequest, callerFormName, callerFormMethod, callerIpAddress);

                //log response
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"Get Bvn with Account Number Call with request id >> {getBvnWithAccountNumberRequest?.RequestId} | Response >> {JsonConvert.SerializeObject(response)}");

                //get result
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        getBvnWithAccountNumberResponse = new GetBvnWithAccountNumberResponse();
                        getBvnWithAccountNumberResponse = JsonConvert.DeserializeObject<GetBvnWithAccountNumberResponse>(response.Content);
                    }
                    catch (Exception ex)
                    {
                        LogUtility.LogError(callerFormName, callerFormMethod + "|" + "JsonConvert.DeserializeObject", callerIpAddress, ex);
                    }
                }
                else
                {
                    LogUtility.LogError(callerFormName, callerFormMethod + "|" + "response.StatusCode != HttpStatusCode.OK", callerIpAddress, JsonConvert.SerializeObject(response));
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return getBvnWithAccountNumberResponse;
        }
    }
}
