using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Demo.Models
{
    public class Subscription
    {
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("resourceGroupName")]
        public string ResourceGroupName { get; set; }

        [JsonProperty("apimServiceName")]
        public string ApimServiceName { get; set; }

        [JsonProperty("subscriptionId")]
        public string SubscriptionId { get; set; }

        [JsonProperty("subscriptionDisplayName")]
        public string SubscriptionDisplayName { get; set; }
    }
}
