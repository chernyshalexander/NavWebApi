using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NavWebApi.Models;

namespace NavWebApi.Controllers
{
    public class ItemsController : ApiController
    {
        // GET: api/Items/TobaccoPriceList
        [HttpGet]
        public /* Plist*/ IHttpActionResult TobaccoPriceList()
        {
           // return /PricelistModel.GetTobaccoPriceList();
            return Json(PricelistModel.GetTobaccoPriceList());
        }


    }
}
