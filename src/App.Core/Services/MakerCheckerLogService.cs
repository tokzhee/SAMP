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
    public static class MakerCheckerLogService
    {
        public static makercheckerlog GetWithMakerCheckerLogId(string makerCheckerLogId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            makercheckerlog makerCheckerLog = null;

            try
            {
                using (var db = new Data())
                {
                    makerCheckerLog = db.Get<makercheckerlog>(makerCheckerLogId);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return makerCheckerLog;
        }
        public static makercheckerlog GetWithQueryString(string queryString, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            makercheckerlog makerCheckerLog = null;

            try
            {
                using (var db = new Data())
                {
                    makerCheckerLog = db.Query<makercheckerlog>("select * from maker_checker_log where query_string = @QueryString", new { QueryString = queryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return makerCheckerLog;
        }
        public static List<makercheckerlog> GetWithMakerCheckerTypeAndStatus(int makerCheckerType, int makerCheckerStatus, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<makercheckerlog> makerCheckerLogs = null;

            try
            {
                using (var db = new Data())
                {
                    makerCheckerLogs = (List<makercheckerlog>)db.GetList<makercheckerlog>("where maker_checker_type_id = @Type and maker_checker_status = @Status", new { Type = makerCheckerType, Status = makerCheckerStatus });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return makerCheckerLogs;
        }
        public static List<makercheckerlog> GetWithMakerCheckerCategoryAndStatus(int makerCheckerCategory, int makerCheckerStatus, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<makercheckerlog> makerCheckerLogs = null;

            try
            {
                using (var db = new Data())
                {
                    makerCheckerLogs = (List<makercheckerlog>)db.GetList<makercheckerlog>("where maker_checker_category_id = @Category and maker_checker_status = @Status", new { Category = makerCheckerCategory, Status = makerCheckerStatus });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return makerCheckerLogs;
        }
        public static List<makercheckerlog> GetWithMakerCheckerCategoryAndSolIdAndStatus(int makerCheckerCategory, string solId, int makerCheckerStatus, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<makercheckerlog> makerCheckerLogs = null;

            try
            {
                using (var db = new Data())
                {
                    makerCheckerLogs = (List<makercheckerlog>)db.GetList<makercheckerlog>("where maker_checker_category_id = @Category and maker_sol_id = @SolId and maker_checker_status = @Status", new { Category = makerCheckerCategory, SolId = solId, Status = makerCheckerStatus });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return makerCheckerLogs;
        }
        public static long Insert(makercheckerlog makerCheckerLog, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    result = db.Insert(makerCheckerLog);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Update(makercheckerlog makerCheckerLog, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    result = db.Update(makerCheckerLog);
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
