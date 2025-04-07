using FluentScheduler;
using System;
using System.Web.Hosting;
using WindowsServiceExample.ServiceLogger;

namespace WindowsServiceExample.Execution
{
    public class ExportJob : IJob, IRegisteredObject
    {
        private readonly object _lock = new object();

        private bool _shuttingDown;
        private readonly LogHelper serviceLog = new LogHelper("YourCompanyService");

        public ExportJob()
        {
            // Register this job with the hosting environment.
            // Allows for a more graceful stop of the job, in the case of IIS shutting down.
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
            try
            {
                lock (_lock)
                {
                    if (_shuttingDown)
                    {
                        return;
                    }
                    serviceLog.Log(LogLevel.Info, "Starting export job...");
                    var yourCompanyExecutionProcess = new YourCompanyExecutionProcess();
                    yourCompanyExecutionProcess.InvoiceExport();
                    serviceLog.Log(LogLevel.Info, "Export job finished correctly");
                }
            }
            catch (Exception ex)
            {
                serviceLog.Log(LogLevel.Error, ex.Message);
                throw new Exception(ex.Message);
            }

        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
        }
    }
}
