using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;

namespace ApiGraphQL.Repository.Interfaces
{
	public interface ITransactionRepository : IBaseRepository<Transaction>
	{
	}
}
