using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;

namespace ApiGraphQL.Repository.Interfaces
{
	public interface IZolikRepository : IBaseRepository<Zolik>
	{
		IEnumerable<Zolik> GetByOwnerId(int userId);
		Task<ILookup<int, Transaction>> GetTransactionsByZolikIdsAsync(IEnumerable<int> zolikIds);
	}
}