using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;
using SharedLibrary.Interfaces;

namespace DataAccess.Managers.New.Interfaces
{
	public interface IManager : IDisposable { }

	public interface IManager<T> : IManager, IBaseManager<T> where T : class, IDbEntity
	{
		Task<bool> IdExistsAsync(int id);
		Task<MActionResult<T>> GetByIdAsync(int id);
		Task<MActionResult<T>> CreateAsync(T entity);
		Task<bool> DeleteAsync(int id);
	}
}