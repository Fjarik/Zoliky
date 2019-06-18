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
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.Web.Http;
using SharedLibrary;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[OwnAuthorize]
	[RoutePrefix("rank")]
	public class RankController : OwnApiController<RankManager>
	{
		[HttpGet]
		[Route("select")]
		[ResponseType(typeof(MActionResult<Rank>))]
		public async Task<IHttpActionResult> Select(int xp)
		{
			var res = await Mgr.SelectAsync(xp);
			return Ok(res);
		}

		[HttpGet]
		[Route("getall")]
		[ResponseType(typeof(List<Rank>))]
		public async Task<IHttpActionResult> GetAll()
		{
			var res = await Mgr.GetAllAsync();
			return Ok(res);
		}
	}
}