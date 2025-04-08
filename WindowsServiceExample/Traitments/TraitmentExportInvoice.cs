using Doc.ECM.APIHelper;
using Doc.ECM.APIHelper.DTO;
using Doc.ECM.Extension.SyncExample.Models;
using Doc.ECM.Extension.SyncExample.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using WindowsServiceExample.ConfigHelper;
using WindowsServiceExample.ServiceLogger;

namespace Doc.ECM.Extension.SyncExample.Traitments
{
    internal class TraitmentExportInvoice
    {
        private static LogHelper LogHelper = new LogHelper("TraitmentExportInvoice");

        private YourCompanyConfig Config;

        public TraitmentExportInvoice()
        {
            LogHelper.Log(LogLevel.Info, "Initializing and Loading Config...");

            Config = YourCompanyConfigHelper.LoadConfig();

            LogHelper.Log(LogLevel.Info, "Settings Doc.ECM Helper parameters...");

            DocECMApiHelper.SetParameters(Config.DocECMParameters);
        }

        /// <summary>
        /// Main process for the job that will search documents according to your business condition and export them to the external system
        /// </summary>
        public void ProcessDocumentsToExport()
        {
            try
            {
                LogHelper.Log(LogLevel.Info, "Starting export process");
                List<SearchResultsDTO> documents = SearchDocuments();
                Dictionary<int, string> errorList = new Dictionary<int, string>();
                if (documents.Count > 0)
                {
                    LogHelper.Log(LogLevel.Info, "Exporting documents");
                    foreach (var document in documents)
                    {
                        try
                        {
                            ExportDocument(document);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Log(LogLevel.Error, $"Error processing document {document.ObjectID}: {ex.Message}");
                            errorList.Add(document.ObjectID, ex.Message);
                        }
                    }

                    foreach (KeyValuePair<int, string> error in errorList)
                    {
                        //Add comment to the document
                        List<CommentDTO> comments = DocECMApiHelper.GetComments(error.Key);
                        if (!comments.Any(c => c.Text == error.Value))
                        {
                            DocECMApiHelper.SaveComment(new CommentDTO
                            {
                                ObjectID = error.Key,
                                Text = error.Value,
                            });
                        }
                        else
                        {
                            CommentDTO comentFound = comments.FirstOrDefault(c => c.Text == error.Value);
                            comentFound.Date = DateTime.Now.ToString();
                            DocECMApiHelper.SaveComment(comentFound);
                        }
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Info, "No documents to export");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogLevel.Error, "Error exporting documents: " + ex.Message);
            }
        }

        /// <summary>
        /// Logic to load the document creating your system model and export
        /// </summary>
        /// <param name="document"></param>
        private void ExportDocument(SearchResultsDTO document)
        {
            List<ImputationDTO> imputations = DocECMApiHelper.GetImputations(document.ObjectID, "imputations");
            MyBusinessInvoice invoice = new MyBusinessInvoice();
            invoice.ExternalId = document.ObjectID;
            var invoiceNumber = document.Fields.FirstOrDefault(f => f.Code == "InvoiceNumber");
            if (invoiceNumber != null)
            {
                invoice.InvoiceNumber = invoiceNumber.Value;
            }
            var invoiceDate = document.Fields.FirstOrDefault(f => f.Code == "InvoiceDate");
            if (invoiceDate != null)
            {
                invoice.InvoiceDate = DateTime.Parse(invoiceDate.Value);
            }
            //Continue by adding the rest of the fields
            foreach (ImputationDTO imputation in imputations)
            {
                MyBusinessInvoiceLine line = new MyBusinessInvoiceLine();
                var lineDescription = imputation.Fields.FirstOrDefault(f => f.Code == "LineDescription");
                if (lineDescription != null)
                {
                    line.Description = lineDescription.Value;
                }
                var lineQuantity = imputation.Fields.FirstOrDefault(f => f.Code == "LineQuantity");
                if (lineQuantity != null)
                {
                    line.Quantity = decimal.Parse(lineQuantity.Value);
                }
                //Continue by adding the rest of the fields for the line
            }

            //Export the invoice to your system
            var response = MyExternalApiService.ExecuteApiRequest<bool>("/invoices", Method.POST, invoice);
            if (response)
            {
                LogHelper.Log(LogLevel.Info, $"Document {document.ObjectID} exported successfully");
                UpdateDocumentStateOnDocECM(document, "Exported");
            }
            else
            {
                LogHelper.Log(LogLevel.Error, $"Error exporting document {document.ObjectID}");
            }
        }
        /// <summary>
        /// Update the state of the document on DocECM after being imported
        /// </summary>
        /// <param name="document"></param>
        /// <param name="state"></param>
        /// <exception cref="Exception"></exception>
        private void UpdateDocumentStateOnDocECM(SearchResultsDTO document, string state)
        {
            FieldsDTO fieldState = document.Fields.FirstOrDefault(f => f.Code == "state") ?? throw new Exception("Field state not found");
            fieldState.Value = state;

            DocECMApiHelper.SaveDocument(new ObjectsDTO
            {
                ObjectID = document.ObjectID,
                Fields = document.Fields,
                ContentTypeID = document.ContentTypeID,
            });
        }

        /// <summary>
        /// Search documents on DocECM according to your business criteria
        /// </summary>
        /// <returns></returns>
        private List<SearchResultsDTO> SearchDocuments()
        {
            List<SearchResultsDTO> results = new List<SearchResultsDTO>();
            try
            {
                string searchPattern = $"Your|search|criteria";
                LogHelper.Log(LogLevel.Info, $"Searching documents on state");
                results = DocECMApiHelper.AdvancedSearch(searchPattern);
                LogHelper.Log(LogLevel.Info, $"{results.Count} documents found");
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogLevel.Error, "Error searching for documents, error: " + ex.Message);
            }
            return results;
        }
    }
}
