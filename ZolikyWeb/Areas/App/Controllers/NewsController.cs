using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Managers;
using ZolikyWeb.Models.Base;

namespace ZolikyWeb.Areas.App.Controllers
{
    public class NewsController : OwnController<NewsManager>
    {
		public async Task<ActionResult> Dashboard()
		{
			var news = await Mgr.GetAllActive();
			return View(news);
		}
    }
}