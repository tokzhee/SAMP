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
    public static class UserAccessActivityService
    {
        public static List<useraccessactivity> GetAll(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<useraccessactivity> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<useraccessactivity>)db.GetList<useraccessactivity>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static useraccessactivity GetWithKey(string accessKey, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            useraccessactivity usageLog = null;

            try
            {
                using (var db = new Data())
                {
                    usageLog = db.Query<useraccessactivity>("select * from user_access_activity where access_key = @sessionKey", new { sessionKey = accessKey });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return usageLog;
        }
        public static List<useraccessactivity> GetAllWithStartDateAndEndDate(string startDate, string endDate, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<useraccessactivity> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<useraccessactivity>)db.GetList<useraccessactivity>("where CONVERT(date, created_on) between @StartDate and @EndDate order by created_on desc", new { StartDate = startDate, EndDate = endDate });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<useraccessactivity> GetAllFailedLogonAttempts(string username, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<useraccessactivity> list = null;

            try
            {
                using (var db = new Data())
                {
                    //select * from user_access_activity where username = @Username and (remarks like 'Login Failure%' or remarks like 'false%') and (ignore_for_account_lock is null or ignore_for_account_lock = 'false')
                    list = (List<useraccessactivity>)db.QueryList<useraccessactivity>("select * from user_access_activity where username = @Username and remarks like 'Login Failure%' and (ignore_for_account_lock is null or ignore_for_account_lock = 'false')", new { Username = username });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<Chart> GetMyLoginChartDataWithUsername(string username, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<Chart> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<Chart>)db.QueryList<Chart>("select (case when SUBSTRING(remarks, 1,5) = 'false' then '\"'+'Failed Login'+'\"' when remarks = 'Successful' then '\"'+'Successful Login'+'\"' when SUBSTRING(remarks, 1,13) = 'Login Failure' then '\"'+'Failed Login'+'\"' end) chartKey, count(remarks) as chartValue from user_access_activity where username = @Username group by (case when SUBSTRING(remarks, 1,5) = 'false' then '\"'+'Failed Login'+'\"' when remarks = 'Successful' then '\"'+'Successful Login'+'\"' when SUBSTRING(remarks, 1,13) = 'Login Failure' then '\"'+'Failed Login'+'\"' end) order by 1", new { Username = username });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<Chart> GetOverallLoginChartData(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<Chart> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<Chart>)db.QueryList<Chart>("select (case when SUBSTRING(remarks, 1,5) = 'false' then '\"'+'Failed Login'+'\"' when remarks = 'Successful' then '\"'+'Successful Login'+'\"' when SUBSTRING(remarks, 1,13) = 'Login Failure' then '\"'+'Failed Login'+'\"' end) chartKey, count(remarks) as chartValue from user_access_activity group by (case when SUBSTRING(remarks, 1,5) = 'false' then '\"'+'Failed Login'+'\"' when remarks = 'Successful' then '\"'+'Successful Login'+'\"' when SUBSTRING(remarks, 1,13) = 'Login Failure' then '\"'+'Failed Login'+'\"' end) order by 1");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        
        public static void Insert(useraccessactivity model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            try
            {
                using (var db = new Data())
                {
                    db.Insert(model);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }
        }
        public static void Update(useraccessactivity model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            try
            {
                using (var db = new Data())
                {
                    db.Update(model);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }
        }
    }
}
