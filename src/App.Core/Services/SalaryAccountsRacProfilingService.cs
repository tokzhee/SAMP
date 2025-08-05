using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Utilities;
using App.Database.Access;
using App.DataModels.Models;

namespace App.Core.Services
{
    public static class SalaryAccountsRacProfilingService
    {
        public static salaryaccountsracprofiling GetWithAccountNumber(string accountNumber, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            salaryaccountsracprofiling salaryaccountsracprofiling = null;

            try
            {
                using (var db = new Data())
                {
                    salaryaccountsracprofiling = db.Query<salaryaccountsracprofiling>("select A.* from salary_accounts_rac_profiling A with (nolock) inner join salary_accounts B with (nolock) on A.salary_account_id = B.id where B.account_number = @AccountNumber", new { AccountNumber = accountNumber });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return salaryaccountsracprofiling;
        }
        public static List<SalaryAccountsRacProfiling> GetNonRacProfiled(string connectionString, long numberOfRecordsToFetch, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<SalaryAccountsRacProfiling> salaryaccountsracprofilings = null;

            try
            {
                using (var db = new Data(connectionString, null))
                {
                    salaryaccountsracprofilings = (List<SalaryAccountsRacProfiling>)db.QueryList<SalaryAccountsRacProfiling>($"select top({numberOfRecordsToFetch}) A.id, A.salary_account_id, B.account_number, B.account_name, A.created_on, A.created_by, A.approved_on, A.approved_by, A.rac_profiled_status_id, A.rac_profiled_on, A.rac_profiled_by, A.query_string from salary_accounts_rac_profiling A with (nolock) inner join salary_accounts B with (nolock) on A.salary_account_id = B.id  where A.rac_profiled_status_id is null or A.rac_profiled_status_id = 0 order by newid()");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return salaryaccountsracprofilings;
        }
        public static long Insert(salaryaccountsracprofiling model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    db.DbConnection.Open();

                    using (var dbTransaction = db.DbConnection.BeginTransaction())
                    {
                        //add salary account
                        var salaryAccountId = SalaryAccountsService.Insert(model.salaryaccount, db, dbTransaction, callerFormName, callerFormMethod, callerIpAddress);
                        if (salaryAccountId <= 0)
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        //add profiling
                        var inputParameters = new Dictionary<string, object>
                        {
                            { "@paramSalaryAccountId", salaryAccountId },
                            { "@paramCreatedOn", model.created_on },
                            { "@paramCreatedBy", model.created_by },
                            { "@paramApprovedOn", model.approved_on },
                            { "@paramApprovedBy", model.approved_by }
                        };
                        var outputParameters = new Dictionary<string, object>
                        {
                            { "@paramId", "" }
                        };

                        var racProfilingInsertResult = db.ExecuteProcedure<object>("sp_SalaryAccountRacProfilingInsertSalaryAccountRacProfiling", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                        var salaryAccountRacProfilingId = Convert.ToInt64(racProfilingInsertResult["@paramId"]);
                        if (salaryAccountRacProfilingId <= 0)
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        //payment history
                        var salarypaymenthistories = model.salarypaymenthistory.Select(payment => new salarypaymenthistory
                        {
                            salary_accounts_rac_profiling_id = salaryAccountRacProfilingId,
                            month = payment.month,
                            amount = payment.amount,
                            transaction_date = payment.transaction_date,
                            evidence_save_path = payment.evidence_save_path,
                            created_on = payment.created_on,
                            created_by = payment.created_by,
                            approved_on = payment.approved_on,
                            approved_by = payment.approved_by

                        }).ToList();

                        var numberSaved = SalaryPaymentHistoryService.Insert(salarypaymenthistories, db, dbTransaction, callerFormName, callerFormMethod, callerIpAddress);
                        if (!numberSaved.Equals(salarypaymenthistories.Count))
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        dbTransaction.Commit();
                        result = salaryAccountRacProfilingId;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Update(string connectionString, salaryaccountsracprofiling model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data(connectionString, null))
                {
                    result = db.Update(model);
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
