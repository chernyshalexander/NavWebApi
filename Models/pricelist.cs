using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NavWebApi.Models
{   
    [DataContract]
    public class Item
    {
        [DataMember(Name = "nameeng")]
        public string Nameeng { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "price")]
        public string Price { get; set; }
        [DataMember(Name = "price2")]
        public string Price2 { get; set; }

    }
    public class Plist
    {
        public List<Item> pricelist { get; set; }
    }

}