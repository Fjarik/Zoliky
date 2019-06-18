using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using DataAccess.Errors;
using DataAccess.Models;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class AchievementsManager : IManager<Achievement>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		public AchievementsManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		public MActionResult<Achievement> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Achievement>(StatusCode.NotValidID);
			}

			Achievement a = _ent.Achievements.Find(id);
			if (a == null) {
				return new MActionResult<Achievement>(StatusCode.NotFound);
			}
			if (!a.Enabled) {
				return new MActionResult<Achievement>(StatusCode.NotEnabled, a);
			}
			return new MActionResult<Achievement>(StatusCode.OK, a);
		}

		public MActionResult<Achievement> Select(Achievements a)
		{
			return Select((int)a);
		}

		public int Save(Achievement a, bool throwException = true)
		{
			try {
				if (a != null) {
					_ent.Achievements.AddOrUpdate(a);
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