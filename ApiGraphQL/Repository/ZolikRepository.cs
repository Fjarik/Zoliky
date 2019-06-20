using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
//using DataAccessCore.Models;
using SharedLibrary.Shared;

namespace ApiGraphQL.Repository
{
	public class ZolikRepository : IZolikRepository
	{
		private ZoliksEntities _context;

		public ZolikRepository(ZoliksEntities ctx)
		{
			_context = ctx;
		}

		public IEnumerable<Zolik> GetAll()
		{
			return _context.Zoliky
						   .Where(x => x.Enabled)
						   .Where(DataAccess.Extensions.NonTesterZoliks())
						   .ToList();
		}
	}
}