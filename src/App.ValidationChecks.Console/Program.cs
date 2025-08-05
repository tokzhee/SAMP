using App.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace App.ValidationChecks.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ValidationChecks>(s =>
                {
                    s.ConstructUsing(check => new ValidationChecks());
                    s.WhenStarted(check => check.Start());
                    s.WhenStopped(check => check.Stop());
                });

                x.RunAsLocalSystem();
                x.UseLog4Net("log4net.config", true);

                x.SetServiceName($"SalaryAccountManagementPortal.ValidationChecks.{ConfigurationUtility.GetAppSettingValue("ServiceInstanceId")}");
                x.SetDisplayName($"SalaryAccountManagementPortal.ValidationChecks.{ConfigurationUtility.GetAppSettingValue("ServiceInstanceId")}");
                x.SetDescription("This service is responsible for doing the validation checks like BVN, CRMS and CRC");
            });
        }
    }
}
