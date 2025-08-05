using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Utilities;
using App.Database.Access;
using App.DataModels.Models;

namespace App.Core.Services
{
    public static class SalaryAccountsService
    {
        public static salaryaccount GetWithAccountNumber(string accountNumber, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            salaryaccount salaryaccount = null;

            try
            {
                using (var db = new Data())
                {
                    salaryaccount = db.Query<salaryaccount>("select * from salary_accounts with (nolock) where account_number = @AccountNumber", new { AccountNumber = accountNumber });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return salaryaccount;
        }
        public static List<EmployerProfileSalaryAccountsReport> GetAllEmployerProfiled(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<EmployerProfileSalaryAccountsReport> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<EmployerProfileSalaryAccountsReport>)db.QueryList<EmployerProfileSalaryAccountsReport>("select A.account_number as AccountNumber, A.account_name as AccountName, C.employer_name as EmployerName, A.created_on as CreatedOn, A.created_by as CreatedBy, A.approved_on as ApprovedOn, A.approved_by as ApprovedBy from salary_accounts A with (nolock) inner join salary_accounts_employers B with (nolock) on A.id = B.salary_account_id inner join employer C with (nolock) on C.id = B.employer_id");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<EmployerProfileSalaryAccountsReport> GetAllEmployerProfiledWithEmployerId(string employerId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<EmployerProfileSalaryAccountsReport> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<EmployerProfileSalaryAccountsReport>)db.QueryList<EmployerProfileSalaryAccountsReport>("select A.account_number as AccountNumber, A.account_name as AccountName, C.employer_name as EmployerName, A.created_on as CreatedOn, A.created_by as CreatedBy, A.approved_on as ApprovedOn, A.approved_by as ApprovedBy from salary_accounts A with (nolock) inner join salary_accounts_employers B with (nolock) on A.id = B.salary_account_id inner join employer C with (nolock) on C.id = B.employer_id where B.employer_id = @EmployerId order by A.account_name", new { EmployerId = employerId});
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<EmployerProfileSalaryAccountsReport> GetAllEmployerProfiledWithAccountNumber(string accountNumber, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<EmployerProfileSalaryAccountsReport> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<EmployerProfileSalaryAccountsReport>)db.QueryList<EmployerProfileSalaryAccountsReport>("select A.account_number as AccountNumber, A.account_name as AccountName, C.employer_name as EmployerName, A.created_on as CreatedOn, A.created_by as CreatedBy, A.approved_on as ApprovedOn, A.approved_by as ApprovedBy from salary_accounts A with (nolock) inner join salary_accounts_employers B with (nolock) on A.id = B.salary_account_id inner join employer C with (nolock) on C.id = B.employer_id where A.account_number = @AccountNumber", new { AccountNumber = accountNumber });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<EmployerProfileSalaryAccountsReport> GetAllEmployerProfiledBetweenStartDateAndEndDate(string from, string to, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<EmployerProfileSalaryAccountsReport> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<EmployerProfileSalaryAccountsReport>)db.QueryList<EmployerProfileSalaryAccountsReport>("select A.account_number as AccountNumber, A.account_name as AccountName, C.employer_name as EmployerName, A.created_on as CreatedOn, A.created_by as CreatedBy, A.approved_on as ApprovedOn, A.approved_by as ApprovedBy from salary_accounts A with (nolock) inner join salary_accounts_employers B with (nolock) on A.id = B.salary_account_id inner join employer C with (nolock) on C.id = B.employer_id where CONVERT(date, A.created_on) between @StartDate and @EndDate order by A.account_number, A.created_on desc", new { StartDate = from, EndDate = to });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static long Insert(salaryaccount salaryaccount, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    result = db.Insert(salaryaccount);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Insert(salaryaccount model, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                var inputParameters = new Dictionary<string, object>
                {
                    { "@paramAccountNumber", model.account_number },
                    { "@paramAccountName", model.account_name},
                    { "@paramCreatedOn", model.created_on },
                    { "@paramCreatedBy", model.created_by },
                    { "@paramApprovedOn", model.approved_on },
                    { "@paramApprovedBy", model.approved_by }
                };

                var outputParameters = new Dictionary<string, object>
                {
                    { "@paramId", "" }
                };

                var insertResult = db.ExecuteProcedure<object>("sp_SalaryAccountInsertSalaryAccount", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                result = Convert.ToInt64(insertResult["@paramId"]);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
    }
}
