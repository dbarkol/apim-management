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
    public static class GetSubscriptionById
    {
        private static readonly string TenantId = Environment.GetEnvironmentVariable("TenantId");
        private static readonly string ClientId = Environment.GetEnvironmentVariable("ClientId");
        private static readonly string ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
        private static readonly string AzureSubscriptionId = Environment.GetEnvironmentVariable("AzureSubscriptionId");
        private static readonly string ApimServiceName = Environment.GetEnvironmentVariable("ApimServiceName");
        private static readonly string ResourceGroupName = Environment.GetEnvironmentVariable("ResourceGroupName");
        private static readonly HttpClient Client = new HttpClient();

        [FunctionName("GetSubscriptionById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "subscriptions/{id?}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("GetSubscriptionById triggered");

            // Get the access token
            var token = await Utils.GetAccessToken(TenantId, ClientId, ClientSecret, log);

            // Retrieve the subscription
            var subscription = await Utils.GetSubscriptionbyId(Client, AzureSubscriptionId, ResourceGroupName, ApimServiceName, id, token, log);

            return (ActionResult)new OkObjectResult(subscription);
        }
    }
}
