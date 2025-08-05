using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class SubMenuModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string AccessName { get; set; }
        public string Url { get; set; }
        public bool ActiveFlag { get; set; }
        public int ArrangementOrder { get; set; }
        public bool DisplayFlag { get; set; }
    }

}