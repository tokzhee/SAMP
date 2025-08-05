using App.Core.BusinessLogic;
using App.Core.DataTransferObjects;
using App.Core.Services;
using App.Core.Utilities;
using App.DataModels.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace App.SalaryAccountProfiler.Console
{
    public class SalaryAccountProfiler
    {
        private readonly string CallerFormName = $"Salary Account Management Portal - {nameof(SalaryAccountProfiler)}";
        private readonly Timer timer;
        public SalaryAccountProfiler()
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
            ProfileAccounts();
        }
        private void UpdateSource(salaryaccountsracprofiling salaryaccountsracprofiling, string callerFormMethod)
        {
            try
            {
                var sourceUpdateResult = SalaryAccountsRacProfilingService.Update(ConfigurationUtility.GetConnectionStringValue("SourceDbConnection"), salaryaccountsracprofiling, CallerFormName, callerFormMethod, "");
                LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"{(sourceUpdateResult > 0 ? "Log has been updated for record of ID: " + salaryaccountsracprofiling.id : "Log could not be updated for record of ID:" + salaryaccountsracprofiling.id)} at {DateTime.UtcNow.AddHours(1):dd-MM-yyyy hh:mm tt}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async void ProfileAccounts()
        {
            const string callerFormMethod = nameof(ProfileAccounts);

            try
            {
                var logs = SalaryAccountsRacProfilingService.GetNonRacProfiled(ConfigurationUtility.GetConnectionStringValue("SourceDbConnection"), Convert.ToInt64(ConfigurationUtility.GetAppSettingValue("NumberOfRecordsToFetchForProfilingPerRun")), CallerFormName, callerFormMethod, "");
                if (logs == null)
                {
                    LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"The pending log records retrieved for profiling at {DateTime.UtcNow.AddHours(1):dd-MM-yyyy hh:mm tt} is null");
                    return;
                }
                if (logs.Count <= 0)
                {
                    LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"The pending log records retrieved for profiling at {DateTime.UtcNow.AddHours(1):dd-MM-yyyy hh:mm tt} is 0");
                    return;
                }

                GetCustomerDetailsResponse getCustomerDetailsResponse = null;
                GetBvnWithAccountNumberResponse getBvnWithAccountNumberResponse = null;
                salaryaccountsracprofiling salaryaccountsracprofiling = null;

                if (ConfigurationUtility.GetAppSettingValue("RunMode") == "S")
                {
                    foreach (var log in logs)
                    {
                        salaryaccountsracprofiling = new salaryaccountsracprofiling
                        {
                            id = log.id,
                            salary_account_id = log.salary_account_id,
                            created_on = log.created_on,
                            created_by = log.created_by,
                            approved_on = log.approved_on,
                            approved_by = log.approved_by,
                            rac_profiled_status_id = log.rac_profiled_status_id,
                            rac_profiled_on = log.rac_profiled_on,
                            rac_profiled_by = log.rac_profiled_by,
                            query_string = log.query_string
                        };

                        var salProfiling = FinacleServices.GetFromSalProfilingWithAccountNumber(ConfigurationUtility.GetConnectionStringValue("DestinationDbConnection"), log.account_number, CallerFormName, callerFormMethod, "");
                        if (salProfiling != null)
                        {
                            salaryaccountsracprofiling.rac_profiled_status_id = (int)SalaryAccountsRacProfileStatus.Existent;
                            UpdateSource(salaryaccountsracprofiling, callerFormMethod);
                            continue;
                        }

                        //get customer details
                        var getCustomerDetailsRequest = new GetCustomerDetailsRequest
                        {
                            RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                            CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                            AccountNumber = log.account_number
                        };

                        getCustomerDetailsResponse = await FIBridgeManager.GetCustomerDetailsAsync(getCustomerDetailsRequest, CallerFormName, callerFormMethod, "");
                        if (getCustomerDetailsResponse?.ResponseCode != "00")
                        {
                            salaryaccountsracprofiling.rac_profiled_status_id = (int)SalaryAccountsRacProfileStatus.UnableToGetCustomerDetails;
                            UpdateSource(salaryaccountsracprofiling, callerFormMethod);
                            continue;
                        };

                        //get bvn with account number
                        var getBvnWithAccountNumberRequest = new GetBvnWithAccountNumberRequest
                        {
                            RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                            CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                            AccountNumber = log.account_number
                        };

                        getBvnWithAccountNumberResponse = await FIBridgeManager.GetBvnWithAccountNumberAsync(getBvnWithAccountNumberRequest, CallerFormName, callerFormMethod, "");
                        if (getBvnWithAccountNumberResponse?.ResponseCode != "00")
                        {
                            salaryaccountsracprofiling.rac_profiled_status_id = (int)SalaryAccountsRacProfileStatus.UnableToGetBvnWithAccountNumber;
                            UpdateSource(salaryaccountsracprofiling, callerFormMethod);
                            continue;
                        };

                        //
                        var salarypaymenthistories = (List<salarypaymenthistory>)SalaryPaymentHistoryService.GetAllWithSalaryAccountsRacProfilingId(ConfigurationUtility.GetConnectionStringValue("SourceDbConnection"), Convert.ToString(log.id), CallerFormName, callerFormMethod, "");
                        if (salarypaymenthistories.Count <= 0)
                        {
                            salaryaccountsracprofiling.rac_profiled_status_id = (int)SalaryAccountsRacProfileStatus.SalaryPaymentHistoryCountLessOrEqualToZero;
                            UpdateSource(salaryaccountsracprofiling, callerFormMethod);
                            continue;
                        }

                        salProfiling = new SalProfiling
                        {
                            Foracid = Convert.ToString(log.account_number),
                            CifId = getCustomerDetailsResponse?.CustomerId,
                            Average = salarypaymenthistories.Average(taco => taco.amount).ToString(),
                            MaxVal = salarypaymenthistories.Max(taco => taco.amount).ToString(),
                            MinVal = salarypaymenthistories.Min(taco => taco.amount).ToString(),
                            //MostFreqTranDate = mostFreqTranDate,
                            FirstMonth = salarypaymenthistories.Where(taco => taco.month.Equals("First Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("First Month")).amount.ToString() : "0",
                            SecondMonth = salarypaymenthistories.Where(taco => taco.month.Equals("Second Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("Second Month")).amount.ToString() : "0",
                            ThirdMonth = salarypaymenthistories.Where(taco => taco.month.Equals("Third Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("Third Month")).amount.ToString() : "0",
                            FourthMonth = salarypaymenthistories.Where(taco => taco.month.Equals("Fourth Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("Fourth Month")).amount.ToString() : "0",
                            FifthMonth = salarypaymenthistories.Where(taco => taco.month.Equals("Fifth Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("Fifth Month")).amount.ToString() : "0",
                            SixthMonth = salarypaymenthistories.Where(taco => taco.month.Equals("Sixth Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("Sixth Month")).amount.ToString() : "0",
                            Src = "SAMP",
                            Bvn = getBvnWithAccountNumberResponse?.Bvn,
                            FirstName = getCustomerDetailsResponse?.FirstName,
                            Middlename = getCustomerDetailsResponse?.MiddleName,
                            LastName = getCustomerDetailsResponse?.LastName,
                            InsertedDate = DateTime.UtcNow.AddHours(1).ToString("dd-MMM-yy")
                        };

                        if (!string.IsNullOrEmpty(getCustomerDetailsResponse?.DateOfBirth)) salProfiling.DateOfBirth = Convert.ToDateTime(getCustomerDetailsResponse?.DateOfBirth).Date.ToString("dd-MMM-yy");
                        var destinationInsertResult = FinacleServices.InsertSalProfiling(ConfigurationUtility.GetConnectionStringValue("DestinationDbConnection"), salProfiling, CallerFormName, callerFormMethod, "");
                        if (destinationInsertResult > 0)
                        {
                            salaryaccountsracprofiling.rac_profiled_status_id = (int)SalaryAccountsRacProfileStatus.Profiled;
                            salaryaccountsracprofiling.rac_profiled_on = DateTime.UtcNow.AddHours(1);
                            salaryaccountsracprofiling.rac_profiled_by = "SYSTEM";
                            UpdateSource(salaryaccountsracprofiling, callerFormMethod);
                        }
                    }
                }
                if (ConfigurationUtility.GetAppSettingValue("RunMode") == "P")
                {
                    Parallel.ForEach(logs, async log =>
                    {
                        salaryaccountsracprofiling = new salaryaccountsracprofiling
                        {
                            id = log.id,
                            salary_account_id = log.salary_account_id,
                            created_on = log.created_on,
                            created_by = log.created_by,
                            approved_on = log.approved_on,
                            approved_by = log.approved_by,
                            rac_profiled_status_id = log.rac_profiled_status_id,
                            rac_profiled_on = log.rac_profiled_on,
                            rac_profiled_by = log.rac_profiled_by,
                            query_string = log.query_string
                        };

                        var salProfiling = FinacleServices.GetFromSalProfilingWithAccountNumber(ConfigurationUtility.GetConnectionStringValue("DestinationDbConnection"), log.account_number, CallerFormName, callerFormMethod, "");
                        if (salProfiling != null)
                        {
                            salaryaccountsracprofiling.rac_profiled_status_id = (int)SalaryAccountsRacProfileStatus.Existent;
                            UpdateSource(salaryaccountsracprofiling, callerFormMethod);
                            return;
                        }

                        //get customer details
                        var getCustomerDetailsRequest = new GetCustomerDetailsRequest
                        {
                            RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                            CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                            AccountNumber = log.account_number
                        };

                        getCustomerDetailsResponse = await FIBridgeManager.GetCustomerDetailsAsync(getCustomerDetailsRequest, CallerFormName, callerFormMethod, "");
                        if (getCustomerDetailsResponse?.ResponseCode != "00")
                        {
                            salaryaccountsracprofiling.rac_profiled_status_id = (int)SalaryAccountsRacProfileStatus.UnableToGetCustomerDetails;
                            UpdateSource(salaryaccountsracprofiling, callerFormMethod);
                            return;
                        };

                        //get bvn with account number
                        var getBvnWithAccountNumberRequest = new GetBvnWithAccountNumberRequest
                        {
                            RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                            CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                            AccountNumber = log.account_number
                        };

                        getBvnWithAccountNumberResponse = await FIBridgeManager.GetBvnWithAccountNumberAsync(getBvnWithAccountNumberRequest, CallerFormName, callerFormMethod, "");
                        if (getBvnWithAccountNumberResponse?.ResponseCode != "00")
                        {
                            salaryaccountsracprofiling.rac_profiled_status_id = (int)SalaryAccountsRacProfileStatus.UnableToGetBvnWithAccountNumber;
                            UpdateSource(salaryaccountsracprofiling, callerFormMethod);
                            return;
                        };

                        //
                        var salarypaymenthistories = (List<salarypaymenthistory>)SalaryPaymentHistoryService.GetAllWithSalaryAccountsRacProfilingId(ConfigurationUtility.GetConnectionStringValue("SourceDbConnection"), Convert.ToString(log.id), CallerFormName, callerFormMethod, "");
                        if (salarypaymenthistories.Count <= 0)
                        {
                            salaryaccountsracprofiling.rac_profiled_status_id = (int)SalaryAccountsRacProfileStatus.SalaryPaymentHistoryCountLessOrEqualToZero;
                            UpdateSource(salaryaccountsracprofiling, callerFormMethod);
                            return;
                        }

                        salProfiling = new SalProfiling
                        {
                            Foracid = Convert.ToString(log.account_number),
                            CifId = getCustomerDetailsResponse?.CustomerId,
                            Average = salarypaymenthistories.Average(taco => taco.amount).ToString(),
                            MaxVal = salarypaymenthistories.Max(taco => taco.amount).ToString(),
                            MinVal = salarypaymenthistories.Min(taco => taco.amount).ToString(),
                            //MostFreqTranDate = mostFreqTranDate,
                            FirstMonth = salarypaymenthistories.Where(taco => taco.month.Equals("First Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("First Month")).amount.ToString() : "0",
                            SecondMonth = salarypaymenthistories.Where(taco => taco.month.Equals("Second Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("Second Month")).amount.ToString() : "0",
                            ThirdMonth = salarypaymenthistories.Where(taco => taco.month.Equals("Third Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("Third Month")).amount.ToString() : "0",
                            FourthMonth = salarypaymenthistories.Where(taco => taco.month.Equals("Fourth Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("Fourth Month")).amount.ToString() : "0",
                            FifthMonth = salarypaymenthistories.Where(taco => taco.month.Equals("Fifth Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("Fifth Month")).amount.ToString() : "0",
                            SixthMonth = salarypaymenthistories.Where(taco => taco.month.Equals("Sixth Month")).Count() > 0 ? salarypaymenthistories.FirstOrDefault(taco => taco.month.Equals("Sixth Month")).amount.ToString() : "0",
                            Src = "SAMP",
                            Bvn = getBvnWithAccountNumberResponse?.Bvn,
                            FirstName = getCustomerDetailsResponse?.FirstName,
                            Middlename = getCustomerDetailsResponse?.MiddleName,
                            LastName = getCustomerDetailsResponse?.LastName,
                            InsertedDate = DateTime.UtcNow.AddHours(1).ToString("dd-MMM-yy")
                        };

                        if (!string.IsNullOrEmpty(getCustomerDetailsResponse?.DateOfBirth)) salProfiling.DateOfBirth = Convert.ToDateTime(getCustomerDetailsResponse?.DateOfBirth).Date.ToString("dd-MMM-yy");
                        var destinationInsertResult = FinacleServices.InsertSalProfiling(ConfigurationUtility.GetConnectionStringValue("DestinationDbConnection"), salProfiling, CallerFormName, callerFormMethod, "");
                        if (destinationInsertResult > 0)
                        {
                            salaryaccountsracprofiling.rac_profiled_status_id = (int)SalaryAccountsRacProfileStatus.Profiled;
                            salaryaccountsracprofiling.rac_profiled_on = DateTime.UtcNow.AddHours(1);
                            salaryaccountsracprofiling.rac_profiled_by = "SYSTEM";
                            UpdateSource(salaryaccountsracprofiling, callerFormMethod);
                        }
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
