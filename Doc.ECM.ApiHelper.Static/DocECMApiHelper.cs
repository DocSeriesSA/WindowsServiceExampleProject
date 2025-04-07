using Doc.ECM.APIHelper.DTO;
using Doc.ECM.APIHelper.Models;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Doc.ECM.APIHelper
{
    public static class DocECMApiHelper
    {
        public static string ApiToken = "";
        private static string ApiURL = "";
        private static string WebSiteURL = "";
        private static string Username = "";
        private static string Password = "";

        private const int maxRetryAttempts = 5;
        private static TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(2);

        private static object _lock = new object();
        private static object _tokenLock = new object();

        public static void SetParameters(DocECMAPIParametersDTO apiParams)
        {
            ApiURL = apiParams.ApiUrl;
            Username = apiParams.Username;
            Password = apiParams.Password;
            WebSiteURL = apiParams.WebSiteUrl;

            GetToken();
        }

        public static void GetToken()
        {
            lock (_tokenLock)
            {
                RestClient client = new RestClient($"{ApiURL}/token");
                RestRequest request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/x-www-form-urlencoded", $"username={Username}&password={Password}&grant_type=password", ParameterType.RequestBody);
                IRestResponse<DocECMAPITokenResponse> response = client.Execute<DocECMAPITokenResponse>(request);
                if (response.IsSuccessful)
                {
                    ApiToken = response.Data.access_token;


                }
                else
                {
                    throw new Exception(response.Content);
                }
            }
        }

        public static UserDTO GetCurrentUser()
        {
            return ExecuteDocECMApiRequest<UserDTO>($"account/current", Method.GET);
        }
        public static List<UserDTO> GetUsersList()
        {
            return ExecuteDocECMApiRequest<List<UserDTO>>($"account/get-users-list", Method.GET);
        }
        public static int? GetIdByEmail(string email)
        {
            return ExecuteDocECMApiRequest<int?>($"account/get-id-by-email?email={email}", Method.GET);
        }
        public static bool ActivateUser(int userId)
        {
            return ExecuteDocECMApiRequest<bool>($"account/activate-external/{userId}", Method.GET);
        }
        public static List<ListItemDTO> GetContentTypes()
        {
            return ExecuteDocECMApiRequest<List<ListItemDTO>>($"content-type/list", Method.GET);
        }
        public static List<ListItemDTO> GetInternalTablesList()
        {
            return ExecuteDocECMApiRequest<List<ListItemDTO>>($"internal-table/get-list", Method.GET);
        }
        public static DynamicTableDTO GetInternalTable(int id)
        {
            return ExecuteDocECMApiRequest<DynamicTableDTO>($"internal-table/get/{id}", Method.GET);
        }
        public static bool SaveInternalTable(DynamicTableDTO internalTable)
        {
            return ExecuteDocECMApiRequest<bool>($"internal-table/save", Method.POST, JsonConvert.SerializeObject(internalTable));
        }
        public static ContentTypeDTO GetContentTypeById(int id)
        {
            return ExecuteDocECMApiRequest<ContentTypeDTO>($"content-type/get-by-id/{id}", Method.GET);
        }
        public static bool SaveContentType(ContentTypeDTO contentType)
        {
            string json = JsonConvert.SerializeObject(contentType);
            ContentTypeJsonData jsonData = new ContentTypeJsonData();
            jsonData.ContentTypeStr = json;
            return ExecuteDocECMApiRequest<bool>($"content-type/save-external", Method.POST, JsonConvert.SerializeObject(jsonData));
        }
        public static SettingDTO GetOrganizationSettings()
        {
            return ExecuteDocECMApiRequest<SettingDTO>("organization/get-settings", Method.GET);
        }

        public static SettingDTO SaveOrganizationSettings(SettingDTO settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            return ExecuteDocECMApiRequest<SettingDTO>("organization/save-settings", Method.POST, json);
        }
        public static UserDTO SaveUserOnExternal(UserDTO user)
        {
            string json = JsonConvert.SerializeObject(user);
            return ExecuteDocECMApiRequest<UserDTO>("account/save", Method.POST, json);
        }
        public static List<SearchResultsDTO> AdvancedSearch(string condition, string contentTypeIDs = "")
        {
            AdvancedSearchRequest searchRequest = new AdvancedSearchRequest
            {
                searchPattern = condition,
                contentTypeIDs = contentTypeIDs
            };

            string json = JsonConvert.SerializeObject(searchRequest);
            return ExecuteDocECMApiRequest<List<SearchResultsDTO>>("search/advanced", Method.POST, json);
        }
        public static ObjectsDTO GetDocument(int objectID)
        {
            return ExecuteDocECMApiRequest<ObjectsDTO>($"document/{objectID}/metadata", Method.GET);
        }
        public static List<ObjectVersionDTO> GetDocumentVersions(int objectID)
        {
            return ExecuteDocECMApiRequest<List<ObjectVersionDTO>>($"document/{objectID}/versions", Method.GET);
        }
        public static AttachmentsDTO GetDocumentAttachmentPDF(int objectID)
        {
            return ExecuteDocECMApiRequest<AttachmentsDTO>($"document/{objectID}/display", Method.GET);
        }
        public static AttachmentsDTO GetDocumentAttachment(int objectID)
        {
            return ExecuteDocECMApiRequest<AttachmentsDTO>($"document/{objectID}/attachment", Method.GET);
        }
        public static List<DocumentAttachmentDTO> GetDocumentRealAttachmentList(int objectID)
        {
            return ExecuteDocECMApiRequest<List<DocumentAttachmentDTO>>($"document-attachment/get-list/{objectID}", Method.GET);
        }
        public static byte[] GetDocumentRealAttachmentPDF(int attachmentId)
        {
            return ExecuteDocECMApiRequestForFile($"document-attachment/display/{attachmentId}", Method.GET);
        }
        public static int ValidateDocument(int objectID)
        {
            return ExecuteDocECMApiRequest<int>($"flow/validate/{objectID}", Method.POST);
        }
        public static List<ImputationDTO> GetImputations(int objectID, string dbTableName)
        {
            return ExecuteDocECMApiRequest<List<ImputationDTO>>($"plugin/get-imputations/{objectID}?dbTableName={dbTableName}", Method.GET);
        }
        public static List<ImputationDTO> SaveImputations(List<ImputationDTO> imputations, string dbTableName)
        {
            ImputationsSaveRequestModel saveImputationsRequest = new ImputationsSaveRequestModel
            {
                dbTableName = dbTableName,
                imputations = imputations,
                deleteNotInList = true,
            };

            string json = JsonConvert.SerializeObject(saveImputationsRequest);
            return ExecuteDocECMApiRequest<List<ImputationDTO>>("plugin/save-imputations", Method.POST, json);
        }
        public static ObjectsDTO SaveDocument(ObjectsDTO doc)
        {
            //CHECK STATIC LIST FIELDS
            foreach (FieldsDTO field in doc.Fields.Where(f => f.Type == FieldType.StaticList))
            {
                if (!string.IsNullOrEmpty(field.Value) && !int.TryParse(field.Value, out int n))
                {
                    ListElementsDTO listElement = field.ListElements?.FirstOrDefault(fle => fle.DisplayValue == field.Value);
                    if (listElement != null)
                    {
                        field.Value = listElement.Id;
                    }
                }
            }
            string json = JsonConvert.SerializeObject(doc);
            return ExecuteDocECMApiRequest<ObjectsDTO>("document/save", Method.POST, json);
        }
        public static ObjectsDTO ImportDocument(ObjectsDTO doc)
        {
            string json = JsonConvert.SerializeObject(doc);
            return ExecuteDocECMApiRequest<ObjectsDTO>("document/import", Method.POST, json);
        }
        public static List<CommentDTO> GetComments(int objectID)
        {
            return ExecuteDocECMApiRequest<List<CommentDTO>>($"document/{objectID}/comments", Method.GET);
        }
        public static List<InternalTableRowDTO> GetOrCreateDynamicTableData(string tableName, List<string> columnNames, bool getData = true)
        {
            PluginActionDTO action = new PluginActionDTO
            {
                Action = 1,
                Data = JsonConvert.SerializeObject(new CreateOrUpdateInternalTableDTO
                {
                    TableName = tableName,
                    ColumnNames = columnNames,
                })
            };
            string json = JsonConvert.SerializeObject(action);
            ExecuteDocECMApiRequest<string>($"plugin/execute-action", Method.POST, json);
            if (getData)
            {
                action = new PluginActionDTO
                {
                    Action = 2,
                    Data = JsonConvert.SerializeObject(new GetDataInternalTableDTO
                    {
                        TableName = tableName,
                    })
                };
                json = JsonConvert.SerializeObject(action);
                string internalTabeDataJSON = ExecuteDocECMApiRequest<string>($"plugin/execute-action", Method.POST, json);
                return JsonConvert.DeserializeObject<List<InternalTableRowDTO>>(internalTabeDataJSON);
            }
            else
            {
                return new List<InternalTableRowDTO>();
            }
        }
        public static List<InternalTableRowDTO> GetDataInternalTable(GetDataInternalTableDTO obj)
        {
            PluginActionDTO action = new PluginActionDTO
            {
                Action = 2,
                Data = JsonConvert.SerializeObject(obj)
            };
            string json = JsonConvert.SerializeObject(action);
            string internalTabeDataJSON = ExecuteDocECMApiRequest<string>($"plugin/execute-action", Method.POST, json);
            return JsonConvert.DeserializeObject<List<InternalTableRowDTO>>(internalTabeDataJSON);
        }
        public static void SaveDynamicTableData(string tableName, List<InternalTableRowDTO> rows)
        {
            if (rows.Count > 0)
            {
                PluginActionDTO action = new PluginActionDTO
                {
                    Action = 3,
                    Data = JsonConvert.SerializeObject(new SaveDataInternalTableDTO
                    {
                        TableName = tableName,
                        Rows = rows,
                    })
                };
                string json = JsonConvert.SerializeObject(action);
                ExecuteDocECMApiRequest<string>($"plugin/execute-action", Method.POST, json);
            }
        }
        public static CommentDTO SaveComment(CommentDTO obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return ExecuteDocECMApiRequest<CommentDTO>($"comment/save", Method.POST, json);
        }
        public static string GetDocumentIDSearchURL(int[] objectIds)
        {
            string baseUrl = WebSiteURL;
            if (baseUrl.EndsWith("/"))
            {
                baseUrl = baseUrl.Remove(baseUrl.Length - 1);
            }
            return $"{baseUrl}/#/search/document-ids/{string.Join(",", objectIds)}";
        }
        public static ReadUsersResponseDTO ReadUsers()
        {
            string json = JsonConvert.SerializeObject(new DataSourceLoadOptionsBase());
            return ExecuteDocECMApiRequest<ReadUsersResponseDTO>($"user/get-grid", Method.POST, json);
        }
        public static UserDTO SaveUser(UserDTO obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return ExecuteDocECMApiRequest<UserDTO>($"user/save", Method.POST, json);
        }

        private static byte[] ExecuteDocECMApiRequestForFile(string url, Method method, string jsonBody = "")
        {
            RestClient client = new RestClient($"{ApiURL}/api/{url}");
            RestRequest request = new RestRequest(method);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", $"bearer {ApiToken}");
            request.Timeout = 600000;
            if (!string.IsNullOrEmpty(jsonBody))
            {
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            }
            //IRestResponse response = client.Execute(request);
            IRestResponse response = RestResponseWithPolicy(client, request);

            if (response.IsSuccessful)
            {
                return response.RawBytes;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                lock (_lock)
                {
                    GetToken();
                }

                return ExecuteDocECMApiRequestForFile(url, method, jsonBody);
            }
            else
            {
                throw new Exception(response.Content);
            }
        }

        private static T ExecuteDocECMApiRequest<T>(string url, Method method, string jsonBody = "", bool isFile = false)
        {
            RestClient client = new RestClient($"{ApiURL}/api/{url}");
            RestRequest request = new RestRequest(method);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", $"bearer {ApiToken}");
            request.Timeout = 600000;
            if (!string.IsNullOrEmpty(jsonBody))
            {
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            }
            //IRestResponse response = client.Execute(request);
            IRestResponse response = RestResponseWithPolicy(client, request);

            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                lock (_lock)
                {
                    GetToken();
                }

                return ExecuteDocECMApiRequest<T>(url, method, jsonBody);
            }
            else
            {
                throw new Exception(response.Content);
            }
        }

        private static IRestResponse RestResponseWithPolicy(RestClient restClient, RestRequest restRequest)
        {
            var retryPolicy = Policy
                .HandleResult<IRestResponse>(x => !x.IsSuccessful)
                .WaitAndRetry(maxRetryAttempts, x => pauseBetweenFailures, async (response, timeSpan, retryCount, context) =>
                {
                });

            return retryPolicy.Execute(() => restClient.Execute(restRequest));
        }

        private class DocECMAPITokenResponse
        {
            public string access_token { get; set; }
        }

        private class AdvancedSearchRequest
        {
            public string searchPattern { get; set; }
            public string contentTypeIDs { get; set; }
        }

        private class ImputationsSaveRequestModel
        {
            public string dbTableName { get; set; }
            public List<ImputationDTO> imputations { get; set; }
            public bool deleteNotInList { get; set; } = true;
        }
    }
}
