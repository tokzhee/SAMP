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
    public class CRCManager : RequestResponseEntryManager
    {
        public static async Task<CRCCreditCheckResponse> CreditCheckAsync(string bvn, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            CRCCreditCheckResponse creditCheckResponse = null;

            try
            {
                if (ConfigurationUtility.GetAppSettingValue("EnableCRC").ToUpper().Equals("N"))
                {
                    creditCheckResponse = new CRCCreditCheckResponse();
                    //creditCheckResponse = JsonConvert.DeserializeObject<CRCCreditCheckResponse>("{\"ConsumerNoHitResponse\":{\"BODY\":{\"NOHIT\":{\"DATAPACKET\":{\"ConsDisclaimer\":{\"GetDisclaimerContent\":{\"Item1\":\"1\"}},\"NoHit_CONSUMER_PROFILE\":{},\"NoHit_SUBJECT_DETAILS\":{\"SUBJECT_DETAILS\":{}}}}},\"HEADER\":{\"RESPONSETYPE\":{\"CODE\":\"2\",\"DESCRIPTION\":\"Response NoHit\"}},\"REQUESTID\":\"1\"}}");
                    creditCheckResponse = JsonConvert.DeserializeObject<CRCCreditCheckResponse>("{  \"ConsumerHitResponse\": {    \"BODY\": {      \"AddressHistory\": null,      \"Amount_OD_BucketCURR1\": null,      \"CONSUMER_RELATION\": {},      \"CREDIT_MICRO_SUMMARY\": null,      \"CREDIT_NANO_SUMMARY\": {        \"SUMMARY\": {          \"HAS_CREDITFACILITIES\": \"NO\",          \"LAST_REPORTED_DATE\": null,          \"NO_OF_DELINQCREDITFACILITIES\": \"0\"        }      },      \"CREDIT_SCORE_DETAILS\": null,      \"ClassificationInsType\": null,      \"ClassificationProdType\": null,      \"ClosedAccounts\": null,      \"ConsCommDetails\": null,      \"ConsumerMergerDetails\": null,      \"ContactHistory\": null,      \"CreditDisputeDetails\": null,      \"CreditFacilityHistory24\": null,      \"CreditProfileOverview\": null,      \"CreditProfileSummaryCURR1\": null,      \"CreditProfileSummaryCURR2\": null,      \"CreditProfileSummaryCURR3\": null,      \"CreditProfileSummaryCURR4\": null,      \"CreditProfileSummaryCURR5\": null,      \"DMMDisputeSection\": null,      \"DODishonoredChequeDetails\": null,      \"DOJointHolderDetails\": null,      \"DOLitigationDetails\": null,      \"DisclaimerDetails\": null,      \"EmploymentHistory\": null,      \"GuaranteedLoanDetails\": null,      \"InquiryHistoryDetails\": null,      \"Inquiry_Product\": null,      \"LegendDetails\": null,      \"MFCREDIT_MICRO_SUMMARY\": null,      \"MFCREDIT_NANO_SUMMARY\": {        \"SUMMARY\": {          \"HAS_CREDITFACILITIES\": \"YES\",          \"NO_OF_DELINQCREDITFACILITIES\": \"1\"        }      },      \"MGCREDIT_MICRO_SUMMARY\": null,      \"MGCREDIT_NANO_SUMMARY\": {        \"SUMMARY\": {          \"HAS_CREDITFACILITIES\": \"NO\",          \"NO_OF_DELINQCREDITFACILITIES\": \"0\"        }      },      \"MIC_CONSUMER_PROFILE\": null,      \"NANO_CONSUMER_PROFILE\": {        \"CONSUMER_DETAILS\": {          \"CITIZENSHIP\": \"NG\",          \"DATE_OF_BIRTH\": \"25-OCT-1984\",          \"FIRST_NAME\": \"BEAUTY\",          \"GENDER\": \"002\",          \"IDENTIFICATION\": [            {              \"EXP_DATE\": null,              \"EXP_DATESpecified\": false,              \"ID_DISPLAY_NAME\": \"Business Verification Number\",              \"ID_VALUE\": \"22227770649\",              \"RUID\": \"1112020004261013\",              \"SOURCE_ID\": \"BVN\"            }          ],          \"IDENTIFICATION_DETAILS\": null,          \"LAST_NAME\": \"ODIOR\",          \"NAME\": \"BEAUTY MOMOH ODIOR\",          \"RUID\": \"1112020004261013\"        }      },      \"RelatedToDetails\": null,      \"ReportDetail\": null,      \"ReportDetailAcc\": null,      \"ReportDetailBVN\": null,      \"ReportDetailMob\": null,      \"ReportDetailsSIR\": null,      \"SecurityDetails\": null,      \"SummaryOfPerformance\": null    },    \"HEADER\": {      \"REPORTHEADER\": {        \"MAILTO\": \"support@crccreditbureau.com\",        \"PRODUCTNAME\": \"Nano Consumer Report\",        \"REASON\": {},        \"REPORTDATE\": \"29/May/2022\",        \"REPORTORDERNUMBER\": \"W-0052817313/2022\",        \"USERID\": \"907991firstb1\"      },      \"RESPONSETYPE\": {        \"CODE\": \"1\",        \"DESCRIPTION\": \"Response DataPacket\"      },      \"SEARCHCRITERIA\": {        \"BRANCHCODE\": null,        \"BVN_NO\": \"22227770649\",        \"CFACCOUNTNUMBER\": null,        \"DATEOFBIRTH\": null,        \"GENDER\": null,        \"NAME\": null,        \"TELEPHONE_NO\": null      },      \"SEARCHRESULTLIST\": {        \"SEARCHRESULTITEM\": {          \"ADDRESSES\": {},          \"BUREAUID\": \"1112020004261013\",          \"CONFIDENCESCORE\": \"100\",          \"IDENTIFIERS\": {},          \"NAME\": \"BEAUTY MOMOH ODIOR\",          \"SURROGATES\": {}        }      }    },    \"REQUESTID\": \"1\"  }}");
                    return creditCheckResponse;
                }

                var requestUrl = $"{ConfigurationUtility.GetAppSettingValue("CRCServiceUrl")}/CreditRegistry/ProcessRequestJson?Bvn={bvn}";

                var restRequest = new RestRequest
                {
                    Method = Method.Post
                };

                restRequest.AddHeader("Accept", "*/*");

                //
                var response = await PushOutRequestAsync(requestUrl, restRequest, callerFormName, callerFormMethod, callerIpAddress);

                //log response
                LogUtility.LogInfo(callerFormName, callerFormMethod, callerIpAddress, $"CRC Credit Check for BVN >> {bvn} >> | Response >> {JsonConvert.SerializeObject(response)}");

                //get result
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        creditCheckResponse = new CRCCreditCheckResponse();
                        creditCheckResponse = JsonConvert.DeserializeObject<CRCCreditCheckResponse>(response.Content);
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

            return creditCheckResponse;
        }
    }
}
