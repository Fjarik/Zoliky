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
using SharedLibrary.Shared;
using SharedLibrary.Shared.ApiModels;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[OwnAuthorize]
	[RoutePrefix("statistics")]
	public class StatisticsController : OwnApiController<SchoolManager>
	{
#region Admin

		// GET: statistics/getStudentCount
		[HttpGet]
		[Route("getstudentcount")]
		[ResponseType(typeof(int))]
		[OwnAuthorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
		public async Task<IHttpActionResult> GetStudentCount()
		{
			var schoolId = this.User.Identity.GetSchoolId();
			var count = await Mgr.GetStudentCountAsync(schoolId);
			return Ok(count);
		}

		// GET: statistics/getZolikCount
		[HttpGet]
		[Route("getzolikcount")]
		[ResponseType(typeof(int))]
		[OwnAuthorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
		public async Task<IHttpActionResult> GetZolikCount()
		{
			var schoolId = this.User.Identity.GetSchoolId();
			var count = await Mgr.GetZolikCountAsync(schoolId);
			return Ok(count);
		}

		// GET: statistics/getTeacherCount
		[HttpGet]
		[Route("getteachercount")]
		[ResponseType(typeof(int))]
		[OwnAuthorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
		public async Task<IHttpActionResult> GetTeacherCount()
		{
			var schoolId = this.User.Identity.GetSchoolId();
			var count = await Mgr.GetTeacherCountAsync(schoolId);
			return Ok(count);
		}

		// GET: statistics/getZolikTypesData
		[HttpGet]
		[Route("getzoliktypesdata")]
		[ResponseType(typeof(List<ZolikTypesData>))]
		[OwnAuthorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
		public async Task<IHttpActionResult> GetZolikTypesData()
		{
			var schoolId = this.User.Identity.GetSchoolId();
			var res = await Mgr.GetZoliksGraphDataAsync(schoolId);
			return Ok(res);
		}

#endregion
	}
}