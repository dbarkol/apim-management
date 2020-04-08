using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Models
{
    public class GetKeysResponse
    {
        public string PrimaryKey { get; set; }

        public string SecondaryKey { get; set; }

        /*
        {
    "id": "/subscriptions/845f733e-4587-4548-b46c-3e9a7d705fb5/resourceGroups/api-rg/providers/Microsoft.ApiManagement/service/hibp-demo/subscriptions/v4testsub4",
    "type": "Microsoft.ApiManagement/service/subscriptions",
    "name": "v4testsub4",
    "properties": {
        "scope": "/subscriptions/845f733e-4587-4548-b46c-3e9a7d705fb5/resourceGroups/api-rg/providers/Microsoft.ApiManagement/service/hibp-demo/products/beta",
        "displayName": "v4testsub4 display name",
        "state": "active",
        "createdDate": "2019-07-18T22:11:52.447Z",
        "startDate": null,
        "expirationDate": null,
        "endDate": null,
        "notificationDate": null,
        "primaryKey": "cc374f111e784df7b53d92bad4555eb0",
        "secondaryKey": "9314961dd20a413b9410d662266dad05",
        "stateComment": null
    }*/
    }
}

