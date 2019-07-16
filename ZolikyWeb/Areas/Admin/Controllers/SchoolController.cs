using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using ZolikyWeb.Models.Base;

namespace ZolikyWeb.Areas.Admin.Controllers
{
    public class SchoolController : OwnController
    {
        // GET: Admin/School/Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }

		// GET: Admin/School/Create
		public ActionResult Create()
		{
			return View();
		}

		// GET: Admin/School/Edit
		public ActionResult Edit()
		{
			return View();
		}
	}
}