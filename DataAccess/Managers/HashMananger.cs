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
	public class HashMananger : IManager<SomeHash>
	{
		ZoliksEntities _ent { get; set; }
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="HashMananger"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public HashMananger(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects Hash by ID
		/// </summary>
		/// <param name="id">Hash ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<SomeHash> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<SomeHash>(StatusCode.NotValidID);
			}
			SomeHash hash = _ent.SomeHashes.AsNoTracking().SingleOrDefault(x => x.ID == id);
			if (hash == null) {
				return new MActionResult<SomeHash>(StatusCode.NotFound);
			}
			return new MActionResult<SomeHash>(StatusCode.OK, hash);
		}

		/// <summary>
		/// Creates new Hash.
		/// </summary>
		/// <param name="hash">The hash.</param>
		/// <param name="owner">The owner ID.</param>
		/// <exception cref="StatusCode.InvalidInput" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<SomeHash> Create(string hash, int ownerId)
		{
			if (string.IsNullOrWhiteSpace(hash) || (ownerId < 1)) {
				return new MActionResult<SomeHash>(StatusCode.InvalidInput);
			}
			SomeHash h = new SomeHash()
			{
				OwnerID = ownerId,
				Hash = hash,
				Version = new Version(1, 0, 1).ToString(),
				Created = DateTime.Now,
				Expiration = null
			};
			SomeHash h1 = _ent.SomeHashes.Add(h);
			Save(null);
			h1 = _ent.SomeHashes.AsNoTracking().SingleOrDefault(x => x.ID == h1.ID);
			return new MActionResult<SomeHash>(StatusCode.OK, h1);
		}

		/// <summary>
		/// Saves Transaction
		/// </summary>
		/// <param name="hash">The Hash to save</param>
		/// <param name="throwException">if set to <c>true</c> throw exception</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(SomeHash hash, bool throwException = true)
		{
			try {
				if (hash != null) {
					_ent.SomeHashes.AddOrUpdate(hash);
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
