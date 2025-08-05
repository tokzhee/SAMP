using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class ViewAppUsersViewModel
    {
        public ViewAppUsersViewModel()
        {
            ReportSearchDateModel = new ReportSearchDateModel();
        }
        public ReportSearchDateModel ReportSearchDateModel { get; set; }
    }
}