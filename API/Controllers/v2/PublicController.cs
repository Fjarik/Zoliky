using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using API.Tools;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.Web.Http;
using SharedLibrary;
using SharedLibrary.Enums;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[AllowAnonymous]
	[RoutePrefix("public")]
	public class PublicController : OwnApiController<UnavailabilityManager>
	{
		// GET /public/wpfversion
		[HttpGet]
		[ResponseType(typeof(string))]
		[Route("wpfversion")]
		public async Task<IHttpActionResult> GetWpfVersion()
		{
			var mgr = this.GetManager<ProjectManager>();

			var res = await mgr.SelectAsync(Projects.WPF);
			if (!res.IsSuccess) {
				return BadRequest();
			}
			var project = res.Content;
			return Ok(project.Version);
		}

		// GET /public/time
		[HttpGet]
		[Route("time")]
		public IHttpActionResult GetTime()
		{
			return Ok($"Aktuální serverový čas je: {DateTime.Now.ToLongTimeString()}");
		}

		// GET /public/connection
		[HttpGet]
		[Route("connection")]
		public IHttpActionResult Connection()
		{
			return Ok(true);
		}

		// GET /public/dbconnection
		[HttpGet]
		[Route("dbconnection")]
		public async Task<IHttpActionResult> DbConnection()
		{
			try {
				using (ZoliksEntities ent = new ZoliksEntities()) {
					DbConnection conn = ent.Database.Connection;
					await conn.OpenAsync();
					conn.Close();
					conn.Dispose();
					return Ok(true);
				}
			} catch (Exception ex) {
				return InternalServerError(ex);
			}
		}

#region Status

		// GET /public/webstatus
		[HttpGet]
		[Route("webstatus")]
		[ResponseType(typeof(WebStatus))]
		public Task<IHttpActionResult> GetWebStatus()
		{
			return GetProjectStatusResultAsync((int) Projects.WebNew);
		}

		// GET /public/status
		[HttpGet]
		[Route("status")]
		[ResponseType(typeof(WebStatus))]
		public Task<IHttpActionResult> GetStatus(int projectId)
		{
			return GetProjectStatusResultAsync(projectId);
		}

		private async Task<IHttpActionResult> GetProjectStatusResultAsync(int projectId)
		{
			if (!Enum.IsDefined(typeof(Projects), projectId)) {
				return Ok(new WebStatus(PageStatus.NotAvailable, "Neplatné ID projektu"));
			}
			var res = await GetProjectStatusAsync(Projects.Api);
			if (!res.CanAccess) {
				return Ok(res);
			}
			res = await GetProjectStatusAsync((Projects) projectId);
			return Ok(res);
		}

		private async Task<WebStatus> GetProjectStatusAsync(Projects project)
		{
			var isUnavaible = await Mgr.IsUnavailableAsync(project);
			if (!isUnavaible) {
				return new WebStatus(PageStatus.Functional);
			}
			var res = Mgr.GetFirst(project);
			if (!res.IsSuccess) {
				return new WebStatus(PageStatus.Limited, "Vyskytla se chyba při zjištování dostupnosti");
			}
			var unvs = res.Content;
			if (unvs == null) {
				return new WebStatus(PageStatus.Limited, "Vyskytla se chyba při zjištování dostupnosti");
			}
			return new WebStatus(PageStatus.NotAvailable, null, unvs);
		}

#endregion
	}
}