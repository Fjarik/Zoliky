using System;
using System.Collections.Generic;
using System.IO;
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
using SharedLibrary.Shared.ApiModels;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[RoutePrefix("user")]
	public class UserController : OwnApiController<UserManager>
	{
		// GET /user/get?id=1
		[HttpGet]
		[OwnAuthorize]
		[Route("get")]
		[ResponseType(typeof(MActionResult<User>))]
		public async Task<IHttpActionResult> Get([FromUri] int id,
												 [FromUri] bool includeImage = false)
		{
			var res = await Mgr.GetByIdAsync(id);
			if (res.IsSuccess && !includeImage) {
				res.Content.ProfilePhoto = null;
			}
			return Ok(res);
		}

		// GET /user/me
		[HttpGet]
		[OwnAuthorize]
		[Route("me")]
		[ResponseType(typeof(MActionResult<User>))]
		public async Task<IHttpActionResult> Me()
		{
			var userId = HttpContext.Current.GetOwinContext()?.Authentication?.User?.Identity?.GetId() ?? -1;
			if (userId < 1) {
				return Ok(new MActionResult<User>(SharedLibrary.Enums.StatusCode.NotValidID));
			}
			var res = await Mgr.GetByIdAsync(userId);
			return Ok(res);
		}

		// POST /user/login
		[HttpPost]
		[LoginOnly]
		[Route("login")]
		[ResponseType(typeof(MActionResult<User>))]
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

		// PUT /user/test?zolikId=1&fromId=1&toId=2
		[HttpPut]
		[OwnAuthorize]
		[Route("test")]
		[ResponseType(typeof(bool))]
		public async Task<IHttpActionResult> Test([FromUri] int zolikId, [FromUri] int fromId, [FromUri] int toId)
		{
			var mgr = HttpContext.Current.GetOwinContext().Get<FirebaseManager>();
			return Ok(await mgr.NewZolikAsync(zolikId, fromId, toId));
		}

		// GET /user/getname?id=1
		[HttpGet]
		[OwnAuthorize]
		[Route("getname")]
		[ResponseType(typeof(string))]
		public async Task<IHttpActionResult> GetName([FromUri] int id)
		{
			return Ok(await Mgr.GetUserFullnameAsync(id));
		}

		// GET /user/getusers?classId=1
		[HttpGet]
		[OwnAuthorize]
		[Route("getusers")]
		[ResponseType(typeof(MActionResult<List<User>>))]
		public async Task<IHttpActionResult> GetUsers([FromUri] int? classId = null,
													  [FromUri] bool includeImage = false)
		{
			var res = await Mgr.GetStudentUsersAsync(classId, includeImage);
			return Ok(res);
		}

		// POST /user/mobiletoken
		[HttpPost]
		[OwnAuthorize]
		[Route("mobiletoken")]
		[ResponseType(typeof(bool))]
		public async Task<IHttpActionResult> MobileToken(TokenChange tokenChange)
		{
			if (tokenChange == null || string.IsNullOrWhiteSpace(tokenChange.Token)) {
				return Ok(false);
			}
			var guid = ClaimsPrincipal.Current.GetLoggedUserGuid();
			if (guid == Guid.Empty) {
				return Ok(false);
			}
			var res = await Mgr.SetMobileTokenAsync(guid, tokenChange.Token);
			return Ok(res);
		}

		// POST /user/resettoken
		[HttpPost]
		[OwnAuthorize]
		[Route("resettoken")]
		[ResponseType(typeof(bool))]
		public async Task<IHttpActionResult> ResetToken()
		{
			var guid = ClaimsPrincipal.Current.GetLoggedUserGuid();
			if (guid == Guid.Empty) {
				return Ok(false);
			}
			var res = await Mgr.ResetMobileTokenAsync(guid);
			return Ok(res);
		}

		// POST /user/changeProfilePhoto
		[HttpPost]
		[OwnAuthorize]
		[Route("changeprofilephoto")]
		[ResponseType(typeof(bool))]
		public async Task<IHttpActionResult> ChangeProfilePhoto()
		{
			if (!this.Request.Content.IsMimeMultipartContent()) {
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}
			var request = HttpContext.Current.Request;

#region File Check

			if (request.Files.Count != 1) {
				throw new HttpResponseException(HttpStatusCode.NoContent);
			}

			var file = request.Files[0];
			if (file == null || file.ContentLength < 1) {
				throw new HttpResponseException(HttpStatusCode.NoContent);
			}

			if (file.ContentLength > 4096 * 1024) {
				throw new HttpResponseException(HttpStatusCode.RequestEntityTooLarge);
			}

			if (!file.IsImage()) {
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}

#endregion

			var mime = file.ContentType;

			byte[] bytes;
			using (BinaryReader br = new BinaryReader(file.InputStream)) {
				bytes = br.ReadBytes((int) file.InputStream.Length);
			}

			var userId = this.User.Identity.GetId();

			var imgMgr = this.GetManager<ImageManager>();
			var res = await imgMgr.CreateAsync(userId, bytes, mime);
			if (res.IsSuccess) {
				var img = res.Content;
				await Mgr.ChangeProfilePhotoAsync(userId, img.ID);
			} else {
				return Ok(false);
			}
			return Ok(true);
		}

		// GET /user/logins?take=1
		[HttpGet]
		[OwnAuthorize]
		[Route("logins")]
		[ResponseType(typeof(MActionResult<List<UserLogin>>))]
		public async Task<IHttpActionResult> GetUserLogins([FromUri] int take = 100)
		{
			var userId = this.User.Identity.GetId();
			if (userId < 1) {
				return Ok(new MActionResult<List<UserLogin>>(SharedLibrary.Enums.StatusCode.NotValidID));
			}

			var uMgr = this.GetManager<UserLoginManager>();
			try {
				var res = await uMgr.GetAllAsync(userId, take);
				return Ok(res);
			} catch (Exception ex) {
				return Ok(new MActionResult<List<UserLogin>>(SharedLibrary.Enums.StatusCode.SeeException, ex));
			}
		}
	}
}