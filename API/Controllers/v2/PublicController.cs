using System;
using System.Collections.Generic;
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
	public class PublicController : OwnApiController<ProjectManager>
	{
		// GET /public/wpfversion
		[HttpGet]
		[ResponseType(typeof(string))]
		[Route("wpfversion")]
		public async Task<IHttpActionResult> GetWpfVersion()
		{
			var res = await Mgr.SelectAsync(Projects.WPF);
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
				using (var ent = new ZoliksEntities()) {
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

#region Xml

		[HttpGet]
		[Route("userZoliksXml")]
		public async Task<IHttpActionResult> UserZolikCountXml([FromUri] int userId,
															   [FromUri] string password)
		{
			if (userId < 1) {
				return Ok();
			}
			if (password != "yCpJZrc18Dn5JSxE") {
				return Ok();
			}
			try {
				var zMgr = this.GetManager<ZolikManager>();
				var count = await zMgr.CountUserZoliksAsync(userId);

				var xml = GetXml(count);

				return Ok(xml);
			} catch {
				return Ok();
			}
		}

		private string GetXml(int count)
		{
			var path = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Tiles.xml");
			if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path)) {
				return string.Empty;
			}

			var content = System.IO.File.ReadAllText(path);

			content = FixXml(content, count);

			return content;
		}

		private string FixXml(string content, int count)
		{
			content = content.Replace("#Count#", count.ToString());
			content = content.Replace("\r", "");
			content = content.Replace("\n", "");
			content = content.Replace("\t", "");
			content = content.Replace("\\", "");
			content = content.Trim('"');
			return content;
		}

#endregion

#region Status

		// GET /public/webstatus
		[HttpGet]
		[Route("webstatus")]
		[ResponseType(typeof(WebStatus))]
		public IHttpActionResult GetWebStatus()
		{
			return Ok(new WebStatus(PageStatus.Functional));
		}

		// GET /public/status
		[HttpGet]
		[Route("status")]
		[ResponseType(typeof(WebStatus))]
		public IHttpActionResult GetStatus(int projectId)
		{
			return Ok(new WebStatus(PageStatus.Functional));
		}

#endregion
	}
}