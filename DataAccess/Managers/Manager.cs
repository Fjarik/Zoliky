using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using Z.EntityFramework.Plus;

namespace DataAccess.Managers
{
	public class Manager<T> : BaseManager<T>, IDisposable, New.Interfaces.IManager<T> where T : class, IDbEntity
	{
#region Constructors

		protected Manager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

#endregion

#region Methods

		public virtual Task<bool> IdExistsAsync(int id)
		{
			return _ctx.Set<T>().AnyAsync(x => x.ID == id);
		}

		public virtual Task<int> GetPreviousIdAsync(int currentId)
		{
			return _ctx.Set<T>()
					   .Where(x => x.ID < currentId)
					   .OrderByDescending(x => x.ID)
					   .Select(x => x.ID)
					   .FirstOrDefaultAsync();
		}

		public virtual Task<int> GetNextIdAsync(int currentId)
		{
			return _ctx.Set<T>()
					   .Where(x => x.ID > currentId)
					   .OrderBy(x => x.ID)
					   .Select(x => x.ID)
					   .FirstOrDefaultAsync();
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