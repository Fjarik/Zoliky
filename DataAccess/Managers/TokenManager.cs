using System;
using System.Collections.Generic;
using System.Data.Entity;
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
	public class TokenManager : IManager<Token>
	{
		ZoliksEntities _ent { get; set; }
		private Manager _mgr;

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenManager"/> class.
		/// </summary>
		/// <param name="ent">The database entities</param>
		/// <param name="mgr">The <see cref="Manager"/></param>
		public TokenManager(ZoliksEntities ent, Manager mgr)
		{
			this._ent = ent;
			this._mgr = mgr;
		}

		/// <summary>
		/// Selects Token by ID
		/// </summary>
		/// <param name="id">Token ID</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.NotFound" />
		/// <exception cref="StatusCode.OK" />
		public MActionResult<Token> Select(int id)
		{
			if (id < 1) {
				return new MActionResult<Token>(StatusCode.NotValidID);
			}
			Token t = _ent.Tokens.Find(id);
			if (t == null) {
				return new MActionResult<Token>(StatusCode.NotFound);
			}
			return new MActionResult<Token>(StatusCode.OK, t);
		}

		public MActionResult<Token> Use(int tokenId)
		{
			if (tokenId < 1) {
				return new MActionResult<Token>(StatusCode.NotValidID);
			}
			using (ZoliksEntities ctx = new ZoliksEntities()) {
				var token = ctx.Tokens.Find(tokenId);
				if (token == null || token.Used) {
					return new MActionResult<Token>(StatusCode.NotFound);
				}
				token.Used = true;
				ctx.Entry(token).State = EntityState.Modified;
				ctx.SaveChanges();
				return new MActionResult<Token>(StatusCode.OK, token);
			}
		}

		/// <summary>
		/// Creates the Token.
		/// </summary>
		/// <param name="userID">The user identifier.</param>
		/// <param name="expiraton">The expiraton.</param>
		/// <param name="purpose">The purpose.</param>
		/// <exception cref="StatusCode.NotValidID" />
		/// <exception cref="StatusCode.OK" />
		[NotNull]
		public MActionResult<Token> Create([NotNull] int userID, DateTime expiraton, [NotNull] string purpose)
		{
			if (userID < 1 || expiraton.ToUniversalTime() <= DateTime.UtcNow || string.IsNullOrWhiteSpace(purpose)) {
				return new MActionResult<Token>(StatusCode.InvalidInput);
			}

			var code = CreateGuid();

			Token t = new Token() {
				UserID = userID,
				Issue = DateTime.UtcNow,
				Expiration = expiraton.ToUniversalTime(),
				Purpose = purpose,
				Type= TokenPurpose.Other,
				Used = false,
				Code = code
			};
			Token t1 = _ent.Tokens.Add(t);
			Save(null);
			return new MActionResult<Token>(StatusCode.OK, t1);
		}

		private Guid CreateGuid()
		{
			var g = Guid.NewGuid();

			var exists = _ent.Tokens.Any(x => x.Code == g);
			if (exists) {
				g = CreateGuid();
			}
			return g;
		}

		/// <summary>
		/// Saves Token
		/// </summary>
		/// <param name="t">The Token to save</param>
		/// <param name="throwException">if set to <c>true</c> throw exception</param>
		/// <exception cref="DbEntityValidationException"></exception>
		public int Save(Token t, bool throwException = true)
		{
			try {
				if (t != null) {
					_ent.Entry(t).State = EntityState.Modified;
					_ent.Tokens.AddOrUpdate(t);
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