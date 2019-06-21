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
using Microsoft.Web.Http;
using SharedLibrary;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[OwnAuthorize]
	[RoutePrefix("class")]
	public class ClassController : OwnApiController<ClassManager>
	{
		// GET: class/get/5
		[HttpGet]
		[Route("get/{id}")]
		[ResponseType(typeof(MActionResult<Class>))]
		public async Task<IHttpActionResult> Get([FromUri] int id)
		{
			if (id < 1) {
				return Ok(new MActionResult<Class>(SharedLibrary.Enums.StatusCode.NotValidID));
			}
			var res = await Mgr.GetByIdAsync(id);
			return Ok(res);
		}

		// GET: class/getall
		[HttpGet]
		[Route("getall")]
		[ResponseType(typeof(List<Class>))]
		public async Task<IHttpActionResult> GetAll()
		{
			var res = await Mgr.GetAllAsync();
			return Ok(res);
		}
	}
}