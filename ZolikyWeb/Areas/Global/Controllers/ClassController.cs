using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using ZolikyWeb.Models.Base;

namespace ZolikyWeb.Areas.Global.Controllers
{
    public class ClassController : OwnController<ClassManager>
    {
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}