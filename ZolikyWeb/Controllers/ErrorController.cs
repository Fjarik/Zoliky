using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Controllers
{
	[AllowAnonymous]
	[OfflineActionFilter(Active = false)]
    public class ErrorController : Controller
	{
        public ActionResult Error404()
        {
            return View();
        }
    }
}