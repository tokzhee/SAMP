using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using App.Core.Services;
using App.Web.Models;

namespace App.Web.ViewModels
{
    public class LoginViewModel
    {
        public LoginViewModel()
        {

        }

        public LoginViewModel(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            //
        }

        [Required]
        //[RegularExpression("(^[ A-Za-z0-9./-]+$)", ErrorMessage = ("special characters are not allowed"))]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Invalid password length")]
        public string Password { get; set; }
        
        [RegularExpression("(^[0-9]*$)", ErrorMessage = ("Only sequence of numbers are allowed for access token"))]
        public string AccessToken { get; set; }
    }
}