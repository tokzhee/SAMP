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
    public static class EmployerService
    {
        public static employer GetWithEmployerId(string employerId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            employer employer = null;

            try
            {
                using (var db = new Data())
                {
                    employer = db.Get<employer>(employerId);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return employer;
        }
        public static employer GetWithEmployerName(string employerName, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            employer employer = null;

            try
            {
                using (var db = new Data())
                {
                    employer = db.Query<employer>("select * from employer with (nolock) where employer_name = @EmployerName", new { EmployerName = employerName });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return employer;
        }
        public static List<employer> GetWithAccountNumber(string accountNumber, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<employer> employers = null;

            try
            {
                using (var db = new Data())
                {
                    employers = (List<employer>)db.QueryList<employer>("select C.* from salary_accounts_employers A inner join salary_accounts B on A.salary_account_id = B.id inner join employer C on A.employer_id = C.id where B.account_number = @AccountNumber", new { AccountNumber = accountNumber });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return employers;
        }
        public static List<employer> GetAll(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<employer> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<employer>)db.QueryList<employer>("select * from employer with (nolock) order by employer_name");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static long Insert(employer employer, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    result = db.Insert(employer);
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
