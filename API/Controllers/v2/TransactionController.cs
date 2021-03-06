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
using SharedLibrary.Shared;

namespace API.Controllers.v2
{
	[ApiVersion("2.0")]
	[OwnAuthorize]
	[RoutePrefix("transaction")]
	public class TransactionController : OwnApiController<TransactionManager>
	{
		// GET: transaction/get/5
		[HttpGet]
		[Route("get/{id}")]
		[ResponseType(typeof(MActionResult<Transaction>))]
		public async Task<IHttpActionResult> Get([FromUri] int id)
		{
			if (id < 1) {
				return Ok(new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.NotValidID));
			}
			var res = await Mgr.GetByIdAsync(id);
			return Ok(res);
		}

		// GET: transaction/getbyuser?lastTransId=50
		[HttpGet]
		[Route("getbyuser")]
		[ResponseType(typeof(MActionResult<List<Transaction>>))]
		public async Task<IHttpActionResult> GetUserTransactions([FromUri] int? lastTransId = null,
																 [FromUri] bool isTester = false,
																 [FromUri] int take = 100)
		{
			var userId = this.User.Identity.GetId();
			if (userId < 1) {
				return Ok(new MActionResult<List<Transaction>>(SharedLibrary.Enums.StatusCode.NotValidID));
			}
			MActionResult<List<Transaction>> mAz;
			try {
				mAz = await Mgr.UserTransactionsAsync(userId, isTester, lastTransId, take);
			} catch (Exception ex) {
				mAz = new MActionResult<List<Transaction>>(SharedLibrary.Enums.StatusCode.SeeException, ex);
			}

			return Ok(mAz);
		}

		// GET: transaction/getbyzolik?zolikId=10
		[HttpGet]
		[Route("getbyzolik")]
		[Authorize(Roles = UserRoles.AdminOrDeveloperOrTeacher)]
		[ResponseType(typeof(List<Transaction>))]
		public async Task<IHttpActionResult> GetZolikTransactions([FromUri] int zolikId)
		{
			if (zolikId < 1) {
				return Ok(new List<Transaction>());
			}
			try {
				var res = await Mgr.ZolikTransactionsAsync(zolikId);
				return Ok(res);
			} catch (Exception ex) {
				return Ok(new List<Transaction>());
			}
		}
	}
}