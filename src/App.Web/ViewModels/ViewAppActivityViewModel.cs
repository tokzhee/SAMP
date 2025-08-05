using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using App.Core.Services;
using App.Web.Models;

namespace App.Web.ViewModels
{
    public class ViewAppActivityViewModel
    {
        public ViewAppActivityViewModel()
        {
            ReportSearchDateModel = new ReportSearchDateModel();
        }
        public ReportSearchDateModel ReportSearchDateModel { get; set; }
    }
}