using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class FintechOnBoardingDocumentsModel
    {
        public string FintechId { get; set; }
        public string DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSavePath { get; set; }
        public HttpPostedFileBase DocumentFile { get; set; }
        public bool MandatoryFlag { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string UrlQueryString { get; set; }
    }
}