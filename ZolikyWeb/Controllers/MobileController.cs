using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Controllers
{
	[OfflineActionFilter(Active = false)]
    public class MobileController : Controller
	{
        // GET: Mobile
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Android()
		{
			return View();
		}

		public ActionResult Ios()
		{
			return RedirectPermanent("https://itunes.apple.com/cz/app/%C5%BEol%C3%ADky/id1463194810");
		}

    }
}