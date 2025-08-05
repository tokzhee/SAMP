using App.Core.Utilities;
using App.Database.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.DataModels.Models;

namespace App.Core.Services
{
    public static class PasswordChangeLogService
    {
        public static List<passwordchangelog> GetLatestTwelveRecordsWithPersonId(string personId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<passwordchangelog> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<passwordchangelog>)db.QueryList<passwordchangelog>("select top(12)* from password_change_log where person_id = @PersonId order by logged_on desc", new { PersonId = personId });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static long Insert(passwordchangelog model, string callerFormName, string callerFormMethod, string callerIpAddress)
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
    }
}
