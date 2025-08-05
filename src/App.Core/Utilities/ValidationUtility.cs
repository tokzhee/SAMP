using App.Core.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace App.Core.Utilities
{
    public static class ValidationUtility
    {
        public static bool IsValidTextInput(string input)
        {
            return !string.IsNullOrEmpty(input);
        }
        public static bool IsValidDecimalInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            
            var result = decimal.TryParse(input, out var response);
            return result;
        }
        public static bool IsValidDateFormat(string dateString, string dateFormat)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return false;
            }

            var result = DateTime.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var transactionDateTemp);
            return result;
        }
        public static bool IsValidMobileNumber(string mobileNumber)
        {
            if (mobileNumber.StartsWith("+234") || mobileNumber.StartsWith("234") || mobileNumber.Length != 11)
            {
                return false;
            }

            var r = new Regex(@"^\d{11}$");
            return !string.IsNullOrEmpty(mobileNumber) && r.IsMatch(mobileNumber);
        }
        public static bool IsValidEmail(string email)
        {
            var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
            return !string.IsNullOrEmpty(email) && r.IsMatch(email);
        }
        public static bool IsValidIntegerInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var result = int.TryParse(input, out int response);
            return result;
        }
        public static bool IsValidLongInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var result = long.TryParse(input, out long response);
            return result;
        }
        public static bool IsValidPasswordStrength(string password)
        {
            var r = new Regex(@"^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$");
            return !string.IsNullOrEmpty(password) && r.IsMatch(password);
        }
        public static bool IsComparableStrings(string input1, string input2)
        {
            return string.Compare(input1, input2).Equals(0);
        }
        public static bool IsValidBvnDetails(Dictionary<string, string> input1, Dictionary<string, string> input2)
        {
            try
            {
                bool snOK = false;
                if (!string.IsNullOrEmpty(input1["SN"]) && !string.IsNullOrEmpty(input2["SN"])) snOK = string.Equals(input1["SN"], input2["SN"], StringComparison.OrdinalIgnoreCase);

                bool fnOK = false;
                if (!string.IsNullOrEmpty(input1["FN"]) && !string.IsNullOrEmpty(input2["FN"])) fnOK = string.Equals(input1["FN"], input2["FN"], StringComparison.OrdinalIgnoreCase);

                bool gOK = false;
                if (!string.IsNullOrEmpty(input1["G"]) && !string.IsNullOrEmpty(input2["G"])) gOK = string.Equals(input1["G"], input2["G"], StringComparison.OrdinalIgnoreCase);

                bool dobOK = false;
                if (!string.IsNullOrEmpty(input1["DOB"]) && !string.IsNullOrEmpty(input2["DOB"]))
                {
                    DateTime input1DOB = Convert.ToDateTime(input1["DOB"]);
                    DateTime input2DOB = Convert.ToDateTime(input2["DOB"]);
                    dobOK = input1DOB.Day == input2DOB.Day && input1DOB.Month == input2DOB.Month && input1DOB.Year == input2DOB.Year;
                }

                return snOK && fnOK && gOK && dobOK;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool IsValidCRMSDetails(string crmsSummary)
        {
            //return string.IsNullOrEmpty(crmsSummary) || (!string.IsNullOrEmpty(crmsSummary) && crmsSummary.Contains("Total Number of Non-Performing Credits: 0"));
            return !string.IsNullOrEmpty(crmsSummary) && crmsSummary.Contains("Total Number of Non-Performing Credits: 0");
        }
        public static bool IsValidCRCDetails(CRCCreditCheckResponse creditCheckResponse, ref string summary)
        {
            var result = false;

            try
            {
                if (creditCheckResponse != null)
                {
                    if (creditCheckResponse.ConsumerNoHitResponse != null)
                    {
                        result = true;
                        summary = "NO HIT";
                    }

                    if (creditCheckResponse.ConsumerHitResponse != null)
                    {
                        var deliquentOnCREDITMICROSUMMARY = 0;
                        var deliquentOnCREDITNANOSUMMARY = 0;
                        var deliquentOnMFCREDITMICROSUMMARY = 0;
                        var deliquentOnMFCREDITNANOSUMMARY = 0;
                        var deliquentOnMGCREDITMICROSUMMARY = 0;
                        var deliquentOnMGCREDITNANOSUMMARY = 0;

                        var consumerHitResponse = creditCheckResponse.ConsumerHitResponse;
                        if (consumerHitResponse.BODY != null)
                        {
                            var body = consumerHitResponse.BODY;
                            if (body != null)
                            {
                                //
                                var sUMMARY = new SUMMARY();

                                //
                                var cREDITMICROSUMMARY = body.CREDIT_MICRO_SUMMARY;
                                if (cREDITMICROSUMMARY != null)
                                {
                                    sUMMARY = cREDITMICROSUMMARY.SUMMARY;
                                    if (sUMMARY != null) deliquentOnCREDITMICROSUMMARY = Convert.ToInt32(sUMMARY.NO_OF_DELINQCREDITFACILITIES);
                                }

                                //
                                var cREDITNANOSUMMARY = body.CREDIT_NANO_SUMMARY;
                                if (cREDITNANOSUMMARY != null)
                                {
                                    sUMMARY = cREDITNANOSUMMARY.SUMMARY;
                                    if (sUMMARY != null) deliquentOnCREDITNANOSUMMARY = Convert.ToInt32(sUMMARY.NO_OF_DELINQCREDITFACILITIES);
                                }


                                //
                                var mFCREDITMICROSUMMARY = body.MFCREDIT_MICRO_SUMMARY;
                                if (mFCREDITMICROSUMMARY != null)
                                {
                                    sUMMARY = mFCREDITMICROSUMMARY.SUMMARY;
                                    if (sUMMARY != null) deliquentOnMFCREDITMICROSUMMARY = Convert.ToInt32(sUMMARY.NO_OF_DELINQCREDITFACILITIES);
                                }

                                //
                                var mFCREDITNANOSUMMARY = body.MFCREDIT_NANO_SUMMARY;
                                if (mFCREDITNANOSUMMARY != null)
                                {
                                    sUMMARY = mFCREDITNANOSUMMARY.SUMMARY;
                                    if (sUMMARY != null) deliquentOnMFCREDITNANOSUMMARY = Convert.ToInt32(sUMMARY.NO_OF_DELINQCREDITFACILITIES);
                                }

                                //
                                var mGCREDITMICROSUMMARY = body.MGCREDIT_MICRO_SUMMARY;
                                if (mGCREDITMICROSUMMARY != null)
                                {
                                    sUMMARY = mGCREDITMICROSUMMARY.SUMMARY;
                                    if (sUMMARY != null) deliquentOnMGCREDITMICROSUMMARY = Convert.ToInt32(sUMMARY.NO_OF_DELINQCREDITFACILITIES);
                                }

                                //
                                var mGCREDITNANOSUMMARY = body.MGCREDIT_NANO_SUMMARY;
                                if (mGCREDITNANOSUMMARY != null)
                                {
                                    sUMMARY = mGCREDITNANOSUMMARY.SUMMARY;
                                    if (sUMMARY != null) deliquentOnMGCREDITNANOSUMMARY = Convert.ToInt32(sUMMARY.NO_OF_DELINQCREDITFACILITIES);
                                }
                            }
                        }

                        result = deliquentOnCREDITMICROSUMMARY <= 0 && deliquentOnCREDITNANOSUMMARY <= 0 && deliquentOnMFCREDITMICROSUMMARY <= 0 && deliquentOnMFCREDITNANOSUMMARY <= 0 && deliquentOnMGCREDITMICROSUMMARY <= 0 && deliquentOnMGCREDITNANOSUMMARY <= 0;
                        summary = $"NO OF DELINQUENT CREDIT FACILITIES ON CREDIT MICRO SUMMARY : {deliquentOnCREDITMICROSUMMARY}<br/>" +
                            $"NO OF DELINQUENT CREDIT FACILITIES ON CREDIT NANO SUMMARY : {deliquentOnCREDITNANOSUMMARY}<br/>" +
                            $"NO OF DELINQUENT CREDIT FACILITIES ON MF CREDIT MICRO SUMMARY : {deliquentOnMFCREDITMICROSUMMARY}<br/>" +
                            $"NO OF DELINQUENT CREDIT FACILITIES ON MF CREDIT NANO SUMMARY : {deliquentOnMFCREDITNANOSUMMARY}<br/>" +
                            $"NO OF DELINQUENT CREDIT FACILITIES ON MG CREDIT MICRO SUMMARY : {deliquentOnMGCREDITMICROSUMMARY}<br/>" +
                            $"NO OF DELINQUENT CREDIT FACILITIES ON MG CREDIT NANO SUMMARY : {deliquentOnMGCREDITNANOSUMMARY}";

                    }

                    if (creditCheckResponse.ConsumerSearchResultResponse != null)
                    {
                        result = true;
                        summary = "RESPONSE SEARCH LIST";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}