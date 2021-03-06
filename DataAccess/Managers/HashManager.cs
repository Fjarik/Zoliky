﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class HashManager : Manager<Password>
	{
		/// 
		/// Fields, Constants
		///
		private const int SaltByteSize = 32;

		private const string CurrentPasswordVersion = "1.0.2";

		private IPasswordHasher Hasher { get; set; }

		///  
		/// Constructors
		/// 
		public HashManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public HashManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx)
		{
			this.Hasher = new PasswordHasher();
		}

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static HashManager Create(IdentityFactoryOptions<HashManager> options,
										 IOwinContext context)
		{
			return new HashManager(context);
		}

		private static byte[] GenerateSaltBytes(int saltByteSize = SaltByteSize)
		{
			using (RNGCryptoServiceProvider saltGenerator = new RNGCryptoServiceProvider()) {
				byte[] salt = new byte[saltByteSize];
				saltGenerator.GetNonZeroBytes(salt);
				return salt;
			}
		}

		private static string GenerateSaltString(out byte[] salt, int saltByteSize = SaltByteSize)
		{
			salt = GenerateSaltBytes(saltByteSize);
			return Convert.ToBase64String(salt);
		}

#endregion

#region Own Methods

		public PasswordVerificationResult VerifyHashedPassword(int userId, Password hash, string password)
		{
			if (userId < 1 || hash == null || string.IsNullOrWhiteSpace(password)) {
				return PasswordVerificationResult.Failed;
			}
			if (hash.Version == "1.0.1") {
				// Without salt, password same
			}
			if (hash.Version == "1.0.2") {
				// With salt
				password = $"{userId}{password}{Convert.ToBase64String(hash.Salt)}{userId}";
			}
			var res = Hasher.VerifyHashedPassword(hash.Hash, password);
			if (res == PasswordVerificationResult.Success && hash.Version != CurrentPasswordVersion) {
				return PasswordVerificationResult.SuccessRehashNeeded;
			}
			return res;
		}

		public Task<MActionResult<Password>> CreatePasswordAsync(string password,
																 int ownerId,
																 DateTime? exp = null)
		{
			var salt = GenerateSaltString(out byte[] saltBytes);

			// {id}{password}{salt}{id}
			password = $"{ownerId}{password}{salt}{ownerId}";

			var hash = Hasher.HashPassword(password);
			return this.CreateAsync(ownerId, hash, saltBytes, exp);
		}

		private async Task<MActionResult<Password>> CreateAsync(int ownerId,
																string hash,
																byte[] salt,
																DateTime? exp = null,
																string version = CurrentPasswordVersion)
		{
			if (ownerId < 1) {
				return new MActionResult<Password>(StatusCode.NotValidID);
			}
			if (string.IsNullOrWhiteSpace(hash)) {
				return new MActionResult<Password>(StatusCode.InvalidInput);
			}

			var ent = new Password() {
				OwnerID = ownerId,
				Created = DateTime.Now,
				Expiration = exp,
				Hash = hash,
				Salt = salt,
				Version = version
			};
			return await this.CreateAsync(ent);
		}

		public async Task<IList<Password>> GetAllAsync(int userId)
		{
			if (userId < 1) {
				return new List<Password>();
			}
			var res = await _ctx.Passwords.Where(x => x.OwnerID == userId).ToListAsync();

			return res;
		}

#endregion

#endregion
	}
}