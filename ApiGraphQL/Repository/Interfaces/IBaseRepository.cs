using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary.Interfaces;

namespace ApiGraphQL.Repository.Interfaces
{
	public interface IBaseRepository<T> where T : IDbEntity
	{
		IEnumerable<T> GetAll();
		T GetById(int id);
	}
}