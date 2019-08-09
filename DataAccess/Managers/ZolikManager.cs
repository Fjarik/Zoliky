using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using SharedLibrary.Shared.ApiModels;

namespace DataAccess.Managers
{
	public class ZolikManager : Manager<Zolik>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public ZolikManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public ZolikManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

#region Overrides

		public override async Task<bool> DeleteAsync(Zolik entity)
		{
			entity.OwnerID = Ext.BankId;
			entity.Enabled = false;
			return await this.SaveAsync(entity) != 0;
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static ZolikManager Create(IdentityFactoryOptions<ZolikManager> options,
										  IOwinContext context)
		{
			return new ZolikManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<List<Zolik>>> GetUsersZoliksAsync(int userId,
																		  bool isTester = false,
																		  bool onlyEnabled = true)
		{
			if (userId < 1) {
				return new MActionResult<List<Zolik>>(StatusCode.NotValidID);
			}

			var z = await GetUsersZolikListAsync(userId, isTester, onlyEnabled);

			return new MActionResult<List<Zolik>>(StatusCode.OK, z);
		}

		public async Task<List<Zolik>> GetUsersZolikListAsync(int userId,
															  bool isTester = false,
															  bool onlyEnabled = true)
		{
			if (userId < 1) {
				return new List<Zolik>();
			}
			var query = _ctx.Zoliky.Where(x => x.OwnerID == userId);
			if (onlyEnabled) {
				query = query.Where(x => x.Enabled);
			}
			if (!isTester) {
				query = query.Where(x => x.Type != ZolikType.Debug && x.Type != ZolikType.DebugJoker);
			}
			query = query.OrderByDescending(x => x.OwnerSince);

			var z = await query.ToListAsync();

			return z;
		}

		public Task<List<Zolik>> GetSchoolZoliksAsync(int schoolId,
													  bool onlyEnabled = true,
													  bool includeTester = false)
		{
			var date = Ext.SchoolYearStart;
			var query = _ctx.Zoliky.Where(x => x.OriginalOwner.SchoolID == schoolId &&
											   x.OriginalOwner.ClassID != null &&
											   x.Teacher.SchoolID == schoolId &&
											   !x.Teacher.Roles.Any(y => y.Name == UserRoles.Robot ||
																		 y.Name == UserRoles.Administrator) &&
											   x.Created > date);
			if (onlyEnabled) {
				query = query.Where(x => x.Enabled);
			}
			if (!includeTester) {
				query = query.Where(x => x.Type != ZolikType.Debug &&
										 x.Type != ZolikType.DebugJoker);
			}
			query = query.OrderByDescending(x => x.OwnerSince);
			return query.ToListAsync();
		}

#region Create

		public Task<MActionResult<Zolik>> CreateAsync(int teacherId,
													  int originalOwnerId,
													  ZolikType type,
													  int subjectId,
													  string title,
													  bool allowSplit = true)
		{
			return CreateAsync(teacherId, teacherId, originalOwnerId, type, subjectId, title, allowSplit);
		}

		private async Task<MActionResult<Zolik>> CreateAsync(int teacherId,
															 int ownerId,
															 int originalOwnerId,
															 ZolikType type,
															 int subjectId,
															 string title,
															 bool allowSplit = true)
		{
			if (ownerId < 1 || originalOwnerId < 1 || subjectId < 1) {
				return new MActionResult<Zolik>(StatusCode.NotValidID);
			}
			if (string.IsNullOrWhiteSpace(title)) {
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
			return await this.CreateAsync(z);
		}

		public async Task<MActionResult<Transaction>> CreateAndTransferAsync(ZolikCPackage p,
																			 User logged)
		{
			if (p == null || !p.IsValid) {
				return new MActionResult<Transaction>(StatusCode.InvalidInput);
			}

			return await CreateAndTransferAsync(p.TeacherId,
												p.ToId,
												p.Type,
												p.SubjectId,
												p.Title,
												logged,
												p.AllowSplit);
		}

		public async Task<MActionResult<Transaction>> CreateAndTransferAsync(int teacherId,
																			 int toId,
																			 ZolikType type,
																			 int subjectId,
																			 string title,
																			 User logged,
																			 bool allowSplit = true)
		{
			var res = await this.CreateAsync(teacherId, toId, type, subjectId, title, allowSplit);
			if (!res.IsSuccess) {
				return new MActionResult<Transaction>(res.Status);
			}
			var z = res.Content;
			var msg = $"Přiřazení od {logged.FullName}";
			var zp = new ZolikPackage(teacherId, toId, z.ID, msg, TransactionAssignment.NewAssignment);
			return await this.TransferAsync(zp, logged);
		}

#endregion

#region Transfer

		public async Task<MActionResult<Transaction>> TransferAsync(ZolikPackage p, User logged)
		{
			if (p == null || logged == null || !p.IsValid) {
				return new MActionResult<Transaction>(StatusCode.InvalidInput);
			}

			if (!logged.IsInRolesOr(UserRoles.Teacher, UserRoles.Administrator, UserRoles.Developer) &&
				logged.ID != p.FromID) {
				return new MActionResult<Transaction>(StatusCode.InsufficientPermissions);
			}
			return await TransferAsync(p.FromID, p.ToID, p.ZolikID, p.Type, p.Message);
		}

		private Task<MActionResult<Transaction>> TransferToBankAsync(int fromId,
																	 int zolikId,
																	 TransactionAssignment type,
																	 string message)
		{
			return this.TransferAsync(fromId, Ext.BankId, zolikId, type, message);
		}

		private Task<MActionResult<Transaction>> TransferToBankAsync(int fromId,
																	 Zolik zolik,
																	 TransactionAssignment type,
																	 string message)
		{
			return this.TransferAsync(fromId, Ext.BankId, zolik, type, message);
		}

		private async Task<MActionResult<Transaction>> TransferAsync(int fromId,
																	 int toId,
																	 int zolikId,
																	 TransactionAssignment type,
																	 string message)
		{
			if (zolikId < 1) {
				return new MActionResult<Transaction>(StatusCode.NotValidID);
			}
			var resZolik = await this.GetByIdAsync(zolikId);
			if (!resZolik.IsSuccess) {
				return new MActionResult<Transaction>(resZolik.Status);
			}

			Zolik z = resZolik.Content;
			if (z.OwnerID != fromId) {
				return new MActionResult<Transaction>(StatusCode.InsufficientPermissions);
			}
			return await TransferAsync(fromId, toId, z, type, message);
		}

		private async Task<MActionResult<Transaction>> TransferAsync(int fromId,
																	 int toId,
																	 Zolik z,
																	 TransactionAssignment type,
																	 string message)
		{
			var uMgr = Context.Get<UserManager>();
			if (!(await uMgr.IdExistsAsync(fromId)) || !(await uMgr.IdExistsAsync(toId))) {
				return new MActionResult<Transaction>(StatusCode.NotValidID);
			}
			if (type != TransactionAssignment.ZerziRemoval &&
				type != TransactionAssignment.NewAssignment &&
				!z.CanBeTransfered) {
				return new MActionResult<Transaction>(StatusCode.CannotTransfer);
			}

			if (type == TransactionAssignment.ZerziRemoval) {
				toId = Ext.BankId; // Remove -> to Bank
			}

			z.OwnerID = toId;
			z.OwnerSince = DateTime.Now;
			if (type == TransactionAssignment.ZerziRemoval) {
				z.Enabled = false;
			}
			try {
				await this.SaveAsync(z);

				var tMgr = Context.Get<TransactionManager>();
				var tranRes = await tMgr.CreateAsync(fromId, toId, z.ID, message, type);

				bool xp = await uMgr.AddXpAsync(5, fromId, toId);

				if (!tranRes.IsSuccess || !xp) {
					return new MActionResult<Transaction>(StatusCode.JustALittleError);
				}
				var tran = tranRes.Content;

				var notify = type.IsNotifyType(); // Nespamovat email při odebírání 
				if (notify) {
					var eMgr = Context.Get<MailManager>();
					var email = await eMgr.NewZolikAsync(toId, z, tran);
					if (!email) {
						return new MActionResult<Transaction>(StatusCode.JustALittleError);
					}

					var fMgr = Context.Get<FirebaseManager>();
					var notification = await fMgr.NewZolikAsync(z, fromId, toId);
				}

				return new MActionResult<Transaction>(StatusCode.OK, tran);
			} catch (Exception ex) {
#if DEBUG
				throw;
#else
				return new MActionResult<Transaction>(SharedLibrary.Enums.StatusCode.SeeException, ex);
#endif
			}
		}

#endregion

#region Split

		public async Task<MActionResult<List<Transaction>>> SplitAsync(int zolikId, User logged)
		{
			if (zolikId < 1) {
				return new MActionResult<List<Transaction>>(StatusCode.NotValidID);
			}
			var res = await this.GetByIdAsync(zolikId);
			if (!res.IsSuccess) {
				return new MActionResult<List<Transaction>>(res.Status);
			}
			var zolik = res.Content;
			return await this.SplitAsync(zolik, logged);
		}

		private async Task<MActionResult<List<Transaction>>> SplitAsync(IZolik zolik, User logged)
		{
			if (zolik == null || logged == null) {
				return new MActionResult<List<Transaction>>(StatusCode.InvalidInput);
			}
			var uMgr = this.Context.Get<UserManager>();
			if (!(await uMgr.IdExistsAsync(logged.ID))) {
				return new MActionResult<List<Transaction>>(StatusCode.InvalidInput);
			}
			if (zolik.OwnerID != logged.ID) {
				return new MActionResult<List<Transaction>>(StatusCode.InsufficientPermissions);
			}
			return await this.SplitAsync(logged.ID, zolik);
		}

		private async Task<MActionResult<List<Transaction>>> SplitAsync(int toId, IZolik zolik)
		{
			if (zolik == null) {
				return new MActionResult<List<Transaction>>(StatusCode.InvalidInput);
			}
			if (zolik.IsLocked) {
				return new MActionResult<List<Transaction>>(StatusCode.CannotTransfer);
			}
			if (!zolik.IsSplittable) {
				return new MActionResult<List<Transaction>>(StatusCode.CannotSplit);
			}
			var type = zolik.Type.GetSplitType();
			if (type == null) {
				return new MActionResult<List<Transaction>>(StatusCode.CannotSplit);
			}

			var msg = "Rozdělen na žolíky";
			var res = await this.TransferToBankAsync(fromId: toId,
													 zolikId: zolik.ID,
													 type: TransactionAssignment.ZerziRemoval,
													 message: msg);
			if (!res.IsSuccess) {
				return new MActionResult<List<Transaction>>(res.Status);
			}
			var delete = res.Content;

			var count = this.GetSplitCount();

			return await this.SplitAsync(toId: toId,
										 joker: zolik,
										 toType: (ZolikType) type,
										 count: count,
										 deleteTran: delete);
		}

		private async Task<MActionResult<List<Transaction>>> SplitAsync(int toId,
																		IZolik joker,
																		ZolikType toType,
																		int count,
																		Transaction deleteTran)
		{
			if (joker == null || string.IsNullOrWhiteSpace(joker.Title)) {
				return new MActionResult<List<Transaction>>(StatusCode.InvalidInput);
			}
			if (joker.SubjectID < 1 || joker.OriginalOwnerID < 1) {
				return new MActionResult<List<Transaction>>(StatusCode.NotValidID);
			}
			var title = joker.Title;
			var subjectId = joker.SubjectID;
			var tMessage = $"Rozdělení \"{joker.Title}\"";
			var originalOwnerId = joker.OriginalOwnerID;
			return await this.SplitAsync(toId, originalOwnerId, toType, subjectId, title, tMessage, count, deleteTran);
		}

		private async Task<MActionResult<List<Transaction>>> SplitAsync(int toId,
																		int originalOwnerId,
																		ZolikType toType,
																		int subjectId,
																		string title,
																		string tMessage,
																		int count,
																		Transaction deleteTran)
		{
			if (deleteTran == null) {
				return new MActionResult<List<Transaction>>(StatusCode.InvalidInput);
			}

			try {
				var transactions = new List<Transaction> {
					deleteTran
				};
				for (int i = 0; i < count; i++) {
					var newTitle = $"{title} [{i + 1}/{count}]";
					var tran = await this.BankCreateAndTransferAsync(toId: toId,
																	 originalOwnerId: originalOwnerId,
																	 toType: toType,
																	 subjectId: subjectId,
																	 title: newTitle,
																	 tMessage: tMessage);
					if (!tran.IsSuccess) {
						return new MActionResult<List<Transaction>>(tran.Status);
					}
					transactions.Add(tran.Content);
				}
				return new MActionResult<List<Transaction>>(StatusCode.OK, transactions);
			} catch (Exception ex) {
#if DEBUG
				throw;
#else
				return new MActionResult<List<Transaction>>(SharedLibrary.Enums.StatusCode.SeeException, ex);
#endif
			}
		}

		private async Task<MActionResult<Transaction>> BankCreateAndTransferAsync(int toId,
																				  int originalOwnerId,
																				  ZolikType toType,
																				  int subjectId,
																				  string title,
																				  string tMessage)
		{
			int bank = Ext.BankId;
			var res = await this.CreateAsync(teacherId: bank,
											 originalOwnerId: originalOwnerId,
											 type: toType,
											 subjectId: subjectId,
											 title: title,
											 allowSplit: false);
			if (!res.IsSuccess) {
				return new MActionResult<Transaction>(res.Status);
			}
			var zolik = res.Content;
			var tran = await this.TransferAsync(fromId: bank,
												toId: toId,
												z: zolik,
												type: TransactionAssignment.Split,
												message: tMessage);
			return tran;
		}

		private int GetSplitCount()
		{
			var rnd = new Random(Guid.NewGuid().GetHashCode()); // Random seed
			return rnd.Next(3, 6); // 3 - 5
		}

#endregion

#region Lock & Unlock

		public async Task<MActionResult<Zolik>> LockAsync(ZolikLock locked, User logged)
		{
			if (locked == null || string.IsNullOrWhiteSpace(locked.Lock)) {
				return new MActionResult<Zolik>(StatusCode.InvalidInput);
			}
			return await this.LockAsync(locked.ZolikId, locked.Lock, logged);
		}

		private async Task<MActionResult<Zolik>> LockAsync(int zolikId, string lockText, User logged)
		{
			if (logged == null) {
				return new MActionResult<Zolik>(StatusCode.InvalidInput);
			}
			if (zolikId < 1) {
				return new MActionResult<Zolik>(StatusCode.NotValidID);
			}

			var res = await this.GetByIdAsync(zolikId);
			if (!res.IsSuccess) {
				return res;
			}
			var zolik = res.Content;
			if (!zolik.Enabled) {
				return new MActionResult<Zolik>(StatusCode.NotEnabled);
			}
			if (zolik.OwnerID != logged.ID) {
				return new MActionResult<Zolik>(StatusCode.InsufficientPermissions);
			}
			return await this.LockAsync(zolik, lockText);
		}

		private async Task<MActionResult<Zolik>> LockAsync(Zolik z, string lockText)
		{
			if (z == null) {
				return new MActionResult<Zolik>(StatusCode.InvalidInput);
			}

			z.Lock = lockText;
			await this.SaveAsync(z);
			return new MActionResult<Zolik>(StatusCode.OK, z);
		}

		public Task<MActionResult<Zolik>> UnlockAsync(int zolikId, User logged)
		{
			return this.LockAsync(zolikId, null, logged);
		}

#endregion

#region Other (count)

		public async Task<int> CountUserZoliksAsync(int userId,
													bool incTester = false,
													ZolikType? type = null)
		{
			if (userId < 1) {
				return 0;
			}

			var query = _ctx.Zoliky.Where(x => x.OwnerID == userId && x.Enabled);
			if (!incTester) {
				query = query.Where(x => x.Type != ZolikType.Debug && x.Type != ZolikType.DebugJoker);
			}
			if (type != null) {
				query = query.Where(x => x.Type == type);
			}

			return await query.CountAsync();
		}

		public async Task<List<int>> GetZolikOwnerIdsAsync()
		{
			var z = await _ctx.Zoliky
							  .Where(x => x.Enabled &&
										  x.Owner.Roles.All(y => y.Name != UserRoles.HiddenStudent) &&
										  x.Type != ZolikType.Debug &&
										  x.Type != ZolikType.DebugJoker)
							  .Select(x => x.OwnerID)
							  .ToListAsync();
			return z;
		}

#endregion

#endregion

#endregion
	}
}