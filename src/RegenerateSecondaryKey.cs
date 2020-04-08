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
    public static class RegenerateSecondaryKey
    {
        private static readonly string TenantId = Environment.GetEnvironmentVariable("TenantId");
        private static readonly string ClientId = Environment.GetEnvironmentVariable("ClientId");
        private static readonly string ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
        private static readonly string AzureSubscriptionId = Environment.GetEnvironmentVariable("AzureSubscriptionId");
        private static readonly HttpClient Client = new HttpClient();

        [FunctionName("RegenerateSecondaryKey")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "regenerateSecondaryKey")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Regenerate secondary key triggered");

            // Read the body of the request
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (requestBody.Length == 0)
            {
                return new BadRequestObjectResult("Missing request body");
            }

            // Get the access token
            var token = await Utils.GetAccessToken(TenantId, ClientId, ClientSecret, log);

            // Regenerate the key
            var subscription = JsonConvert.DeserializeObject<Subscription>(requestBody);
            var result = await Utils.RegenerateSubscriptionKey(Client,
                    false,
                    AzureSubscriptionId,
                    subscription.ResourceGroupName,
                    subscription.ApimServiceName,
                    subscription.SubscriptionId,
                    token,
                    log);

            var subscriptionKeys = await Utils.GetSubscriptionKeys(Client, AzureSubscriptionId, subscription.ResourceGroupName, subscription.ApimServiceName, subscription.SubscriptionId, token, log);
            return (ActionResult)new OkObjectResult(subscriptionKeys);
        }
    }
}
