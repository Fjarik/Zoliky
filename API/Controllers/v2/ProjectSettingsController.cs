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
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using JetBrains.Annotations;
using Microsoft.Web.Http;
using SharedLibrary;
using SharedLibrary.Enums;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[OwnAuthorize]
	[RoutePrefix("projectSettings")]
	public class ProjectSettingsController : OwnApiController<ProjectSettingManager>
	{
		// GET: projectSettings/get
		[HttpGet]
		[Route("get")]
		[ResponseType(typeof(MActionResult<ProjectSetting>))]
		public async Task<IHttpActionResult> Get([FromUri] string key,
												 [FromUri] Projects? project = null)
		{
			if (string.IsNullOrWhiteSpace(key)) {
				return Ok(new MActionResult<ProjectSetting>(SharedLibrary.Enums.StatusCode.InvalidInput));
			}
			var res = await Mgr.GetAsync(project, key);
			return Ok(res);
		}

		// GET: projectSettings/getAll
		[HttpGet]
		[Route("getall")]
		[ResponseType(typeof(List<ProjectSetting>))]
		public async Task<IHttpActionResult> GetAll()
		{
			var res = await Mgr.GetAllAsync();
			return Ok(res);
		}
	}
}