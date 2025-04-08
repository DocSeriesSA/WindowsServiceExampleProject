using Doc.ECM.Extension.SyncExample.Traitments;
using FluentScheduler;
using System;
using System.Web.Hosting;
using WindowsServiceExample.ServiceLogger;

namespace WindowsServiceExample.Jobs
{
    internal class JobExportInvoices : IJob, IRegisteredObject
    {
        private readonly LogHelper serviceLog = new LogHelper("JobExportInvoices");

        private readonly object _lock = new object();

        private bool _shuttingDown;

        public JobExportInvoices()
        {
            // Register this job with the hosting environment.
            //    // Allows for a more graceful stop of the job, in the case of IIS shutting down.
            HostingEnvironment.RegisterObject(this);
        }

        /// <summary>
        /// This method is called by the FluentScheduler when the job should start, define what the job should do here.
        /// </summary>
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
                    serviceLog.Log(LogLevel.Info, "Job traitment Started");
                    TraitmentExportInvoice traitment = new TraitmentExportInvoice();
                    traitment.ProcessDocumentsToExport();
                    serviceLog.Log(LogLevel.Info, "Job traitment Finished");
                }
            }
            catch (Exception ex)
            {
                serviceLog.Log(LogLevel.Error, $"Job Execution Error: {ex.Message}");
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