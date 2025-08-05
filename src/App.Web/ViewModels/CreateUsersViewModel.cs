using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Core.Services;
using App.Web.Models;

namespace App.Web.ViewModels
{
    public class CreateUsersViewModel
    {
        public CreateUsersViewModel(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            UserModel = new UserModel(callerFormName, callerFormMethod, callerIpAddress);
        }

        public UserModel UserModel { get; set; }
    }
}