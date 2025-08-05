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
    public static class UserAccountAuthenticationTypeService
    {
        public static List<useraccountauthenticationtype> GetAll(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<useraccountauthenticationtype> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<useraccountauthenticationtype>)db.GetList<useraccountauthenticationtype>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }

    }
}
