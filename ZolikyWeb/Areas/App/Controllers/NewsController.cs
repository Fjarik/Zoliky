using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZolikyWeb.Models.Base;

namespace ZolikyWeb.Areas.App.Controllers
{
    public class NewsController : OwnController
    {
        // GET: App/News
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

		public ActionResult Dashboard()
		{
			return View();
		}
    }
}