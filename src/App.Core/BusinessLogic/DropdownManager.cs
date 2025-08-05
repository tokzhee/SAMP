using App.Core.Services;
using App.Core.Utilities;
using App.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace App.Core.BusinessLogic
{
    public static class DropdownManager
    {
        public static List<SelectListItem> PopulateUserAccountAuthenticationTypeSelectListItems(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var selectListItems = new List<SelectListItem>();

            var selectListItem = new SelectListItem { Value = "0", Text = "-- choose authentication type --" };
            selectListItems.Add(selectListItem);

            try
            {
                var list = CacheData.Useraccountauthenticationtypes.Where(c => c.active_flag.Equals(true)).OrderBy(c => c.authentication_type_name).ToList();
                if (list != null && list.Count > 0)
                {
                    selectListItems.AddRange(list.Select(c => new SelectListItem
                    {
                        Value = Convert.ToString(c.id),
                        Text = c.authentication_type_name
                    }));
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return selectListItems;
        }
        public static List<SelectListItem> PopulateRoleSelectListItems(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var selectListItems = new List<SelectListItem>();

            var selectListItem = new SelectListItem { Value = "", Text = "-- choose role --" };
            selectListItems.Add(selectListItem);

            try
            {
                var list = RoleService.GetActive(callerFormName, callerFormMethod, callerIpAddress);
                if (list != null && list.Count > 0)
                {
                    selectListItems.AddRange(list.Select(c => new SelectListItem
                    {
                        Value = Convert.ToString(c.id),
                        Text = c.role_name
                    }));
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return selectListItems;
        }
        public static List<SelectListItem> PopulateUnreservedRoleSelectListItems(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var selectListItems = new List<SelectListItem>();

            var selectListItem = new SelectListItem { Value = "", Text = "-- choose role --" };
            selectListItems.Add(selectListItem);

            try
            {
                var list = RoleService.GetUnreservedActive(callerFormName, callerFormMethod, callerIpAddress);
                if (list != null && list.Count > 0)
                {
                    selectListItems.AddRange(list.Select(c => new SelectListItem
                    {
                        Value = Convert.ToString(c.id),
                        Text = c.role_name
                    }));
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return selectListItems;
        }
        public static List<SelectListItem> PopulateUserAccountStatusSelectListItems(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var selectListItems = new List<SelectListItem>();

            var selectListItem = new SelectListItem { Value = "0", Text = "-- choose status --" };
            selectListItems.Add(selectListItem);

            try
            {
                var list = CacheData.Useraccountstatus.OrderBy(c => c.account_status).ToList();
                if (list != null && list.Count > 0)
                {
                    selectListItems.AddRange(list.Select(c => new SelectListItem
                    {
                        Value = Convert.ToString(c.id),
                        Text = c.account_status
                    }));
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return selectListItems;
        }
        public static List<SelectListItem> PopulateFeeScaleSelectListItems(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var selectListItems = new List<SelectListItem>();

            var selectListItem = new SelectListItem { Value = "", Text = "-- how are they charged? --" };
            selectListItems.Add(selectListItem);

            try
            {
                selectListItems.Add(new SelectListItem { Value = "FLAT", Text = "FLAT" });
                selectListItems.Add(new SelectListItem { Value = "PERCENTAGE", Text = "PERCENTAGE" });
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return selectListItems;
        }
        public static List<SelectListItem> PopulateFintechSelectListItems(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var selectListItems = new List<SelectListItem>();

            var selectListItem = new SelectListItem { Value = "", Text = "-- choose fintech --" };
            selectListItems.Add(selectListItem);

            try
            {
                var list = FintechService.GetAll(callerFormName, callerFormMethod, callerIpAddress);
                if (list != null && list.Count > 0)
                {
                    selectListItems.AddRange(list.Select(c => new SelectListItem
                    {
                        Value = Convert.ToString(c.finacle_term_id),
                        Text = c.corporate_name
                    }));
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return selectListItems;
        }
        public static List<SelectListItem> PopulateEmployerSelectListItems(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var selectListItems = new List<SelectListItem>();

            var selectListItem = new SelectListItem { Value = "", Text = "-- choose employer --" };
            selectListItems.Add(selectListItem);

            try
            {
                var list = EmployerService.GetAll(callerFormName, callerFormMethod, callerIpAddress);
                if (list != null && list.Count > 0)
                {
                    selectListItems.AddRange(list.Select(c => new SelectListItem
                    {
                        Value = Convert.ToString(c.id),
                        Text = c.employer_name
                    }));
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return selectListItems;
        }
        public static List<SelectListItem> PopulateEmployerProfiledSalaryAccountSearchCriteriaSelectListItems(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            return new List<SelectListItem> 
            {
                new SelectListItem
                {
                    Value= "0",
                    Text = "Default"
                },
                new SelectListItem
                {
                    Value= "1",
                    Text = "By Employer"
                },
                new SelectListItem
                {
                    Value= "2",
                    Text = "By Account Number"
                },
                new SelectListItem
                {
                    Value= "3",
                    Text = "By Date Profiled"
                }
            };
        }
    }
}
