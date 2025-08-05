using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class UpdateEmployerDetailsInBulkViewModel
    {
        [Required(ErrorMessage = "Kindly browse and upload")]
        public HttpPostedFileBase FileUpload { get; set; }
    }
}