using Doc.ECM.APIHelper;
using Doc.ECM.APIHelper.DTO;
using System.Collections.Generic;
using System;
using WindowsServiceExample.ConfigHelper;
using WindowsServiceExample.ServiceLogger;
using System.Linq;

namespace WindowsServiceExample.Execution
{
    internal class YourCompanyExecutionProcess
    {
        private static LogHelper LogHelper = new LogHelper("YourCompanyService");

        private YourCompanyConfig Config;

        private readonly object _invoiceExportLock = new object();

        public YourCompanyExecutionProcess()
        {
            LogHelper.Log(LogLevel.Info, "Initializing and Loading Config...");

            Config = YourCompanyConfigHelper.LoadConfig();

            LogHelper.Log(LogLevel.Info, "Settings Doc.ECM Helper parameters...");

            DocECMApiHelper.SetParameters(Config.DocECMParameters);
        }

        public void InvoiceExport()
        {
            lock (_invoiceExportLock)
            {
                LogHelper.Log(LogLevel.Info, "Starting invoice exporting process.");

                ProcessReadyToExportDocuments();

                LogHelper.Log(LogLevel.Info, "Ending invoice exporting and saving the new config.");

                YourCompanyConfigHelper.SaveConfig(Config);
            }
        }

        private void ProcessReadyToExportDocuments()
        {
            //Get invoices to export
            Dictionary<int, string> errorList = new Dictionary<int, string>();
            UserDTO currentUser = DocECMApiHelper.GetCurrentUser();

            string searchPattern = $"YourContentTypeCode|s04|Test|string";
            string contentTypeIds = "2,5,7";
            LogHelper.Log(LogLevel.Info, "Searching documents in 'Export' status for invoice generation...");

            try
            {
                List<SearchResultsDTO> results = DocECMApiHelper.AdvancedSearch(searchPattern, contentTypeIds);
                LogHelper.Log(LogLevel.Info, $"{results.Count} documents found for invoice generation");
                foreach (SearchResultsDTO result in results)
                {
                    try
                    {
                        //Set document as processed
                        FieldsDTO newStatusField = result.Fields.FirstOrDefault(f => f.Code == "InProgress");
                        newStatusField.Value = "Done";
                        ObjectsDTO newObj = DocECMApiHelper.SaveDocument(new ObjectsDTO
                        {
                            ObjectID = result.ObjectID,
                            Fields = result.Fields,
                            ContentTypeID = result.ContentTypeID
                        });
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Log(LogLevel.Error, $"Error processing document in 'Export' with ID: {result.ObjectID}: {ex}");
                        errorList.Add(result.ObjectID, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogLevel.Error, $"Error searching for documents to 'Export': {ex}");
            }

            LogHelper.Log(LogLevel.Info, "Finished process: Searching documents in 'Export' status for invoice generation...");
        }
    }
}