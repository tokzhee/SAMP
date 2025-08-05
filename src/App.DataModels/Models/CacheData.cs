using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataModels.Models
{
    public class CacheData
    {
        public static List<menuicon> Menuicons { get; set; }
        public static List<mainmenu> Mainmenus { get; set; }
        public static List<submenu> Submenus { get; set; }
        public static List<rolemenu> Rolemenus { get; set; }
        public static List<useraccountauthenticationtype> Useraccountauthenticationtypes { get; set; }
        public static List<useraccountstatu> Useraccountstatus { get; set; }
        public static List<onboardingdocument> Onboardingdocuments { get; set; }
        public static List<persontype> Persontypes { get; set; }
    }
}
