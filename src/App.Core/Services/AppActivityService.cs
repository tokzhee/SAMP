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
    public static class AppActivityService
    {
        public static List<appactivity> GetAll(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<appactivity> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<appactivity>)db.GetList<appactivity>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<appactivity> GetAllWithStartDateAndEndDate(string startDate, string endDate, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<appactivity> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<appactivity>)db.GetList<appactivity>("where CONVERT(date, created_on) between @StartDate and @EndDate order by created_on desc", new { StartDate = startDate, EndDate = endDate });
                }
                
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static void Insert(appactivity appactivity, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            try
            {
                using (var db = new Data())
                {
                    db.Insert(appactivity);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }
        }
    }
}
