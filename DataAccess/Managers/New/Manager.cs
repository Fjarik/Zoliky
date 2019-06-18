﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Errors;
using DataAccess.Managers.New.Interfaces;
using DataAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace DataAccess.Managers.New
{
	public class Manager<T> : BaseManager<T>, IDisposable, Interfaces.IManager<T> where T : class, IDbEntity
	{
#region Constructors

		protected Manager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

#endregion

#region Methods

		public virtual async Task<bool> IdExistsAsync(int id)
		{
			return await _ctx.Set<T>().AnyAsync(x => x.ID == id);
		}

		public virtual async Task<MActionResult<T>> GetByIdAsync(int id)
		{
			return new MActionResult<T>(StatusCode.OK, await _ctx.Set<T>()
																 .AsNoTracking()
																 .FirstOrDefaultAsync(x => x.ID == id));
		}

		public virtual async Task<MActionResult<T>> CreateAsync(T entity)
		{
			var ent = _ctx.Set<T>().Add(entity);
			var changes = await base.SaveAsync();
			if (changes == 0) {
				return new MActionResult<T>(StatusCode.InternalError);
			}
			var res = await this.GetByIdAsync(ent.ID);
			if (res.IsSuccess) {
				ent = res.Content;
			}
			return new MActionResult<T>(StatusCode.OK, ent);
		}

		public virtual async Task<bool> DeleteAsync(int id)
		{
			var result = await this.GetByIdAsync(id);
			if (!result.IsSuccess) {
				return false;
			}
			var entity = result.Content;
			return await base.DeleteAsync(entity);
		}

#endregion
	}
}