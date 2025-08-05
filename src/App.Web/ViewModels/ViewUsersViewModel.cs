using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Core.Services;
using App.Web.Models;

namespace App.Web.ViewModels
{
    public class ViewUsersViewModel
    {
        public List<UserModel> UserModels { get; set; }
    }
}