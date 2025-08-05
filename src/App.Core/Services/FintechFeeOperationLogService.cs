using App.Core.Utilities;
using App.Database.Access;
using App.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Services
{
    public static class FintechFeeOperationLogService
    {
        public static List<fintechfeeoperationlog> GetPendingForOperation(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<fintechfeeoperationlog> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<fintechfeeoperationlog>)db.GetList<fintechfeeoperationlog>("where operation_status_id = 0");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static long Insert(fintechfeeoperationlog model, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                result = db.Insert(model, db.DbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Update(fintechfeeoperationlog model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
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
