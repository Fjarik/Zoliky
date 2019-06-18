using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using DataAccess.Errors;
using DataAccess.Models;
using JetBrains.Annotations;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

namespace DataAccess.Managers
{
	public class ZolikManager : IManager<Zolik>
	{
		private ZoliksEntities _ent;
		private Manager _mgr;

		public ZolikManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		public MActionResult<Zolik> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Zolik>(StatusCode.NotValidID);
			}
			Zolik z = _ent.Zoliky.Find(id);
			if (z == null) {
				return new MActionResult<Zolik>(StatusCode.NotFound);
			}

			if (!z.Enabled) {
				return new MActionResult<Zolik>(StatusCode.NotEnabled, z);
			}
			return new MActionResult<Zolik>(StatusCode.OK, z);
		}

		[NotNull]
		public MActionResult<List<Zolik>> UsersZoliky(int userId, bool isTester = false)
		{
			if (userId < 1) {
				return new MActionResult<List<Zolik>>(StatusCode.NotValidID);
			}
			List<Zolik> z = new List<Zolik>();
			if (isTester) {
				z = _ent.Zoliky.Where(x => x.OwnerID == userId && x.Enabled).ToList();
			} else {
				z = _ent.Zoliky.Where(x => x.OwnerID == userId &&
										   x.Enabled &&
										   x.Type != ZolikType.Debug &&
										   x.Type != ZolikType.DebugJoker).ToList();
			}

			z = z.OrderByDescending(x => x.Created).ToList();

			return new MActionResult<List<Zolik>>(StatusCode.OK, z);
		}

		[NotNull]
		public MActionResult<Zolik> Create(int teacherId, int ownerId, int originalOwnerId, ZolikType type,
										   int subjectId, string title, bool allowSplit = true)
		{
			if (ownerId < 1 || originalOwnerId < 1 || string.IsNullOrWhiteSpace(title)) {
				return new MActionResult<Zolik>(StatusCode.InvalidInput);
			}

			if (!type.IsSplittable()) {
				allowSplit = false;
			}

			Zolik z = new Zolik() {
				TeacherID = teacherId,
				OwnerID = ownerId,
				SubjectID = subjectId,
				OriginalOwnerID = originalOwnerId,
				Type = type,
				Title = title,
				OwnerSince = DateTime.Now,
				Created = DateTime.Now,
				AllowSplit = allowSplit,
				Enabled = true,
				Lock = null
			};
			Zolik z1 = _ent.Zoliky.Add(z);
			Save(null);
			return new MActionResult<Zolik>(StatusCode.OK, z1);
		}

		// TODO: To Private
		public bool Transfer(ZolikPackage p)
		{
			try {
				User from = _mgr.Users.Select(p.FromID).Content;
				User to = _mgr.Users.Select(p.ToID).Content;
				Zolik z = _mgr.Zoliky.Select(p.ZolikID).Content;
				if (p.Type != TransactionAssignment.ZerziRemoval && p.Type != TransactionAssignment.NewAssignment &&
					!z.CanBeTransfered) {
					return false;
				}
				z.OwnerID = to.ID;
				z.OwnerSince = DateTime.Now;
				if (p.Type == TransactionAssignment.ZerziRemoval) {
					z.Enabled = false;
				}
				_mgr.Zoliky.Save(z);
				_mgr.Transactions.Create(from.ID, to.ID, z.ID, p.Message, p.Type);
				return true;
			} catch {
				return false;
			}
		}

		public MActionResult<Transaction> Transfer(ZolikPackage p, User logged)
		{
			if (p == null || logged == null || !p.IsValid) {
				return new MActionResult<Transaction>(StatusCode.NotValidID);
			}

			int fromId = p.FromID;
			int toId = p.ToID;

			if (logged.Type != UserPermission.Teacher && logged.Type != UserPermission.Dev && logged.ID != fromId) {
				return new MActionResult<Transaction>(StatusCode.InsufficientPermissions);
			}

			try {
				if (!_mgr.Users.Exists(fromId) || !_mgr.Users.Exists(toId)) {
					return new MActionResult<Transaction>(StatusCode.NotValidID);
				}

				var resZolik = Select(p.ZolikID);
				if (!resZolik.IsSuccess) {
					return new MActionResult<Transaction>(StatusCode.NotValidID);
				}

				Zolik z = resZolik.Content;
				if (z.OwnerID != p.FromID) {
					return new MActionResult<Transaction>(StatusCode.InsufficientPermissions);
				}
				if (p.Type != TransactionAssignment.ZerziRemoval && p.Type != TransactionAssignment.NewAssignment &&
					!z.CanBeTransfered) {
					return new MActionResult<Transaction>(StatusCode.CannotTransfer);
				}

				z.OwnerID = toId;
				z.OwnerSince = DateTime.Now;
				if (p.Type == TransactionAssignment.ZerziRemoval) {
					z.Enabled = false;
				}
				Save(z);
				var tranRes = _mgr.Transactions.Create(fromId, toId, z.ID, p.Message, p.Type);

				bool xp = _mgr.Users.AddXp(5, new[] {fromId, toId});

				if (!tranRes.IsSuccess || !xp) {
					return new MActionResult<Transaction>(StatusCode.JustALittleError);
				}
				var tran = tranRes.Content;

				var email = SendEmailNewZolik(toId, z, tran);
				if (!email) {
					return new MActionResult<Transaction>(StatusCode.JustALittleError);
				}
				return new MActionResult<Transaction>(StatusCode.OK, tran);
			} catch (Exception ex) {
#if DEBUG
				throw ex;
#else
				return new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.SeeException, ex);
#endif
			}
		}

		private bool SendEmailNewZolik(User to, Zolik z, Transaction t)
		{
			try {
				Mail m = new Mail();
				return m.NewZolik(to, z, t);
			} catch {
				return false;
			}
		}

		private bool SendEmailNewZolik(int userId, Zolik z, Transaction t)
		{
			var res = _mgr.Users.Select(userId);
			if (!res.IsSuccess && res.Status != StatusCode.NotEnabled) {
				return false;
			}
			return SendEmailNewZolik(res.Content, z, t);
		}

		public int Save(Zolik z, bool throwException = false)
		{
			try {
				if (z != null) {
					_ent.Zoliky.AddOrUpdate(z);
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