using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers.New;
using DataAccess.Models;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using SharedLibrary.Shared.ApiModels;
using ZolikyWeb.Areas.App.Models;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.App.Controllers
{
	[Authorize]
	public class MainController : Controller
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public async Task<ActionResult> Dashboard()
		{
			var rankMgr = this.GetManager<RankManager>();
			var uMgr = this.GetManager<UserManager>();
			var user = await this.GetLoggedUserAsync();
			var isTester = this.User.Identity.IsTester();
			var zoliks = user.OriginalZoliks.Where(x => x.Enabled).OrderByDescending(x => x.OwnerSince).ToList();
			var className = user.ClassName;
			IList<GetTopStudents_Result> studentsClass = new List<GetTopStudents_Result>();
			IList<GetTopStudents_Result> students = new List<GetTopStudents_Result>();
			if (user.ClassID != null) {
				studentsClass = uMgr.GetStudentsWithMostZoliks(5, user.ClassID, 1);
			}
			if (!user.IsInRole(UserRoles.Public)) {
				students = uMgr.GetStudentsWithMostZoliks(5, null, 1);
			}

			var count = zoliks.CountZoliks(isTester);
			var jokerCount = zoliks.CountZoliks(isTester, ZolikType.Joker);
			var top = zoliks.SelectZoliks(isTester).Take(3);
			var rank = await rankMgr.GetTitleAsync(user.XP);

			var model = new DashboardModel() {
				ZolikCount = count,
				JokerCount = jokerCount,
				Rank = rank,
				Zoliky = top,
				SpecialDate = new DateTime(2019, 06, 21),
				SpecialDateDesc = "Uzavření známek",
				ClassName = className,
				LeaderboardClass = studentsClass,
				Leaderboard = students
			};

			return View(model);
		}

		public ActionResult RefreshProfilePhotos()
		{
			Session.Add(Globals.CacheBurst, (int) DateTime.Now.Ticks);
			return RedirectToAction("Dashboard");
		}

		public ActionResult Mobile()
		{
			return View();
		}
	}
}