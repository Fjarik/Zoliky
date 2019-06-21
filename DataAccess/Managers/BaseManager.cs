using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using DataAccess.Errors;
using DataAccess.Managers.New.Interfaces;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace DataAccess.Managers
{
	public abstract class BaseManager<T> : IBaseManager<T> where T : class, IDbObject
	{
#region Fields

		protected readonly ZoliksEntities _ctx;
		protected readonly IOwinContext Context;

#endregion

#region Constructors

		protected BaseManager(IOwinContext context) : this(context, context.Get<ZoliksEntities>()) { }

		protected BaseManager(IOwinContext context, ZoliksEntities ctx)
		{
			this._ctx = ctx;
			this.Context = context;
		}

#endregion

#region Methods

		public virtual async Task<bool> DeleteAsync(T entity)
		{
			try {
				_ctx.Set<T>().Attach(entity);
				_ctx.Set<T>().Remove(entity);
				await this.SaveAsync();
				return true;
			} catch {
				return false;
			}
		}

		public virtual async Task<MActionResult<T>> UpdateAsync(T entity)
		{
			_ctx.Entry(entity).State = EntityState.Modified;
			await this.SaveAsync(entity);
			return new MActionResult<T>(StatusCode.OK, entity);
		}

		public virtual Task<List<T>> GetAllAsync()
		{
			return _ctx.Set<T>().ToListAsync();
		}

		public virtual async Task<int> SaveAsync(T entity, bool throwException = true)
		{
			if (entity != null) {
				/*
				if (_ctx.Entry(entity).State != EntityState.Unchanged) {
					_ctx.Entry(entity).State = EntityState.Modified;
				}*/
				_ctx.Set<T>().AddOrUpdate(entity);
			}
			return await this.SaveAsync(throwException);
		}

		public virtual async Task<int> SaveAsync(bool throwException = true)
		{
			try {
				return await _ctx.SaveChangesAsync();
			} catch (DbEntityValidationException ex) {
				if (throwException) {
					throw new DbEntityValidationException(ex.GetExceptionMessage(), ex.EntityValidationErrors);
				}
				return 0;
			}
		}

#endregion

#region Dispose

		public void Dispose()
		{
			_ctx?.Dispose();
		}

#endregion
	}
}