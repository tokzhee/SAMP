using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace App.SalaryAccountProfiler.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<SalaryAccountProfiler>(s =>
                {
                    s.ConstructUsing(profiler => new SalaryAccountProfiler());
                    s.WhenStarted(profiler => profiler.Start());
                    s.WhenStopped(profiler => profiler.Stop());
                });

                x.RunAsLocalSystem();
                x.UseLog4Net("log4net.config", true);

                x.SetServiceName("SalaryAccountManagementPortal.SalaryAccountProfiler");
                x.SetDisplayName("SalaryAccountManagementPortal.SalaryAccountProfiler");
                x.SetDescription("This service is responsible for profiling salary accounts to the finacle environment from where validation checks will be done");
            });
        }
    }
}
