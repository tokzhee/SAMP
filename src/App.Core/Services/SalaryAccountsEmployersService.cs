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
    public static class SalaryAccountsEmployersService
    {
        public static salaryaccountsemployer GetWithSalaryAccountIdAndEmployerId(string salaryAccountId, string employerId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            salaryaccountsemployer salaryaccountsemployer = null;

            try
            {
                using (var db = new Data())
                {
                    salaryaccountsemployer = db.Query<salaryaccountsemployer>("select * from salary_accounts_employers with (nolock) where salary_account_id = @SalaryAccountId and employer_id = @EmployerId", new { SalaryAccountId = salaryAccountId, EmployerId = employerId });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return salaryaccountsemployer;
        }        
        public static long Insert(salaryaccountsemployer salaryaccountsemployer, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    result = db.Insert(salaryaccountsemployer);
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
