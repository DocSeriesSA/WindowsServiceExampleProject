using FluentScheduler;
using System;
using WindowsServiceExample.ServiceLogger;

namespace WindowsServiceExample.Execution
{
    public class JobRegistry : Registry
    {
        private readonly LogHelper serviceLog = new LogHelper("YourCompanyService");

        public JobRegistry(int intervalInMinutes)
        {
            try
            {
                Schedule<ExportJob>().ToRunNow().AndEvery(intervalInMinutes).Minutes();
                serviceLog.Log(LogLevel.Info, "Job scheduled!");
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