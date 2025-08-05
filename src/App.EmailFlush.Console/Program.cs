using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace App.EmailFlush.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<EmailFlush>(s =>
                {
                    s.ConstructUsing(emailFlush => new EmailFlush());
                    s.WhenStarted(emailFlush => emailFlush.Start());
                    s.WhenStopped(emailFlush => emailFlush.Stop());
                });

                x.RunAsLocalSystem();
                x.UseLog4Net("log4net.config", true);

                x.SetServiceName("SalaryAccountManagementPortal.EmailFlush");
                x.SetDisplayName("SalaryAccountManagementPortal.EmailFlush");
                x.SetDescription("This service is responsible for sending emails for Salary Account Management Portal");
            });
        }
    }
}
