using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiGraphQL.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class PublicController : ControllerBase
	{
		// GET /public/connection
		[HttpGet]
		[Route("connection")]
		public ActionResult<bool> Connection()
		{
			return Ok(true);
		}
	}
}
