using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
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
using SharedLibrary.Enums;
using SharedLibrary.Shared;
using SharedLibrary.Shared.ApiModels;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[OwnAuthorize]
	[RoutePrefix("zolik")]
	public class ZolikController : OwnApiController<ZolikManager>
	{
		// GET: /zolik/get/5
		[HttpGet]
		[Route("get/{id}")]
		[ResponseType(typeof(MActionResult<Zolik>))]
		public async Task<IHttpActionResult> GetZolik([FromUri] int id)
		{
			if (id < 1) {
				return Ok(new MActionResult<Zolik>(SharedLibrary.Enums.StatusCode.NotValidID));
			}
			try {
				return Ok(await Mgr.GetByIdAsync(id));
			} catch (Exception ex) {
				return Ok(new MActionResult<Zolik>(SharedLibrary.Enums.StatusCode.SeeException, ex));
			}
		}

		// GET /zolik/getuserzoliks?userId=10
		[HttpGet]
		[Route("getuserzoliks")]
		[ResponseType(typeof(MActionResult<List<Zolik>>))]
		public async Task<IHttpActionResult> GetUserZoliks([FromUri] int userId,
														   [FromUri] bool isTester = false,
														   [FromUri] bool onlyEnabled = true)
		{
			if (userId < 1) {
				return Ok(new MActionResult<List<Zolik>>(SharedLibrary.Enums.StatusCode.NotValidID));
			}
			try {
				return Ok(await Mgr.GetUsersZoliksAsync(userId, isTester, onlyEnabled));
			} catch (Exception ex) {
				return Ok(new MActionResult<List<Zolik>>(SharedLibrary.Enums.StatusCode.SeeException, ex));
			}
		}

		// GET /zolik/getuserzolikcount?userId=10
		[HttpGet]
		[Route("getuserzolikcount")]
		[ResponseType(typeof(int))]
		public async Task<IHttpActionResult> GetUserZolikCount([FromUri] int userId,
															   [FromUri] bool isTester = false,
															   [FromUri] bool onlyEnabled = true)
		{
			if (userId < 1) {
				return Ok(0);
			}
			try {
				var count = await Mgr.GetUsersZolikCountAsync(userId, isTester, onlyEnabled);
				return Ok(count);
			} catch {
				return Ok(0);
			}
		}

		// GET /zolik/getuserzoliklist?userId=10
		[HttpGet]
		[Route("getuserzoliklist")]
		[ResponseType(typeof(List<Zolik>))]
		public async Task<IHttpActionResult> GetUserZolikList([FromUri] int userId,
															  [FromUri] bool isTester = false,
															  [FromUri] bool onlyEnabled = true)
		{
			try {
				var res = await Mgr.GetUsersZolikListAsync(userId, isTester, onlyEnabled);
				return Ok(res);
			} catch (Exception ex) {
				return Ok(new List<Zolik>());
			}
		}

#region Zolik types

		// GET /zolik/gettypes
		[HttpGet]
		[Route("gettypes")]
		[ResponseType(typeof(List<ZolikType>))]
		public async Task<IHttpActionResult> GetTypes()
		{
			try {
				var sMgr = this.GetManager<SchoolManager>();
				var schoolId = this.User.Identity.GetSchoolId();

				var res = await sMgr.GetSchoolZolikTypesAsync(schoolId, false);
				return Ok(res);
			} catch (Exception ex) {
				return Ok(new List<ZolikType>());
			}
		}

#endregion

#region Transfer & Split

		// POST /zolik/transfer
		[HttpPost]
		[Route("transfer")]
		[ResponseType(typeof(MActionResult<Transaction>))]
		public async Task<IHttpActionResult> TransferZolik(ZolikPackage package)
		{
			if (package == null || !package.IsValid) {
				return Ok(new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.NotValidID));
			}

			var user = await ClaimsPrincipal.Current.GetLoggedUserAsync();
			if (user == null ||
				!user.IsInRolesOr(UserRoles.Teacher, UserRoles.Administrator) &&
				user.ID != package.FromID) {
				return Ok(new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.InsufficientPermissions));
			}
			var res = await Mgr.TransferAsync(package, user);
			return Ok(res);
		}

		// POST /zolik/split
		[HttpPost]
		[Route("split")]
		[ResponseType(typeof(MActionResult<List<Transaction>>))]
		public async Task<IHttpActionResult> Split(ZolikSplit split)
		{
			if (split == null || !split.IsValid) {
				return Ok(new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.NotValidID));
			}

			var user = await ClaimsPrincipal.Current.GetLoggedUserAsync();
			if (user == null) {
				return Ok(new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.InsufficientPermissions));
			}
			var zolikId = split.ZolikId;
			var res = await Mgr.SplitAsync(zolikId, user);
			return Ok(res);
		}

