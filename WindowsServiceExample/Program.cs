﻿using Doc.ECM.Extension.SyncExample.Traitments;

namespace WindowsServiceExample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            #if !DEBUG
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new WindowsService() 
                };
                ServiceBase.Run(ServicesToRun);
            #else
                // For development and debug
                var yourCompanyExecutionProcess = new TraitmentSyncInternalTableData();
                yourCompanyExecutionProcess.SyncTables();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            #endif
        }
    }
}
