using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using API.Tools;
using API.Tools.Annotations;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Models;
using Microsoft.Web.Http;
using SharedLibrary;
using SharedLibrary.Shared.Objects;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[OwnAuthorize]
	[RoutePrefix("achievement")]
	public class AchievementController : OwnApiController<Achievement, AchievementManager>
	{
		[HttpGet]
		[Route("getuserachivements")]
		[ResponseType(typeof(List<AchievementModel>))]
		public async Task<IHttpActionResult> GetUserAchivements()
		{
			var userId = this.User.Identity.GetId();
			if (userId < 1) {
				return Ok(new MActionResult<List<Zolik>>(SharedLibrary.Enums.StatusCode.NotValidID));
			}
			try {
				return Ok(await Mgr.GetUserAchievementModels(userId));
			} catch (Exception ex) {
				return Ok(new MActionResult<List<Zolik>>(SharedLibrary.Enums.StatusCode.SeeException, ex));
			}
		}
	}
}