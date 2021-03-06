﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using API.Tools;
using API.Tools.Annotations;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.Web.Http;
using SharedLibrary;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[OwnAuthorize]
	[RoutePrefix("subject")]
	public class SubjectController : OwnApiController<SubjectManager>
	{
		// GET: subject/get/5
		[HttpGet]
		[Route("get/{id}")]
		[ResponseType(typeof(MActionResult<Subject>))]
		public async Task<IHttpActionResult> Get([FromUri] int id)
		{
			if (id < 1) {
				return Ok(new MActionResult<Subject>(SharedLibrary.Enums.StatusCode.NotValidID));
			}
			var res = await Mgr.GetByIdAsync(id);
			if (res.Content == null) {
				return Ok(new MActionResult<Subject>(SharedLibrary.Enums.StatusCode.NotFound));
			}
			return Ok(res);
		}

		// GET: subject/getAll
		[HttpGet]
		[Route("getall")]
		[ResponseType(typeof(List<Subject>))]
		public async Task<IHttpActionResult> GetAll()
		{
			var schoolId = this.User.Identity.GetSchoolId();

			var sMgr = this.GetManager<SchoolManager>();
			var res = await sMgr.GetSubjectsAsync(schoolId);

			return Ok(res);
		}

		// GET: subject/getByTeacher
		[HttpGet]
		[Route("getbyteacher")]
		[ResponseType(typeof(List<TeacherSubject>))]
		public async Task<IHttpActionResult> GetByTeacher()
		{
			var teacherId = this.User.Identity.GetId();
			var tMgr = this.GetManager<TeacherSubjectManager>();

			var r = await tMgr.GetByTeacher(teacherId);

			var res = r.Select(x => new {
				x.TeacherID,
				x.ClassID,
				x.SubjectID,
			}).ToList();

			return Ok(res);
		}
	}
}