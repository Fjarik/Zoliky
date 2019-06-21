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
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Web.Http;
using SharedLibrary.Shared.ApiModels;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[OwnAuthorize]
	[RoutePrefix("student")]
	public class StudentController : OwnApiController<UserManager>
	{
		// GET /student/getstudents?classId=1
		[HttpGet]
		[Route("getstudents")]
		[ResponseType(typeof(List<Student<Image>>))]
		public IHttpActionResult GetStudents([FromUri] int? classId = null,
											 [FromUri] int? imageMaxSize = null,
											 [FromUri] bool onlyActive = true)
		{
			var res = Mgr.GetStudents(classId: classId, imageMaxSize: imageMaxSize, onlyActive: onlyActive);
			return Ok(res);
		}

		// GET /student/gettop?classId=1
		[HttpGet]
		[Route("gettop")]
		[ResponseType(typeof(List<Student<Image>>))]
		public IHttpActionResult GetStudentsWithMostZoliks([FromUri] int? classId = null,
														   [FromUri] int top = 5,
														   [FromUri] int? imageMaxSize = null)
		{
			var res = Mgr.GetStudentsWithMostZoliks(top, classId, imageMaxSize);
			return Ok(res);
		}

		// GET /student/gettopxp?classId=1
		[HttpGet]
		[Route("gettopxp")]
		[ResponseType(typeof(List<Student<Image>>))]
		public IHttpActionResult GetStudentsWithMostXp([FromUri] int? classId = null,
													   [FromUri] int top = 5,
													   [FromUri] int? imageMaxSize = null)
		{
			var res = Mgr.GetStudentsWithMostXp(top, classId, imageMaxSize);
			return Ok(res);
		}

		// GET /student/getfake?classId=1
		[HttpGet]
		[Route("getfake")]
		[ResponseType(typeof(List<Student<Image>>))]
		public IHttpActionResult GetFakeStudents([FromUri] int? imageMaxSize = null,
												 [FromUri] bool onlyActive = true)
		{
			var res = Mgr.GetFakeStudents(imageMaxSize, onlyActive);
			return Ok(res);
		}
	}
}