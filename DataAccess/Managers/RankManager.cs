using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using DataAccess.Errors;
using DataAccess.Models;
using JetBrains.Annotations;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class RankManager
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		public RankManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}


		[CanBeNull]
		public Rank Select(int xp)
		{
			if (xp < 0) {
				return null;
			}

			Rank r = _ent.Ranks.SingleOrDefault(x => x.FromXP <= xp && xp <= (x.ToXP ?? int.MaxValue));

			return r;
		}

		public int Save(Rank content, bool throwException = true)
		{
			try {
				if (content != null) {
					_ent.Ranks.AddOrUpdate(content);
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