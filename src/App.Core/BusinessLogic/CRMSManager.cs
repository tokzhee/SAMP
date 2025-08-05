using App.Core.DataTransferObjects;
using App.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace App.Core.BusinessLogic
{
    public class CRMSManager
    {
        private readonly string serviceUrl = "";
        private readonly CRMSServiceClient.ReturnSubmissionWSEJBBeanService service;
        private readonly XmlSerializerUtility xmlSerializerUtility;

        public CRMSManager()
        {
            serviceUrl = ConfigurationUtility.GetAppSettingValue("CRMSServiceUrl");
            service = new CRMSServiceClient.ReturnSubmissionWSEJBBeanService();
            xmlSerializerUtility = new XmlSerializerUtility();
        }

        public CRMSValidateBvnResponse ValidateBvn(string bvn, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            CRMSValidateBvnResponse validateBvnResponse = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableCRMS").ToUpper().Equals("Y"))
                {
                    if (!string.IsNullOrEmpty(serviceUrl))
                    {
                        service.Url = serviceUrl;
                    }

                    BypassManager.BypassCertificateError();
                    var xmlResult = service.validateBVN(bvn);

                    //
                    LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"CRMS BVN Validation for BVN >> {bvn} >> | Response >> {xmlResult}");

                    //
                    validateBvnResponse = xmlSerializerUtility.Deserialize<CRMSValidateBvnResponse>(xmlResult, callerFormName, callerFormMethod, callerIpAddress);
                }
                else
                {
                    validateBvnResponse = new CRMSValidateBvnResponse
                    {
                        ResponseCode = "00",
                        BVN = "",
                        FirstName = "",
                        MiddleName = "",
                        LastName = "",
                        DateOfBirth = "",
                        PhoneNumber1 = "",
                        PhoneNumber2 = "",
                        Gender = "",
                        ResidentialAddress = "",
                    };
                }

                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"CRMS BVN Validation for BVN >> {bvn} >> | Response >> {JsonConvert.SerializeObject(validateBvnResponse)}");
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return validateBvnResponse;
        }
        public CRMSCreditCheckResponse CreditCheck(string bvn, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            CRMSCreditCheckResponse creditCheckResponse = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableCRMS").ToUpper().Equals("Y"))
                {
                    if (!string.IsNullOrEmpty(serviceUrl))
                    {
                        service.Url = serviceUrl;
                    }

                    BypassManager.BypassCertificateError();
                    var xmlResult = service.creditCheck(bvn);

                    //
                    LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"CRMS Credit Check for BVN >> {bvn} >> | Response >> {xmlResult}");

                    //
                    creditCheckResponse = xmlSerializerUtility.Deserialize<CRMSCreditCheckResponse>(xmlResult, callerFormName, callerFormMethod, callerIpAddress);
                }
                else
                {
                    creditCheckResponse = xmlSerializerUtility.Deserialize<CRMSCreditCheckResponse>("<CreditCheck><Credit><CRMSRefNumber>00011/20220504/72776147</CRMSRefNumber><CreditType>Personal Loan</CreditType><CreditLimit>15000</CreditLimit><OutstandingAmount>15000</OutstandingAmount><EffectiveDate>04-05-2022</EffectiveDate><Tenor>12</Tenor><ExpiryDate>04-05-2023</ExpiryDate><GrantingInstitution>First Bank Plc</GrantingInstitution><PerformanceStatus>GOOD</PerformanceStatus></Credit><Summary>Total Number of Credits: 1 | Total Number of Performing Credits: 1 | Total Number of Non-Performing Credits: 0</Summary></CreditCheck>", callerFormName, callerFormMethod, callerIpAddress);
                }

                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"CRMS Credit Check for BVN >> {bvn} >> | Response >> {JsonConvert.SerializeObject(creditCheckResponse)}");
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return creditCheckResponse;
        }
        public string CreditCheckSummary(string bvn, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var result = "";

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableCRMS").ToUpper().Equals("Y"))
                {
                    if (!string.IsNullOrEmpty(serviceUrl))
                    {
                        service.Url = serviceUrl;
                    }

                    BypassManager.BypassCertificateError();
                    result = service.creditCheckSummary(bvn);

                    //
                    LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"CRMS Credit Check Summary for BVN >> {bvn} >> | Response >> {result}");
                }
                else
                {
                    result = "Total Number of Credits: 1 | Total Number of Performing Credits: 1 | Total Number of Non-Performing Credits: 0";
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

