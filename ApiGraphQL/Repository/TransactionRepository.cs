using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.Repository.Interfaces;
using DataAccess.Models;

namespace ApiGraphQL.Repository
{
	public class TransactionRepository : ITransactionRepository
	{
		private ZoliksEntities _context;

		public TransactionRepository(ZoliksEntities context)
		{
			_context = context;
		}

		public IEnumerable<Transaction> GetAll()
		{
			return _context.Transactions
						   .Include(x => x.Zolik)
						   .ToList();
		}

		public Transaction GetById(int id)
		{
			return _context.Transactions
						   .Include(x => x.Zolik)
						   .SingleOrDefault(x => x.ID == id);
		}
	}
}