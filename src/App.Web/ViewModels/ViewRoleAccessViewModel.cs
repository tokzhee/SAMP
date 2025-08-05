using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Core.BusinessLogic;
using App.Core.Services;
using App.Web.Models;

namespace App.Web.ViewModels
{
    public class ViewRoleAccessViewModel
    {
        public ViewRoleAccessViewModel()
        {

        }
        public ViewRoleAccessViewModel(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            RoleSelectListItems = DropdownManager.PopulateRoleSelectListItems(callerFormName, callerFormMethod, callerIpAddress);
        }

        [Required(ErrorMessage = "Role field is required")]
        public string RoleId { get; set; }

        public List<SelectListItem> RoleSelectListItems { get; set; }

        public AssignRoleAccessViewModel AssignRoleAccessViewModel { get; set; }
    }
}