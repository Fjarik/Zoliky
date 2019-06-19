using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessCore.Models;
using SharedLibrary.Shared;

namespace ApiGraphQL.Repository
{
	public class ZolikRepository : IZolikRepository
	{
		private ZolikyContext _context;

		public ZolikRepository(ZolikyContext ctx)
		{
			_context = ctx;
		}

		public IEnumerable<Zolik> GetAll()
		{
			return _context.Zoliky
						   .Where(x => x.Enabled && !x.Type.IsTesterType())
						   .ToList();
		}
	}
}