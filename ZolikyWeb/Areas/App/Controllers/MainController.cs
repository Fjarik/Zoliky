using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using SharedLibrary.Shared.ApiModels;
using ZolikyWeb.Areas.App.Models;
using ZolikyWeb.Areas.App.Models.Main;
using ZolikyWeb.Models.Base;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.App.Controllers
{
	[Authorize]
	public class MainController : OwnController<UserManager>
	{
		public ActionResult Index()
		{
			return RedirectToAction("Dashboard");
		}

		public async Task<ActionResult> Dashboard()
		{
			var rankMgr = this.GetManager<RankManager>();
			var user = await this.GetLoggedUserAsync();
			var schoolId = this.User.GetSchoolId();
			var isTester = this.User.Identity.IsTester();
			var zoliks = user.OriginalZoliks.Where(x => x.Enabled).OrderByDescending(x => x.OwnerSince).ToList();
			var className = user.ClassName;
			IList<GetTopStudents_Result> studentsClass = new List<GetTopStudents_Result>();
			IList<GetTopStudents_Result> students = new List<GetTopStudents_Result>();
			if (user.ClassID != null) {
				studentsClass = this.Mgr.GetStudentsWithMostZoliks(schoolId, 5, user.ClassID, 1);
			}
			if (!user.IsInRole(UserRoles.Public)) {
				students = this.Mgr.GetStudentsWithMostZoliks(schoolId, 5, null, 1);
			}

			var count = zoliks.CountZoliks(isTester);
			var jokerCount = zoliks.CountZoliks(isTester, ZolikType.Joker);
			var top = zoliks.SelectZoliks(isTester).Take(3);
			var rank = await rankMgr.GetTitleAsync(user.XP);

			var pMgr = this.GetManager<ProjectSettingManager>();
			var specDate = await pMgr.GetStringValueAsync(null, ProjectSettingKeys.SpecialDate) ??
						   DateTime.Now.ToString("dd.MM.yyyy");
			var specTitle = await pMgr.GetStringValueAsync(null, ProjectSettingKeys.SpecialText) ?? "";

			var model = new DashboardModel {
				ZolikCount = count,
				JokerCount = jokerCount,
				XP = user.XP,
				Rank = rank,
				Zoliky = top,
				SpecialDate = DateTime.Parse(specDate),
				SpecialDateDesc = specTitle,
				ClassName = className,
				LeaderboardClass = studentsClass,
				Leaderboard = students,
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

		// Cache - 5 minutes
		[OutputCache(Duration = 60 * 5, VaryByParam = "c")]
		public async Task<JsonResult> GetStatistics()
		{
			var userId = this.User.Identity.GetId();

			var sMgr = this.GetManager<StatisticsManager>();
			var all = await sMgr.GetAllAsync(userId);

			var data = all.Select(x => new {
				x.Key,
				x.Value
			});

			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public async Task<JsonResult> GetClassXpLeaderboard()
		{
			var user = await this.GetLoggedUserAsync();
			var schoolId = user.SchoolID;

			if (user.ClassID == null) {
				return Json("", JsonRequestBehavior.AllowGet);
			}

			var leaderboard = this.Mgr.GetStudentsWithMostXp(schoolId, 5, user.ClassID, 1);

			var rMgr = this.GetManager<RankManager>();
			var ranks = await rMgr.GetAllAsync();

			var data = leaderboard.Select(x => new {
				imgUrl = Url.GetProfilePhotoUrl(x.ID),
				fullName = x.FullName,
				xp = x.XP,
				rank = ranks.GetRankByXP(x.XP),
			}).ToList();

			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public async Task<JsonResult> GetXpLeaderboard()
		{
			var user = await this.GetLoggedUserAsync();
			var schoolId = user.SchoolID;

			if (user.IsInRole(UserRoles.Public)) {
				return Json("", JsonRequestBehavior.AllowGet);
			}

			var leaderboard = this.Mgr.GetStudentsWithMostXp(schoolId, 5, null, 1);

			var rMgr = this.GetManager<RankManager>();
			var ranks = await rMgr.GetAllAsync();

			var data = leaderboard.Select(x => new {
				imgUrl = Url.GetProfilePhotoUrl(x.ID),
				fullName = x.FullName,
				xp = x.XP,
				rank = ranks.GetRankByXP(x.XP),
			}).ToList();

			return Json(data, JsonRequestBehavior.AllowGet);
		}
	}
}