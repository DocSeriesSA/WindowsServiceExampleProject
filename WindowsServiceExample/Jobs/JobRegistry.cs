using FluentScheduler;
using System;
using WindowsServiceExample.ServiceLogger;

namespace WindowsServiceExample.Jobs
{
    public class JobRegistry : Registry
    {
        private readonly LogHelper serviceLog = new LogHelper("YourCompanyService");

        /// <summary>
        /// This is the job registry, it will register all the jobs that need to be run and their intervals,
        /// make sure to calculate thi time the job takes to run, so you don't have overlapping jobs
        /// </summary>
        public JobRegistry(int intervalInMinutes)
        {
            try
            {
                Schedule<JobExportInvoices>().ToRunNow().AndEvery(Convert.ToInt32(intervalInMinutes)).Minutes();
                serviceLog.Log(LogLevel.Info,$"Traitment export documents jobs to run now and every {intervalInMinutes} minutes");

                // Wait a minute before starting the next job. This is required to get one token for Doc.ECM, so they do not get invalidated.
                Schedule<JobSyncTables>().ToRunOnceAt(DateTime.Now.AddMinutes(1)).AndEvery(Convert.ToInt32(intervalInMinutes)).Minutes();
                serviceLog.Log(LogLevel.Info, $"Traitment sync documents paiement job to run at every {intervalInMinutes} minutes");
            }
            catch (Exception ex)
            {
                string error = $"There was an error programming the scheduler. Error: {ex.Message}";

                serviceLog.Log(LogLevel.Error, error);
                throw new Exception(error);
            }
        }
    }
}