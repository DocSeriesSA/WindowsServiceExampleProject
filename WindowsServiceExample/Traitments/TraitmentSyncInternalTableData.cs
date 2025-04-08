using Doc.ECM.APIHelper.DTO;
using Doc.ECM.APIHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Doc.ECM.Extension.SyncExample.Models;
using Doc.ECM.Extension.SyncExample.Services;
using WindowsServiceExample.ServiceLogger;
using WindowsServiceExample.ConfigHelper;

namespace Doc.ECM.Extension.SyncExample.Traitments
{
    internal class TraitmentSyncInternalTableData
    {
        private static LogHelper LogHelper = new LogHelper("TraitmentSyncInternalTableData");

        private YourCompanyConfig Config;

        public TraitmentSyncInternalTableData()
        {
            LogHelper.Log(LogLevel.Info, "Initializing and Loading Config...");

            Config = YourCompanyConfigHelper.LoadConfig();

            LogHelper.Log(LogLevel.Info, "Settings Doc.ECM Helper parameters...");

            DocECMApiHelper.SetParameters(Config.DocECMParameters);

            MyExternalApiService.Initialize(Config.YourAPIConfig);
        }
        /// <summary>
        /// Main process for the job that will sync internal tables with external data
        /// </summary>
        public void SyncTables()
        {
            SyncMiTableData();
        }

        /// <summary>
        /// Syncs the internal table "Paiement_Fournisseur" with external data, filtering only active payments.
        /// </summary>
        private void SyncMiTableData()
        {
            GetData<MyBusinessDataForInternalTable>("MyTable", "id", new List<string> { "id", "Description", "name" },
               () => MyExternalApiService.ExecuteApiRequest<List<MyBusinessDataForInternalTable>>("/myurl", RestSharp.Method.GET),
               item => item.Id.ToString(),
               item => new List<InternalTableCellDTO>
               {
            new InternalTableCellDTO
            {
                ColumnName = "Id_model",
                Value = item.Id.ToString(),
            },
            new InternalTableCellDTO
            {
                ColumnName = "name",
                Value = item.Name,
            },
            new InternalTableCellDTO
            {
               ColumnName = "description",
               Value = item.Description,
            }
               },
               item => item.IsActive && item.Amount > 0 
            );
        }


        /// <summary>
        /// Retrieves data from an external source, applies optional filtering, and updates or inserts records into an internal table.
        /// </summary>
        /// <typeparam name="T">The type of the objects retrieved from the external source.</typeparam>
        /// <param name="tableName">The name of the internal table in Doc.ECM to be updated.</param>
        /// <param name="idColumnName">The name of the column used as the unique identifier for records.</param>
        /// <param name="columnNames">A list of column names that define the structure of the table.</param>
        /// <param name="getObjectListFunc">A function that retrieves a list of objects from the external source.</param>
        /// <param name="getIdFunc">A function that extracts the unique identifier from each object.</param>
        /// <param name="createNewRowFunc">A function that converts an object into a list of table cells.</param>
        /// <param name="filterFunc">An optional function that filters the objects before processing. If null, all objects are processed.</param>
        private void GetData<T>(string tableName, string idColumnName, List<string> columnNames, Func<List<T>> getObjectListFunc, Func<T, string> getIdFunc, Func<T, List<InternalTableCellDTO>> createNewRowFunc, Func<T, bool> filterFunc = null)
        {
            try
            {
                LogHelper.Log(LogLevel.Info, $"starting sync table ");
                List<InternalTableRowDTO> data = DocECMApiHelper.GetOrCreateDynamicTableData(tableName, columnNames);
                List<T> objects = getObjectListFunc();
                if (objects == null)
                {
                    LogHelper.Log(LogLevel.Error, $"Error getting data from source");
                    return;
                }

                foreach (T item in objects)
                {
                    if (filterFunc != null && !filterFunc(item))
                    {
                        continue; // Skip items that don't pass the filter
                    }

                    string idValue = getIdFunc(item);
                    List<InternalTableCellDTO> newDataRow = createNewRowFunc(item);

                    InternalTableRowDTO existingRow = new InternalTableRowDTO();

                    existingRow = data.FirstOrDefault(r => r.Cells.FirstOrDefault(c => c.ColumnName == idColumnName)?.Value == idValue);
                    if (existingRow != null)
                    {
                        existingRow.Cells = newDataRow;
                    }
                    else
                    {
                        data.Add(new InternalTableRowDTO
                        {
                            Id = "0",
                            Cells = newDataRow,
                        });
                    }
                }
                DocECMApiHelper.SaveDynamicTableData(tableName, data);
                LogHelper.Log(LogLevel.Info, $"Sync {tableName} finished");
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogLevel.Error, $"Process sync  terms failed with error: {ex.Message}");
            }
        }
    }
}