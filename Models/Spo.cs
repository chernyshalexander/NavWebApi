using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NavWebApi.Models
{
    [DataContract]
    public class Spo
    {
        [DataMember(Name = "barcode")]
        public string Barcode { get; set; }

        [DataMember(Name = "art")]
        public string Art { get; set; }

        [DataMember(Name = "art_desc")]
        public string Art_desc { get; set; }

        [DataMember(Name = "price_wo_discount")]
        public string Price_wo_discount { get; set; }

        [DataMember(Name = "price_with_discount")]
        public string Price_with_discount { get; set; }

        [DataMember(Name = "disc_percent")]
        public string Disc_percent { get; set; }

        [DataMember(Name = "disc_no")]
        public string Disc_no { get; set; }

        [DataMember(Name = "disc_desc")]
        public string Disc_desc { get; set; }

        [DataMember(Name = "notes")]
        public string Notes { get; set; }

    }
    public class Slist
    {
        public List<Spo> SpoList { get; set; }
    }


}