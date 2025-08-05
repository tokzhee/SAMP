using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using App.Core.Services;
using App.Web.Models;
using App.DataModels.Models;

namespace App.Web.ViewModels
{
    public class AssignRoleAccessViewModel
    {
        public AssignRoleAccessViewModel()
        {

        }

        public AssignRoleAccessViewModel(string roleId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            RoleId = roleId;

            var submenus = CacheData.Submenus.Where(c => c.active_flag.Equals(true)).OrderBy(c => c.main_menu_id).ThenBy(c => c.arrangement_order).ToList();
            if (submenus.Count > 0)
            {
                SelectedSubMenus = submenus.Select(c => new SelectedSubMenu
                {
                    SelectedSubMenuId = c.id,
                    Id = Convert.ToString(c.id),
                    DisplayName = c.display_name,
                    AccessName = c.access_name,
                    Url = c.url,
                    ActiveFlag = Convert.ToBoolean(c.active_flag),
                    ArrangementOrder = Convert.ToInt16(c.arrangement_order),
                    DisplayFlag = Convert.ToBoolean(c.display_flag),
                    IsSelected = MenuService.IsMenuAssigned(roleId, Convert.ToString(c.id), callerFormName, callerFormMethod, callerIpAddress),
                    SelectedAccessName = c.access_name
                }).ToList();
            }
        }

        [Required]
        public string RoleId { get; set; }
        public List<SelectedSubMenu> SelectedSubMenus { get; set; }

        public List<long> GetSelectedSubMenuId()
        {
            return (from c in SelectedSubMenus where c.IsSelected select c.SelectedSubMenuId).ToList();
        }

    }

    public class SelectedSubMenu : SubMenuModel
    {
        public bool IsSelected { get; set; }
        public long SelectedSubMenuId { get; set; }
        public string SelectedAccessName { get; set; }
        public string SelectedRoleId { get; set; }

    }
}