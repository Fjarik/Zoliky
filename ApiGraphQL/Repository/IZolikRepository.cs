using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessCore.Models;

namespace ApiGraphQL.Repository
{
	public interface IZolikRepository
	{
		IEnumerable<Zolik> GetAll();
	}
}
