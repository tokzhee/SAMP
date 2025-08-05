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
    public class SalaryPaymentHistoryService
    {
        public static List<salarypaymenthistory> GetAllWithSalaryAccountsRacProfilingId(string connectionString, string salaryAccountsRacProfilingId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<salarypaymenthistory> salarypaymenthistories = null;

            try
            {
                using (var db = new Data(connectionString, null))
                {
                    salarypaymenthistories = (List<salarypaymenthistory>)db.QueryList<salarypaymenthistory>("select * from salary_payment_history with (nolock) where salary_accounts_rac_profiling_id = @SalaryAccountsRacProfilingId", new { SalaryAccountsRacProfilingId = salaryAccountsRacProfilingId });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return salarypaymenthistories;
        }
        public static long Insert(List<salarypaymenthistory> salarypaymenthistories, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                foreach (var item in salarypaymenthistories)
                {
                    var inputParameters = new Dictionary<string, object>
                    {
                        { "@paramSalaryAccountRacProfilingId", item.salary_accounts_rac_profiling_id },
                        { "@paramMonth", item.month},
                        { "@paramAmount", item.amount},
                        { "@paramTransactionDate", item.transaction_date},
                        { "@paramEvidenceSavePath", item.evidence_save_path },
                        { "@paramCreatedOn", item.created_on },
                        { "@paramCreatedBy", item.created_by },
                        { "@paramApprovedOn", item.approved_on },
                        { "@paramApprovedBy", item.approved_by }
                    };

                    var outputParameters = new Dictionary<string, object>
                    {
                        { "@paramId", "" }
                    };

                    var insertResult = db.ExecuteProcedure<object>("sp_SalaryPaymentHistoryInsertSalaryPaymentHistory", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                    var insertId = Convert.ToInt64(insertResult["@paramId"]);
                    if (insertId > 0)
                    {
                        result += 1;
                    }
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