#endregion

#region (Un)Lock

		// POST /zolik/lock
		[HttpPost]
		[Route("lock")]
		[ResponseType(typeof(MActionResult<Zolik>))]
		public async Task<IHttpActionResult> Lock(ZolikLock zLock)
		{
			if (zLock == null || !zLock.IsValid) {
				return Ok(new MActionResult<Zolik>(SharedLibrary.Enums.StatusCode.InvalidInput));
			}

			var logged = await ClaimsPrincipal.Current.GetLoggedUserAsync();
			if (logged == null) {
				return Ok(new MActionResult<Zolik>(SharedLibrary.Enums.StatusCode.InsufficientPermissions));
			}
			var res = await Mgr.LockAsync(zLock, logged);
			return Ok(res);
		}

		// POST /zolik/unlock
		[HttpPost]
		[Route("unlock")]
		[ResponseType(typeof(MActionResult<Zolik>))]
		public async Task<IHttpActionResult> Unlock(ZolikLock zLock)
		{
			if (zLock == null || zLock.ZolikId < 1) {
				return Ok(new MActionResult<Zolik>(SharedLibrary.Enums.StatusCode.NotValidID));
			}

			var logged = await ClaimsPrincipal.Current.GetLoggedUserAsync();
			if (logged == null) {
				return Ok(new MActionResult<Zolik>(SharedLibrary.Enums.StatusCode.InsufficientPermissions));
			}
			var res = await Mgr.UnlockAsync(zLock.ZolikId, logged);
			return Ok(res);
		}

#endregion

#region Admin

		// GET /zolik/getzolikownerids
		[HttpGet]
		[Authorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
		[Route("getzolikownerids")]
		[ResponseType(typeof(List<int>))]
		public async Task<IHttpActionResult> GetZolikOwnerIds()
		{
			try {
				var res = await Mgr.GetZolikOwnerIdsAsync();
				return Ok(res);
			} catch (Exception ex) {
				return Ok(new List<int>());
			}
		}

		// GET: zolik/getSchoolZoliks
		[HttpGet]
		[Route("getschoolzoliks")]
		[ResponseType(typeof(List<Zolik>))]
		[OwnAuthorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
		public async Task<IHttpActionResult> GetSchoolZoliks([FromUri] bool onlyEnabled = true)
		{
			var schoolId = this.User.Identity.GetSchoolId();

			var res = await Mgr.GetSchoolZoliksAsync(schoolId, onlyEnabled);
			return Ok(res);
		}

		// POST: zolik/delete
		[HttpPost]
		[Route("delete")]
		[ResponseType(typeof(MActionResult<Transaction>))]
		[OwnAuthorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
		public async Task<IHttpActionResult> DeleteZolik(ZolikRemove package)
		{
			if (package == null || !package.IsValid) {
				return Ok(
						  new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.InvalidInput));
			}
			var loggedId = this.User.Identity.GetId();
			try {
				var res = await Mgr.RemoveAsync(package.ZolikID, package.Reason, loggedId);
				return Ok(res);
			} catch (Exception ex) {
				return Ok(
						  new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.SeeException, ex));
			}
		}

		// POST: zolik/create
		[HttpPost]
		[Route("create")]
		[Route("ctransfer")]
		[ResponseType(typeof(MActionResult<Transaction>))]
		[OwnAuthorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
		public async Task<IHttpActionResult> CreateZolik(ZolikCPackage package)
		{
			if (package == null) {
				return Ok(new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.InvalidInput));
			}
			try {
				var logged = await ClaimsPrincipal.Current.GetLoggedUserAsync();
				if (logged == null) {
					return Ok(new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.InsufficientPermissions));
				}

				package.TeacherID = logged.ID;

				var res = await Mgr.CreateAndTransferAsync(package, logged);
				return Ok(res);
			} catch (Exception ex) {
				return Ok(new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.SeeException, ex));
			}
		}

#endregion
	}
}