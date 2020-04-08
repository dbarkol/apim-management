using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Demo
{
    public static class CreateSubscription
    {        
        private static readonly string TenantId = Environment.GetEnvironmentVariable("TenantId");
        private static readonly string ClientId = Environment.GetEnvironmentVariable("ClientId");
        private static readonly string ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
        private static readonly string AzureSubscriptionId = Environment.GetEnvironmentVariable("AzureSubscriptionId");
        private static readonly HttpClient Client = new HttpClient();

        [FunctionName("CreateSubscription")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "subscription")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Create subscription triggered");

            // Read the body of the request
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (requestBody.Length == 0)
            {
                return new BadRequestObjectResult("Missing request body");
            }
            
            // Get the access token
            var token = await Utils.GetAccessToken(TenantId, ClientId, ClientSecret, log);

            // Create the subscription
            var subscription = JsonConvert.DeserializeObject<Subscription>(requestBody);
            var result = await CreateProductSubscription(subscription.ProductName,
                AzureSubscriptionId,
                subscription.SubscriptionDisplayName,
                subscription.SubscriptionId,
                subscription.ResourceGroupName,
                subscription.ApimServiceName,
                token,
                log);

            return (ActionResult)new OkObjectResult(result);
        }

        private static async Task<CreateSubscriptionResponse> CreateProductSubscription(string productName, 
                                                            string azureSubscriptionId, 
                                                            string subscriptionDisplayName,
                                                            string subscriptionId,
                                                            string resourceGroupName,
                                                            string serviceName,
                                                            string token,
                                                            ILogger log)
        {
            // Reference: https://docs.microsoft.com/en-us/rest/api/apimanagement/2019-01-01/subscription/createorupdate

            // Format the request URI
            var createSubscriptionUri = 
                $"https://management.azure.com/subscriptions/{azureSubscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ApiManagement/service/{serviceName}/subscriptions/{subscriptionId}?api-version=2019-01-01";

            // Clear the request headers, set the content type
            // and add the bearer token.
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Remove("Authorization");
            Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // Update the body of the request with the subscription information.
            // The product subscription ID is already passed into the request as
            // a parameter, the rest of the data is passed into the body.
            var requestBody = new CreateSubscriptionRequest
            {
                Properties = new Properties
                {
                    DisplayName = subscriptionDisplayName,
                    Scope =
                        $"/subscriptions/{azureSubscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ApiManagement/service/{serviceName}/products/{productName}"
                }
            };

            // Serialize and encode the request body
            var jsonRequest = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Make the request to create the product subscription
            var response = await Client.PutAsync(createSubscriptionUri, content);
            var result = await response.Content.ReadAsStringAsync();

            // Format the response and return
            var json = JObject.Parse(result);
            var createSubscriptionResponse = new CreateSubscriptionResponse
            {
                PrimaryKey = json["properties"]["primaryKey"].ToString(),
                SecondaryKey = json["properties"]["secondaryKey"].ToString()
            };

            return createSubscriptionResponse;
        }

    }
}
