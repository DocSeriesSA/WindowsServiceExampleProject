using FluentScheduler;
using System.ServiceProcess;
using WindowsServiceExample.ConfigHelper;
using WindowsServiceExample.Execution;
using WindowsServiceExample.ServiceLogger;

namespace WindowsServiceExample
{
    public partial class WindowsService: ServiceBase
    {
        private readonly LogHelper serviceLog = new LogHelper("YourCompanyService");

        public WindowsService()
        {
            serviceLog.Log(LogLevel.Info, "Service initializing...");
            InitializeComponent();
            serviceLog.Log(LogLevel.Info, "Service initialized!");
        }

        protected override void OnStart(string[] args)
        {
            serviceLog.Log(LogLevel.Info, "Service starting...");
            InitService();
            serviceLog.Log(LogLevel.Info, "Service started!");
        }
 
        protected override void OnStop()
        {
            JobManager.Stop();
        }

        public void InitService()
        {
            serviceLog.Log(LogLevel.Info, "Loading config...");
            var configObj = YourCompanyConfigHelper.LoadConfig();
            serviceLog.Log(LogLevel.Info, "Config loaded!");
            serviceLog.Log(LogLevel.Info, "Registering Job...");
            JobManager.Initialize(new JobRegistry(configObj.ProcessParameters.JobScheduleInMinutes));
            serviceLog.Log(LogLevel.Info, "Job registered!");
        }
    }
}
