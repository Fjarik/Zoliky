using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using API.Tools;
using API.Tools.Annotations;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Web.Http;
using SharedLibrary;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[OwnAuthorize]
	[RoutePrefix("image")]
	public class ImageController : OwnApiController<ImageManager>
	{
		// GET: image/get/5
		[HttpGet]
		[Route("get/{id}")]
		[ResponseType(typeof(MActionResult<Image>))]
		public async Task<IHttpActionResult> Get([FromUri] int id)
		{
			if (id < 1) {
				return Ok(new MActionResult<Image>(SharedLibrary.Enums.StatusCode.NotValidID));
			}
			var res = await Mgr.GetByIdAsync(id);
			return Ok(res);
		}

		// GET: image/getsize/5
		[HttpGet]
		[Route("getsize/{id}")]
		[ResponseType(typeof(int))]
		public async Task<IHttpActionResult> GetSize([FromUri] int id)
		{
			if (id < 1) {
				return Ok(0);
			}
			try {
				var size = await Mgr.GetSizeAsync(id);
				return Ok(size);
			} catch (Exception ex) {
				return Ok(new MActionResult<Image>(SharedLibrary.Enums.StatusCode.SeeException, ex));
			}
		}
	}
}