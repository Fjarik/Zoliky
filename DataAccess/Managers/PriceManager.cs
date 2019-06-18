using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using DataAccess.Errors;
using DataAccess.Models;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class PriceManager : IManager<Price>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="PriceManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public PriceManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects Price change by ID.
		/// </summary>
		/// <param name="id">Price ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Price> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Price>(StatusCode.NotValidID);
			}

			Price p = _ent.Prices.Find(id);
			if (p == null) {
				return new MActionResult<Price>(StatusCode.NotFound);
			}
			return new MActionResult<Price>(StatusCode.OK, p);
		}

		/// <summary>
		/// Gets all price changes.
		/// </summary>
		/// <exception cref="StatusCode.OK" />
		public MActionResult<List<Price>> GetAll()
		{
			List<Price> p = _ent.Prices.OrderBy(x => x.Date).ToList();
			return new MActionResult<List<Price>>(StatusCode.OK, p);
		}

		/// <summary>
		/// Creates new Price change
		/// </summary>
		/// <param name="currentValue">The current value.</param>
		/// <param name="type">The Zolik type.</param>
		/// <exception cref="StatusCode.InvalidInput" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Price> Create(double currentValue, ZolikType type)
		{
			if (currentValue < 1) {
				return new MActionResult<Price>(StatusCode.InvalidInput);
			}
			Price p = new Price() {
				Date = DateTime.Now,
				Value = currentValue,
				ZolikType = type
			};
			Price p1 = _ent.Prices.Add(p);
			Save(null);
			return new MActionResult<Price>(StatusCode.OK, p1);
		}

		/// <summary>
		/// Saves Price
		/// </summary>
		/// <param name="p">The Price to save</param>
		/// <param name="throwException">if set to <c>true</c> throw exception</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(Price p, bool throwException = true)
		{
			try {
				if (p != null) {
					_ent.Prices.AddOrUpdate(p);
				}
				int changes = _ent.SaveChanges();
				return changes;
			} catch (DbEntityValidationException ex) {
				if (throwException) {
					throw new DbEntityValidationException(ex.GetExceptionMessage(), ex.EntityValidationErrors);
				}
				return 0;
			}
		}
	}
}