using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;

namespace ApiGraphQL.Repository
{
	public class ZolikRepository : IZolikRepository
	{
		private ZoliksEntities _context;

		public ZolikRepository(ZoliksEntities ent)
		{
			_context = ent;
		}

		public IEnumerable<Zolik> GetAll()
		{
			return _context.Zoliky
						   .Where(x => x.Enabled)
						   .ToList();
		}
	}
}