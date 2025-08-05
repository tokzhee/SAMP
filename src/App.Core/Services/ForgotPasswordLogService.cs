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
    public static class ForgotPasswordLogService
    {
        public static forgotpasswordlog GetWithForgotPasswordCode(string code, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            forgotpasswordlog forgotpasswordlog = null;

            try
            {
                using (var db = new Data())
                {
                    forgotpasswordlog = db.Query<forgotpasswordlog>("select * from forgot_password_log where forgot_password_code = @Code", new { Code = code });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return forgotpasswordlog;
        }
        public static long Insert(forgotpasswordlog model, string callerFormName, string callerFormMethod, string callerIpAddress)
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
