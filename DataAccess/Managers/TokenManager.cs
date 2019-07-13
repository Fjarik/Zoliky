using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
{
	public class TokenManager : Manager<Token>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public TokenManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public TokenManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

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

		public static TokenManager Create(IdentityFactoryOptions<TokenManager> options,
										  IOwinContext context)
		{
			return new TokenManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<Token>> GetByCodeAsync(string code)
		{
			if (!Guid.TryParse(code, out Guid g)) {
				return new MActionResult<Token>(StatusCode.InvalidInput);
			}
			return await this.GetByCodeAsync(g);
		}

		public async Task<MActionResult<Token>> GetByCodeAsync(Guid code)
		{
			if (code == Guid.Empty) {
				return new MActionResult<Token>(StatusCode.InvalidInput);
			}
			var token = await _ctx.Tokens
								  .AsNoTracking()
								  .FirstOrDefaultAsync(x => x.Code == code);
			if (token == null) {
				return new MActionResult<Token>(StatusCode.NotFound);
			}
			return new MActionResult<Token>(StatusCode.OK, token);
		}

		public string GenerateCode(Token t)
		{
			string code = t.Code.ToString();

			string purpose = ((int) t.Type).ToString();
			string id = t.UserID.ToString();

			return string.Join("-", code, purpose, id);
		}

		public async Task<TokenResult> IsValidAsync(string toCheck)
		{
			var res = new TokenResult() {
				OriginalCode = toCheck
			};

			if (string.IsNullOrWhiteSpace(toCheck)) {
				res.Errors.Add(TokenValidationStatus.InvalidInput);
				return res;
			}

			// toCheck: ...guid...-P51312 ({Purpose:char}{UserID})
			// TODO: Decrypth token from toCheck
			var array = toCheck.Split('-');
			if (array.Length != 7) {
				res.Errors.Add(TokenValidationStatus.InvalidToken);
				return res;
			}
			var id = array.Last();
			if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out int userId)) {
				res.Errors.Add(TokenValidationStatus.WrongUser);
				return res;
			}

			string purpose = array[5];
			if (string.IsNullOrWhiteSpace(purpose)) {
				res.Errors.Add(TokenValidationStatus.WrongPurpose);
				return res;
			}
			TokenPurpose? purposeType = null;
			if (Enum.TryParse(purpose, out TokenPurpose type)) {
				purposeType = type;
			}

			string guid = string.Join("-", array.Take(5));
			if (!Guid.TryParse(guid, out Guid g)) {
				res.Errors.Add(TokenValidationStatus.WrongUqid);
				return res;
			}

			var tRes = await this.GetByCodeAsync(g);
			if (!tRes.IsSuccess) {
				res.Errors.Add(TokenValidationStatus.InvalidToken);
				return res;
			}
			var token = tRes.Content;

			if (token.Used) {
				// Already used
				res.Errors.Add(TokenValidationStatus.AlreadyUsed);
				return res;
			}

			if (token.Expiration < DateTime.Now) {
				// Expired
				res.Errors.Add(TokenValidationStatus.Expired);
				return res;
			}

			if (token.UserID != userId) {
				// Wrong user
				res.Errors.Add(TokenValidationStatus.WrongUser);
				return res;
			}

			if (purposeType == null) {
				res.Errors.Add(TokenValidationStatus.WrongPurpose);
				return res;
			}

			if (purposeType != token.Type) {
				// Wrong type
				res.Errors.Add(TokenValidationStatus.WrongType);
				return res;
			}

			if (g != token.Code) {
				res.Errors.Add(TokenValidationStatus.NotSame);
				return res;
			}
			res.Token = token;
			return res;
		}

		public async Task<bool> UseAsync(int tokenId)
		{
			if (tokenId < 1) {
				return false;
			}
			var res = await this.GetByIdAsync(tokenId);
			if (!res.IsSuccess) {
				return false;
			}
			var token = res.Content;
			return await this.UseAsync(token);
		}

		public async Task<bool> UseAsync(Token token)
		{
			if (token.Used) {
				return false;
			}
			token.Used = true;
			await this.SaveAsync(token);
			return true;
		}

		public Task<MActionResult<Token>> CreateForgotPwdAsync(int userId)
		{
			return this.CreateAsync(userId, TokenPurpose.PasswordReset, TimeSpan.FromHours(12));
		}

		public Task<MActionResult<Token>> CreateActivationTokenAsync(int userId)
		{
			return this.CreateAsync(userId, TokenPurpose.Activation, TimeSpan.FromHours(20));
		}

		public Task<MActionResult<Token>> CreateAsync(int userId, TokenPurpose purpose)
		{
			return this.CreateAsync(userId, purpose, TimeSpan.FromDays(1));
		}

		public async Task<MActionResult<Token>> CreateAsync(int userId,
															TokenPurpose type,
															TimeSpan expiration)
		{
			if (userId < 1) {
				return new MActionResult<Token>(StatusCode.NotValidID);
			}
			if (expiration < TimeSpan.Zero) {
				return new MActionResult<Token>(StatusCode.InvalidInput);
			}

			var c = await GenerateGuidAsync();

			var t = new Token() {
				UserID = userId,
				Code = c,
				Issue = DateTime.UtcNow,
				Expiration = DateTime.UtcNow.Add(expiration),
				Type = type,
				Used = false
			};
			return await base.CreateAsync(t);
		}

		private async Task<Guid> GenerateGuidAsync()
		{
			var g = Guid.NewGuid();

			var exists = await _ctx.Tokens.AnyAsync(x => x.Code == g);
			if (exists) {
				g = await GenerateGuidAsync();
			}
			return g;
		}

#endregion

#endregion
	}
}