using App.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.BusinessLogic
{
    public class NIBSSBvnValidatorManager
    {
        private readonly string serviceUrl = "";

        public NIBSSBvnValidatorManager()
        {
            serviceUrl = ConfigurationUtility.GetAppSettingValue("NIBSSBvnValidatorManager");
        }

        public BvnValidatorServiceClient.ValidatorResponse ValidateBvn(string bvn, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            BvnValidatorServiceClient.ValidatorResponse validatorResponse = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableBVNValidator").ToUpper().Equals("Y"))
                {
                    var serviceOnHttp = new BvnValidatorServiceClient.BasicHttpBinding_IService();
                    var serviceOnHttps = new BvnValidatorServiceClient.BasicHttpsBinding_IService();

                    if (!string.IsNullOrEmpty(serviceUrl))
                    {
                        serviceOnHttp.Url = serviceUrl;
                        serviceOnHttps.Url = serviceUrl;
                    }

                    BypassManager.BypassCertificateError();
                    validatorResponse = ConfigurationUtility.GetAppSettingValue("IsBVNValidatorServiceUrlOnHttps").ToUpper().Equals("Y") ? serviceOnHttps.ValidateBVN(bvn) : serviceOnHttp.ValidateBVN(bvn);

                    //
                    LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"NIBSS BVN Validation for BVN >> {bvn} >> | Response >> {JsonConvert.SerializeObject(validatorResponse)}");
                }
                else
                {
                    validatorResponse = new BvnValidatorServiceClient.ValidatorResponse
                    {
                        Result = new BvnValidatorServiceClient.NIBSSResponse
                        {
                            BVN = "22222222943",
                            DateOfBirth = "23-Jan-1995",
                            Email = "folsds0@ngfg.com",
                            FirstName = "Emmanuel",
                            Gender = "Male",
                            LastName = "Giroud",
                            LevelOfAccount = "Level 2 - Medium Level Accounts",
                            LgaOfOrigin = "Ijebu-Ode",
                            LgaOfResidence = "Agege",
                            MaritalStatus = "Single",
                            MiddleName = "Victor",
                            NIN = "771-757-9123",
                            NameOnCard = "Olivier Giroud",
                            Nationality = "Nigeria",
                            PhoneNumber1 = "08125489906",
                            PhoneNumber2 = "08184051129",
                            ResidentialAddress = "13 banti road Lekki Lagos",
                            StateOfOrigin = "Ekiti state",
                            StateOfResidence = "Lagos state",
                            Title = "Mr."
                        },
                        Status = new BvnValidatorServiceClient.Response
                        {
                            ResponseCode = "00",
                            ResponseMessage = "Successful"
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return validatorResponse;
        }
    }
}
