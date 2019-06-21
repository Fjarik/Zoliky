using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public sealed class LoginTokenManager : BaseManager<UserLoginToken>, IDisposable
	{
#region Constructors

		public LoginTokenManager(IOwinContext context) : this(context, new ZoliksEntities()) { }
		public LoginTokenManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

#endregion

#region Methods

#region Static methods

		public static LoginTokenManager Create(IdentityFactoryOptions<LoginTokenManager> options,
											   IOwinContext context)
		{
			return new LoginTokenManager(context);
		}

#endregion

#region Own methods

		public async Task<MActionResult<UserLoginToken>> GetAsync(UserLoginInfo info)
		{
			if (info == null) {
				return new MActionResult<UserLoginToken>(StatusCode.InvalidInput);
			}
			return await this.GetAsync(info.LoginProvider, info.ProviderKey);
		}

		public async Task<MActionResult<UserLoginToken>> GetAsync(string provider, string key)
		{
			if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(key)) {
				return new MActionResult<UserLoginToken>(StatusCode.InvalidInput);
			}

			var obj = await _ctx.UserLoginTokens
								.AsNoTracking()
								.FirstOrDefaultAsync(x =>
														 x.LoginProvider.ToLower() == provider.ToLower() &&
														 x.ProviderKey.ToLower() == key.ToLower());
			if (obj == null) {
				return new MActionResult<UserLoginToken>(StatusCode.NotFound);
			}

			return new MActionResult<UserLoginToken>(StatusCode.OK, obj);
		}

		public async Task<MActionResult<UserLoginToken>> GetAsync(int userId, string provider)
		{
			if (userId < 1) {
				return new MActionResult<UserLoginToken>(StatusCode.NotValidID);
			}
			if (string.IsNullOrWhiteSpace(provider)) {
				return new MActionResult<UserLoginToken>(StatusCode.InvalidInput);
			}

			var res = await _ctx.UserLoginTokens
								.AsNoTracking()
								.FirstOrDefaultAsync(x => x.UserID == userId &&
														  x.LoginProvider.ToLower() == provider.ToLower());
			if (res == null) {
				return new MActionResult<UserLoginToken>(StatusCode.NotFound);
			}
			return new MActionResult<UserLoginToken>(StatusCode.OK, res);
		}

		public async Task<MActionResult<List<UserLoginToken>>> GetAllAsync(int userId)
		{
			if (userId < 1) {
				return new MActionResult<List<UserLoginToken>>(StatusCode.NotValidID);
			}
			var res = await _ctx.UserLoginTokens
								.AsNoTracking()
								.Where(x => x.UserID == userId)
								.ToListAsync();
			return new MActionResult<List<UserLoginToken>>(StatusCode.OK, res);
		}

		public async Task<bool> ExistsAsync(string provider, string providerKey)
		{
			if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(providerKey)) {
				return false;
			}
			var res = await _ctx.UserLoginTokens
								.AsNoTracking()
								.AnyAsync(x => x.LoginProvider.ToLower() == provider.ToLower() &&
											   x.ProviderKey == providerKey);
			return res;
		}

		public async Task<MActionResult<UserLoginToken>> CreateAsync(UserLoginToken entity)
		{
			var ent = _ctx.UserLoginTokens.Add(entity);
			var changes = await base.SaveAsync();
			if (changes == 0) {
				return new MActionResult<UserLoginToken>(StatusCode.InternalError);
			}
			var res = await this.GetAsync(ent.LoginProvider, ent.ProviderKey);
			if (res.IsSuccess) {
				ent = res.Content;
			}
			return new MActionResult<UserLoginToken>(StatusCode.OK, ent);
		}

		public async Task<MActionResult<UserLoginToken>> CreateAsync(int userId, string provider, string key)
		{
			if (userId < 1) {
				return new MActionResult<UserLoginToken>(StatusCode.NotValidID);
			}

			if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(key)) {
				return new MActionResult<UserLoginToken>(StatusCode.InvalidInput);
			}

			if (await this.ExistsAsync(provider, key)) {
				return new MActionResult<UserLoginToken>(StatusCode.AlreadyExists);
			}

			var t = new UserLoginToken() {
				UserID = userId,
				LoginProvider = provider,
				ProviderKey = key,
				Created = DateTime.Now
			};
			return await CreateAsync(t);
		}

		public async Task<bool> DeleteAsync(int userId, string provider)
		{
			if (userId < 1 || string.IsNullOrWhiteSpace(provider)) {
				return false;
			}
			var tRes = await this.GetAsync(userId, provider);
			return await this.DeleteAsync(tRes);
		}

		public async Task<bool> DeleteAsync(string provider, string providerKey)
		{
			if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(providerKey)) {
				return false;
			}
			var tRes = await this.GetAsync(provider, providerKey);
			return await this.DeleteAsync(tRes);
		}

		private async Task<bool> DeleteAsync(MActionResult<UserLoginToken> res)
		{
			if (res == null || !res.IsSuccess) {
				return false;
			}
			var token = res.Content;
			return await this.DeleteAsync(token);
		}

#endregion

#endregion
	}
}