using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Models
{
    public class CreateSubscriptionResponse
    {
        public string PrimaryKey { get; set; }

        public string SecondaryKey { get; set; }
    }
}



//public class Properties
//{
//    public string ownerId { get; set; }
//    public string scope { get; set; }
//    public string displayName { get; set; }
//    public string state { get; set; }
//    public DateTime createdDate { get; set; }
//    public string primaryKey { get; set; }
//    public string secondaryKey { get; set; }
//}

//public class RootObject
//{
//    public string id { get; set; }
//    public string type { get; set; }
//    public string name { get; set; }
//    public Properties properties { get; set; }
//}