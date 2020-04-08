using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Demo.Models
{
    public class CreateSubscriptionRequest
    {
        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }

    public class Properties
    {
        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}
