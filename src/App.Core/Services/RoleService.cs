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
    public static class RoleService
    {
        public static role GetWithRoleId(string roleId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            role role = null;

            try
            {
                using (var db = new Data())
                {
                    role = db.Get<role>(roleId);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return role;
        }
        public static role GetWithRoleName(string rolename, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            role role = null;

            try
            {
                using (var db = new Data())
                {
                    role = db.Query<role>("select * from roles where role_name = @rolename", new { rolename });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return role;
        }
        public static List<role> GetAll(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<role> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<role>)db.GetList<role>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<role> GetActive(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<role> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<role>)db.GetList<role>("where active_flag = 'True'");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<role> GetUnreservedActive(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<role> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<role>)db.GetList<role>("where active_flag = 'True' and system_reserved_flag = 'false'");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static role GetWithUrlQueryString(string queryString, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            role role = null;

            try
            {
                using (var db = new Data())
                {
                    role = db.Query<role>("select * from roles where query_string = @UrlQueryString", new { UrlQueryString = queryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return role;
        }
        public static long Insert(role role, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    var existingRole = (List<role>)db.GetList<role>("where role_name = @RoleName", new { RoleName = role.role_name });
                    result = existingRole.Count > 0 ? -1 : db.Insert(role);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }

        public static long Update(role role, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    result = db.Update(role);
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
