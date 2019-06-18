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
	public class ClassManager : IManager<Class>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		public ClassManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		public MActionResult<Class> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Class>(StatusCode.NotValidID);
			}
			Class c = _ent.Classes.Find(id);
			if (c == null) {
				return new MActionResult<Class>(StatusCode.NotFound);
			}

			if (!c.Enabled) {
				return new MActionResult<Class>(StatusCode.NotEnabled, c);
			}
			return new MActionResult<Class>(StatusCode.OK, c);
		}

		public MActionResult<List<Class>> GetAll()
		{
			return new MActionResult<List<Class>>(StatusCode.OK, _ent.Classes.Where(x => x.Enabled).OrderBy(x => x.Name).ToList());
		}

		public int Save(Class c, bool throwException = false)
		{
			try {
				if (c != null) {
					_ent.Classes.AddOrUpdate(c);
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
