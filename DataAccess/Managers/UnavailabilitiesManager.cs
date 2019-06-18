using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Errors;
using DataAccess.Models;
using JetBrains.Annotations;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class UnavailabilitiesManager : IManager<DataAccess.Models.Unavailability>
	{

		private ZoliksEntities _ent;
		private Manager _mgr;

		public UnavailabilitiesManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}


		public MActionResult<DataAccess.Models.Unavailability> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<DataAccess.Models.Unavailability>(StatusCode.NotValidID);
			}
			DataAccess.Models.Unavailability u = _ent.Unavailabilities.Find(id);
			if (u == null) {
				return new MActionResult<DataAccess.Models.Unavailability>(StatusCode.NotFound);
			}

			if (u.To < DateTime.Now) {
				return new MActionResult<DataAccess.Models.Unavailability>(StatusCode.NotEnabled, u);
			}
			return new MActionResult<DataAccess.Models.Unavailability>(StatusCode.OK, u);
		}

		[NotNull]
		public MActionResult<List<DataAccess.Models.Unavailability>> GetAll(Projects? project, bool onlyActive = false)
		{
			IQueryable<DataAccess.Models.Unavailability> res = _ent.Unavailabilities;
			if (onlyActive) {
				res = res.Where(x => x.From < DateTime.Now && x.To > DateTime.Now);
			}

			return new MActionResult<List<DataAccess.Models.Unavailability>>(StatusCode.OK, ((project == null) ? res : res.Where(x => x.ProjectID == (int)project)).ToList());
		}

		[NotNull]
		public MActionResult<List<DataAccess.Models.Unavailability>> GetAll(bool onlyActive = false)
		{
			return GetAll(null, onlyActive);
		}

		public bool IsAvailable(Projects project)
		{
			return !_ent.Unavailabilities.Any(x => x.ProjectID == (int)project && x.From < DateTime.Now && x.To > DateTime.Now);
		}

		public int Save(DataAccess.Models.Unavailability content, bool throwException = true)
		{
			try {
				if (content != null) {
					_ent.Unavailabilities.AddOrUpdate(content);
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
