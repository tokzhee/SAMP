using App.Core.Utilities;
using App.Database.Access;
using App.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Services
{
    public static class EmailLogService
    {
        public static List<emaillog> GetAll(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<emaillog> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<emaillog>)db.GetList<emaillog>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static long Insert(emaillog model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    result = db.Insert(model);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Update(emaillog model, string callerFormName, string callerFormMethod, string callerIpAddress)
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
