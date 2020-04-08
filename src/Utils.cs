using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Demo.Models;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Demo
{
    internal static class Utils
    {
        internal static async Task<HttpResponseMessage> DeleteSubscription(
                                                    HttpClient client,
                                                    string azureSubscriptionId,
                                                    string resourceGroupName,
                                                    string serviceName,
                                                    string subscriptionId,
                                                    string token,
                                                    ILogger log)
        {
            // Reference: https://docs.microsoft.com/en-us/rest/api/apimanagement/2019-01-01/subscription/delete

            var deleteSubscriptionUri =
                $"https://management.azure.com/subscriptions/{azureSubscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ApiManagement/service/{serviceName}/subscriptions/{subscriptionId}?api-version=2019-01-01";

            // Clear the request headers, set the content type
            // and add the bearer token.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // Make the request to delete the subscription
            var response = await client.DeleteAsync(deleteSubscriptionUri);

            return response;
        }

        internal static async Task<object> GetSubscriptionbyId(
                                                        HttpClient client,
                                                        string azureSubscriptionId,
                                                        string resourceGroupName,
                                                        string serviceName,
                                                        string subscriptionId,
                                                        string token,
                                                        ILogger log)
        {
            // Reference: https://docs.microsoft.com/en-us/rest/api/apimanagement/2019-01-01/subscription/get
            // GET https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ApiManagement/service/{serviceName}/subscriptions/{sid}?api-version=2019-01-01

            // Format the request URI. The subscription ID is optional.
            if (subscriptionId == null)
                subscriptionId = string.Empty;

            var getSubscriptionUri =
                $"https://management.azure.com/subscriptions/{azureSubscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ApiManagement/service/{serviceName}/subscriptions/{subscriptionId}?api-version=2019-01-01";

            // Clear the request headers, set the content type
            // and add the bearer token.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // Make the request to retrieve the subscription information
            var response = await client.GetAsync(getSubscriptionUri);
            var result = await response.Content.ReadAsAsync<object>();

            return result;
        }

        internal static async Task<GetKeysResponse> GetSubscriptionKeys(
                                                        HttpClient client,
                                                        string azureSubscriptionId,
                                                        string resourceGroupName,
                                                        string serviceName,
                                                        string subscriptionId,
                                                        string token,
                                                        ILogger log)
        {
            // Reference: https://docs.microsoft.com/en-us/rest/api/apimanagement/2019-01-01/subscription/get
            
            // Format the request URI
            var getSubscriptionUri =
                $"https://management.azure.com/subscriptions/{azureSubscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ApiManagement/service/{serviceName}/subscriptions/{subscriptionId}?api-version=2019-01-01";

            // Clear the request headers, set the content type
            // and add the bearer token.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // Make the request to retrieve the subscription information
            var response = await client.GetAsync(getSubscriptionUri);
            var result = await response.Content.ReadAsStringAsync();

            // Format the response and return
            var json = JObject.Parse(result);
            var getKeysResponse = new GetKeysResponse
            {
                PrimaryKey = json["properties"]["primaryKey"].ToString(),
                SecondaryKey = json["properties"]["secondaryKey"].ToString()
            };

            return getKeysResponse;
        }

        internal static async Task<bool> RegenerateSubscriptionKey(
                                                        HttpClient client,
                                                        bool primaryKey,
                                                        string azureSubscriptionId,
                                                        string resourceGroupName,
                                                        string serviceName,
                                                        string subscriptionId,
                                                        string token,
                                                        ILogger log)
        {
            // References:
            // https://docs.microsoft.com/en-us/rest/api/apimanagement/2019-01-01/subscription/regenerateprimarykey
            // https://docs.microsoft.com/en-us/rest/api/apimanagement/2019-01-01/subscription/regeneratesecondarykey

            // Set action to regenerate primary or secondary key based on the passed in parameter
            var action = primaryKey ? "regeneratePrimaryKey" : "regenerateSecondaryKey";
            var regenerateKeyUri =
                $"https://management.azure.com/subscriptions/{azureSubscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ApiManagement/service/{serviceName}/subscriptions/{subscriptionId}/{action}?api-version=2019-01-01";

            // Clear the request headers, set the content type
            // and add the bearer token.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // Make the request to regenerate the key
            var response = await client.PostAsync(regenerateKeyUri, null);
            return response.IsSuccessStatusCode;
        }

        internal static async Task<string> GetAccessToken(string tenantId, string clientId, string clientKey, ILogger log)
        {
            log.LogInformation("Begin GetAccessToken");

            var authContextUrl = "https://login.windows.net/" + tenantId;
            var authenticationContext = new AuthenticationContext(authContextUrl);
            var credential = new ClientCredential(clientId, clientKey);
            var result = await authenticationContext
                .AcquireTokenAsync("https://management.azure.com/", credential);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }
    }
}
