using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NavWebApi.Models;
using System.Web.Http;

namespace NavWebApi.Controllers
{
    public class SpoController : ApiController
    {
    
        [HttpGet]
        public Slist SpoList(string barcode_string)
        {
            return SpoModel.GetSPO(barcode_string);
        }
        [HttpGet]
        public Slist SpoList2(string barcode_string)
        {
            return SpoModel.GetSPO2(barcode_string);
        }
    }
}