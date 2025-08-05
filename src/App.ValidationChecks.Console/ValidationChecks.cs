using App.Core.BusinessLogic;
using App.Core.BvnValidatorServiceClient;
using App.Core.DataTransferObjects;
using App.Core.Services;
using App.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace App.ValidationChecks.Console
{
    public class ValidationChecks
    {
        private readonly string CallerFormName = $"Salary Account Management Portal - {nameof(ValidationChecks)}";
        private readonly Timer timer;
        public ValidationChecks()
        {
            timer = new Timer()
            {
                Interval = GetRunInterval(),
                AutoReset = true
            };

            timer.Elapsed += TimerElapsed;
        }
        public void Start()
        {
            const string callerFormMethod = nameof(Start);

            try
            {
                timer.Start();
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, "", ex);
                LogUtility.LogError(CallerFormName, callerFormMethod, "", "Failed to start timer");
            }

            LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"Service started at {DateTime.UtcNow.AddHours(1):dd-MM-yyyy hh:mm tt}");
        }
        public void Stop()
        {
            const string callerFormMethod = nameof(Stop);

            try
            {
                timer.Stop();
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, "", ex);
                LogUtility.LogError(CallerFormName, callerFormMethod, "", "Failed to stop timer");
            }

            LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"Service stopped at {DateTime.UtcNow.AddHours(1):dd-MM-yyyy hh:mm tt}");
        }
        private double GetRunInterval()
        {
            const string callerFormMethod = nameof(GetRunInterval);

            double runInterval = 0;

            try
            {
                runInterval = Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("RunInterval"));
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, "", ex);
                LogUtility.LogError(CallerFormName, callerFormMethod, "", "Failed on converting run interval to integer value. kindly ensure run interval value in the config is an integer");
            }

            return runInterval;
        }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            DoCheck();
        }
        private async void DoCheck()
        {
            const string callerFormMethod = nameof(DoCheck);

            try
            {
                var salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from (SELECT rownum RN, A.* from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling A) where (MOD(RN, {ConfigurationUtility.GetAppSettingValue("ServiceInstanceCount")})+ 1) = {ConfigurationUtility.GetAppSettingValue("ServiceInstanceId")} and (is_valid_crms is null or is_valid_crc is null or is_valid_bvn is null) and rownum <= {ConfigurationUtility.GetAppSettingValue("NumberOfRecordsToFetchForCheckPerRun")} order by INSERTED_DATE desc", CallerFormName, callerFormMethod, "");
                if (salProfilings == null)
                {
                    LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"The pending log records retrieved for checks at {DateTime.UtcNow.AddHours(1):dd-MM-yyyy hh:mm tt} is null");
                    return;
                }
                if (salProfilings.Count <= 0)
                {
                    LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"The pending log records retrieved for checks at {DateTime.UtcNow.AddHours(1):dd-MM-yyyy hh:mm tt} is 0");
                    return;
                }

                ValidatorResponse validatorResponse = null;
                CRMSCreditCheckResponse creditCheckResponse = null;
                CRCCreditCheckResponse cRCCreditCheckResponse = null;

                if (ConfigurationUtility.GetAppSettingValue("RunMode") == "S")
                {
                    foreach (var salProfiling in salProfilings)
                    {
                        if (string.IsNullOrEmpty(salProfiling.Bvn)) continue;
                        if (string.IsNullOrEmpty(salProfiling.IsValidBvn))
                        {
                            await Task.Run(() => 
                            { 
                                validatorResponse = new NIBSSBvnValidatorManager().ValidateBvn(salProfiling.Bvn, CallerFormName, callerFormMethod, ""); 
                            });

                            if (validatorResponse != null && validatorResponse.Status?.ResponseCode == "00")
                            {
                                var customerDetailsDictionary = new Dictionary<string, string>
                                {
                                    { "SN", salProfiling?.LastName},
                                    { "FN", salProfiling?.FirstName},
                                    { "DOB", salProfiling?.DateOfBirth},
                                    { "G", "N/A"},
                                };

                                var bvnDictionary = new Dictionary<string, string>
                                {
                                    { "SN", validatorResponse.Result?.LastName},
                                    { "FN", validatorResponse.Result?.FirstName},
                                    { "DOB", validatorResponse.Result?.DateOfBirth},
                                    { "G", "N/A"},
                                };

                                var isValidBvn = ValidationUtility.IsValidBvnDetails(customerDetailsDictionary, bvnDictionary);
                                salProfiling.IsValidBvn = isValidBvn ? "Y" : "N";
                            }
                        }
                        if (string.IsNullOrEmpty(salProfiling.IsValidCRMS))
                        {
                            await Task.Run(() => 
                            { 
                                creditCheckResponse = new CRMSManager().CreditCheck(salProfiling.Bvn, CallerFormName, callerFormMethod, ""); 
                            });

                            if (creditCheckResponse != null)
                            {
                                var isValidCRMS = ValidationUtility.IsValidCRMSDetails(creditCheckResponse?.Summary);
                                salProfiling.IsValidCRMS = isValidCRMS ? "Y" : "N";
                            }
                        }
                        if (string.IsNullOrEmpty(salProfiling.IsValidCRC))
                        {
                            cRCCreditCheckResponse = await CRCManager.CreditCheckAsync(salProfiling.Bvn, CallerFormName, callerFormMethod, "");
                            if (cRCCreditCheckResponse != null)
                            {
                                var crcSummary = "";
                                var isValidCRC = ValidationUtility.IsValidCRCDetails(cRCCreditCheckResponse, ref crcSummary);
                                salProfiling.IsValidCRC = isValidCRC ? "Y" : "N";
                            }
                        }

                        var updateResult = FinacleServices.UpdateSalProfiling(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection"), salProfiling, CallerFormName, callerFormMethod, "");
                        LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"{(updateResult > 0 ? "Log has been updated for record of Account Number: " + salProfiling.Foracid : "Log could not be updated for record of Account Number:" + salProfiling.Foracid)} at {DateTime.UtcNow.AddHours(1):dd-MM-yyyy hh:mm tt}");
                    }
                }
                if (ConfigurationUtility.GetAppSettingValue("RunMode") == "P")
                {
                    Parallel.ForEach(salProfilings, async salProfiling =>
                    {
                        if (string.IsNullOrEmpty(salProfiling.Bvn)) return;
                        if (string.IsNullOrEmpty(salProfiling.IsValidBvn))
                        {
                            await Task.Run(() =>
                            {
                                validatorResponse = new NIBSSBvnValidatorManager().ValidateBvn(salProfiling.Bvn, CallerFormName, callerFormMethod, "");
                            });

                            if (validatorResponse != null && validatorResponse.Status?.ResponseCode == "00")
                            {
                                var customerDetailsDictionary = new Dictionary<string, string>
                                {
                                    { "SN", salProfiling?.LastName},
                                    { "FN", salProfiling?.FirstName},
                                    { "DOB", salProfiling?.DateOfBirth},
                                    { "G", "N/A"},
                                };

                                var bvnDictionary = new Dictionary<string, string>
                                {
                                    { "SN", validatorResponse.Result?.LastName},
                                    { "FN", validatorResponse.Result?.FirstName},
                                    { "DOB", validatorResponse.Result?.DateOfBirth},
                                    { "G", "N/A"},
                                };

                                var isValidBvn = ValidationUtility.IsValidBvnDetails(customerDetailsDictionary, bvnDictionary);
                                salProfiling.IsValidBvn = isValidBvn ? "Y" : "N";
                            }
                        }
                        if (string.IsNullOrEmpty(salProfiling.IsValidCRMS))
                        {
                            await Task.Run(() =>
                            {
                                creditCheckResponse = new CRMSManager().CreditCheck(salProfiling.Bvn, CallerFormName, callerFormMethod, "");
                            });

                            if (creditCheckResponse != null)
                            {
                                var isValidCRMS = ValidationUtility.IsValidCRMSDetails(creditCheckResponse.Summary);
                                salProfiling.IsValidCRMS = isValidCRMS ? "Y" : "N";
                            }
                        }
                        if (string.IsNullOrEmpty(salProfiling.IsValidCRC))
                        {
                            cRCCreditCheckResponse = await CRCManager.CreditCheckAsync(salProfiling.Bvn, CallerFormName, callerFormMethod, "");
                            if (cRCCreditCheckResponse != null)
                            {
                                var crcSummary = "";
                                var isValidCRC = ValidationUtility.IsValidCRCDetails(cRCCreditCheckResponse, ref crcSummary);
                                salProfiling.IsValidCRC = isValidCRC ? "Y" : "N";
                            }
                        }

                        var updateResult = FinacleServices.UpdateSalProfiling(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection"), salProfiling, CallerFormName, callerFormMethod, "");
                        LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"{(updateResult > 0 ? "Log has been updated for record of Account Number: " + salProfiling.Foracid : "Log could not be updated for record of Account Number:" + salProfiling.Foracid)} at {DateTime.UtcNow.AddHours(1):dd-MM-yyyy hh:mm tt}");
                    });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, "", ex);
            }
        }
    }
}
