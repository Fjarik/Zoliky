using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using API.Tools;
using API.Tools.Annotations;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Web.Http;
using SharedLibrary;
using SharedLibrary.Enums;

namespace API.Controllers.v1
{
	[ApiVersion("1.0", Deprecated = true)]
	[OwnAuthorize]
	[RoutePrefix("user")]
	public class UserController : OwnApiController<UserManager>
	{
		// GET: /user/get/5
		[HttpGet]
		[ResponseType(typeof(MActionResult<User>))]
		[Route("get/{id}", Name = "GetUser")]
		public async Task<IHttpActionResult> GetUser(int id)
		{
			if (id < 1) {
				return Ok(new MActionResult<User>(SharedLibrary.Enums.StatusCode.NotValidID));
			}
			try {
				var mAu = await Mgr.GetByIdAsync(id);
				return Ok(mAu);
			} catch (Exception ex) {
				return Ok(new MActionResult<User>(SharedLibrary.Enums.StatusCode.SeeException, ex));
			}
		}

		// POST /user/login
		[HttpPost]
		[LoginOnly]
		[Route("login")]
		public async Task<IHttpActionResult> Login(Logins lg)
		{
			if (lg == null || string.IsNullOrWhiteSpace(lg.UName) || string.IsNullOrWhiteSpace(lg.Password)) {
				return Ok(new MActionResult<User>(SharedLibrary.Enums.StatusCode.InvalidInput));
			}
			string ip = this.Request.GetIPAddress();
			try {
				MActionResult<User> mAu = await Mgr.LoginAsync(lg, ip);
				return Ok(mAu);
			} catch (Exception ex) {
				return Ok(new MActionResult<User>(SharedLibrary.Enums.StatusCode.SeeException, ex));
			}
		}

		// POST /user/exists	
		[HttpPost]
		[Route("exists")]
		public async Task<IHttpActionResult> Exists(User u)
		{
			try {
				if (u == null || (string.IsNullOrWhiteSpace(u.Username) && string.IsNullOrWhiteSpace(u.Email))) {
					return BadRequest("Prázdný vstup");
				}
				bool exists = false;

				if (!string.IsNullOrWhiteSpace(u.Username)) {
					exists = await Mgr.ExistsAsync(u.Username, WhatToCheck.Username);
				}
				if (exists) {
					return Ok(true);
				}

				if (!string.IsNullOrWhiteSpace(u.Email)) {
					exists = await Mgr.ExistsAsync(u.Email, WhatToCheck.Email);
				}
				return Ok(exists);
			} catch (Exception ex) {
				return InternalServerError(ex);
			}
		}

		// GET /user/getstudents
		[HttpGet]
		[Route("getstudents")]
		public async Task<IHttpActionResult> GetStudents()
		{
			var res = await Mgr.GetStudentUsersAsync(); // TODO: Not working
			return Ok(res);
		}

		// GET /user/getFromClass/6
		[HttpGet]
		[Route("getfromclass/{classId}")]
		public async Task<IHttpActionResult> GetFromClass(int classId)
		{
			return Ok(await Mgr.GetStudentUsersAsync(classId));
		}

		// POST /user/getTester
		/*[HttpPost]
		[Route("getTester")]
		public IHttpActionResult GetTesterAccount([FromBody] User u)
		{
			if (u == null || string.IsNullOrWhiteSpace(u.Username)) {
				return Ok(new MActionResult<User>(SharedLibrary.Enums.StatusCode.InvalidInput));
			}

			if (!u.HasTesterRights) {
				return Ok(new MActionResult<User>(SharedLibrary.Enums.StatusCode.InsufficientPermissions));
			}

			if (u.IsTesterType) {
				return Ok(new MActionResult<User>(SharedLibrary.Enums.StatusCode.OK, u));
			}
			u.IsTesterType = true;
			return Ok(new MActionResult<User>(SharedLibrary.Enums.StatusCode.OK, u));
		}*/

		// POST /user/mobiletoken?token={token}
		[HttpPost]
		[Route("mobiletoken")]
		public async Task<IHttpActionResult> MobileToken(string token)
		{
			if (string.IsNullOrWhiteSpace(token)) {
				return BadRequest("Token is empty");
			}

			var guid = ClaimsPrincipal.Current.GetLoggedUserGuid();
			if (guid == Guid.Empty) {
				return BadRequest("You are not logged");
			}

			var res = await Mgr.SetMobileTokenAsync(guid, token);
			if (!res) {
				return BadRequest("Nezdařilo se");
			}
			return Ok();
		}
	}
}