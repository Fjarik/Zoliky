using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.Repository.Interfaces;
using DataAccess.Models;
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

		public Zolik GetById(int id)
		{
			return _context.Zoliky
						   .Where(x => x.Enabled)
						   .SingleOrDefault(x => x.ID == id);
		}

		public IEnumerable<Zolik> GetByOwnerId(int userId)
		{
			return _context.Zoliky
						   .Where(x => x.Enabled)
						   .Where(x => x.OwnerID == userId)
						   .ToList();
		}

		public async Task<ILookup<int, Transaction>> GetTransactionsByZolikIdsAsync(IEnumerable<int> zolikIds)
		{
			var trans = await _context.Transactions
									  .Include(x => x.Zolik)
									  .Where(x => zolikIds.Contains(x.ZolikID))
									  .ToListAsync();
			return trans.ToLookup(x => x.ZolikID);
		}
	}
}