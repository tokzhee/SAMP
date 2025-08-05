using System.Web;

namespace App.Web.Models
{
    public class PersonModel
    {
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Fullname
        {
            get
            {
                return $"{Surname}, {Firstname} {Middlename}";
            }
        }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Passport { get; set; }
        public HttpPostedFileBase PassportFile { get; set; }
        public string Signature { get; set; }
        public HttpPostedFileBase SignatureFile { get; set; }
        public string PersonTypeId { get; set; }
        public string PersonTypeName { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string UrlQueryString { get; set; }
    }
}