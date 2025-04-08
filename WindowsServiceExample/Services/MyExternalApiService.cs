using Doc.ECM.Extension.SyncExample.Models;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using WindowsServiceExample.ConfigHelper;
using WindowsServiceExample.ServiceLogger;

namespace Doc.ECM.Extension.SyncExample.Services
{
    internal static class MyExternalApiService
    {
        private static LogHelper LogHelper = new LogHelper("MyExternalApiService");

        private static string _apiUrl;
        private static string _username;
        private static string _password;
        private static string _token;

        /// <summary>
        /// Initialize the service with configuration
        /// </summary>
        public static void Initialize(YourAPIConfig config)
        {
            _apiUrl = config.MyAPIApiUrl;
            _username = config.MyAPIUsername;
            _password = config.MyAPIPassword;
        }

        /// <summary>
        /// Get new auth token, implement the logic necessary for your own API
        /// </summary>
        private static async Task GetTokenAsync()
        {
            try
            {
                var requestData = new { username = _username, password = _password };
                var jsonData = JsonConvert.SerializeObject(requestData);

                RestClient client = new RestClient($"{_apiUrl}/api/token");
                RestRequest request = new RestRequest(Method.POST);

                var response = client.Post(request);

                var responseContent = response.Content;
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                _token = tokenResponse?.Token;
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogLevel.Error, $"There was en error obtaining a valid token. {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Send API request with retry policy to handle token expiration
        /// </summary>
        public static T ExecuteApiRequest<T>(string url, Method method, object body = null)
        {
            RestClient client = new RestClient($"{_apiUrl}/api/{url}");
            RestRequest request = new RestRequest(method);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", $"bearer {_token}");
            request.Timeout = 600000;

            if (body != null)
            {
                string jsonBody = JsonConvert.SerializeObject(body);
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            }

            IRestResponse response = RestResponseWithPolicy(client, request);

            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }
        }

        private static IRestResponse RestResponseWithPolicy(RestClient restClient, RestRequest restRequest)
        {
            var retryPolicy = Policy
                .HandleResult<IRestResponse>(x => !x.IsSuccessful && x.StatusCode == HttpStatusCode.Unauthorized)
                .WaitAndRetry(3, x => TimeSpan.FromSeconds(2), async (response, timeSpan, retryCount, context) =>
                {
                    await GetTokenAsync();
                    restRequest.AddOrUpdateHeader("authorization", $"bearer {_token}");
                });

            return retryPolicy.Execute(() => restClient.Execute(restRequest));
        }
    }
}
