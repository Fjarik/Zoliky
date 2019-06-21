using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;
using SharedLibrary.Interfaces;

namespace DataAccess.Managers.New.Interfaces
{
	public interface IBaseManager<T> where T : class, IDbObject
	{
		Task<MActionResult<T>> UpdateAsync(T entity);
		Task<bool> DeleteAsync(T entity);

		Task<List<T>> GetAllAsync();

		Task<int> SaveAsync(T entity, bool throwException = false);
		Task<int> SaveAsync(bool throwException = false);
	}
}