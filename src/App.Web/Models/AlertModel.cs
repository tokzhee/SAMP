using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class AlertModel
    {
        public AlertModel()
        {

        }

        public AlertModel(string message, int messageCategory)
        {
            Message = message;
            MessageCategory = messageCategory;
        }

        public AlertModel(string messageSalutation, string message, int messageCategory)
        {
            MessageSalutation = messageSalutation;
            Message = message;
            MessageCategory = messageCategory;
        }

        public string MessageSalutation { get; set; }
        public string Message { get; set; }
        public int MessageCategory { get; set; }
        
    }
}