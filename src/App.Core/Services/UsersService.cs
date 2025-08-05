using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.BusinessLogic;
using App.Core.Utilities;
using App.Database.Access;
using App.DataModels.Models;

namespace App.Core.Services
{
    public static class UsersService
    {
        public static user GetWithUserId(string userId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            user user = null;

            try
            {
                using (var db = new Data())
                {
                    user = db.Get<user>(userId);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return user;
        }
        public static user GetWithUsername(string username, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            user user = null;

            try
            {
                using (var db = new Data())
                {
                    user = db.Query<user>("select * from users where username = @Username", new { Username = username });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return user;
        }
        public static List<user> GetAllUsersExceptLoggedInUser(string loggedInUser, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<user> users = null;

            try
            {
                using (var db = new Data())
                {
                    users = (List<user>)db.GetList<user>("where username <> @loggedInUser order by created_on desc", new { loggedInUser});
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return users;
        }
        public static List<user> GetAllActiveUsers(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<user> webusers = null;

            try
            {
                using (var db = new Data())
                {
                    webusers = (List<user>)db.GetList<user>($"where status_id not in ('{(int)UserAccountStatus.Inactive}','{(int)UserAccountStatus.Deleted}') order by created_on desc");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return webusers;
        }
        public static user GetWithUrlQueryString(string queryString, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            user user = null;

            try
            {
                using (var db = new Data())
                {
                    user = db.Query<user>("select * from users where query_string = @UrlQueryString", new { UrlQueryString = queryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return user;
        }
        public static List<Chart> GetUserStatusChartData(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<Chart> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<Chart>)db.QueryList<Chart>("select '\"' + b.account_status + '\"' as chartKey, count(b.account_status) as chartValue from users A, user_account_status B where a.status_id = b.id group by b.account_status order by 1");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static long Insert(user model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    var existingUser = (List<user>)db.GetList<user>("where username = @Username", new { Username = model.username });
                    result = existingUser.Count > 0 ? -1 : db.Insert(model);
                } 
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Update(user model, string callerFormName, string callerFormMethod, string callerIpAddress)
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
        public static List<UserReport> GetAllCreatedBetweenStartDateAndEndDate(string from, string to, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<UserReport> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<UserReport>)db.QueryList<UserReport>("select u.username as Username, p.surname + ', ' + p.first_name + ' ' + ISNULL(p.middle_name,'') as Fullname, r.role_name as RoleName, u.last_login_date as LastLoginDate, u.created_on as CreatedOn, u.created_by as CreatedBy, u.approved_on as ApprovedOn, u.approved_by as ApprovedBy, uas.account_status as AccountStatus from users u, person p, roles r, user_account_status uas where u.person_id = p.id and u.role_id = r.id and u.status_id = uas.id and CONVERT(date, u.created_on) between @StartDate and @EndDate order by u.created_on desc", new { StartDate = from, EndDate = to });
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
