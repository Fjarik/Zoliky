using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace ZolikyWeb.Tools
{
	public class SignInManager : IDisposable
	{
		private readonly IOwinContext _context;

		private SignInManager(IOwinContext context)
		{
			this._context = context;
		}

		public static SignInManager Create(IdentityFactoryOptions<SignInManager> options, IOwinContext context)
		{
			return new SignInManager(context);
		}

		public void SignIn(User user, bool rememberMe, bool isTester = false)
		{
			if (user == null) {
				return;
			}
			var roles = user.Roles.Select(x => new Claim(ClaimTypes.Role, x.Name));

			var identity = new ClaimsIdentity(new[] {
				new Claim(ClaimTypes.NameIdentifier, user.UQID.ToString()),
				new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "Zoliky",
						  "http://www.w3.org/2001/XMLSchema#string"),
				new Claim(ClaimTypes.Gender, user.Sex.ToString()),
				new Claim("publicId", user.ID.ToString(), ClaimValueTypes.Integer32),
				new Claim("isTester", isTester.ToString(), ClaimValueTypes.Boolean)
			}, DefaultAuthenticationTypes.ApplicationCookie);

			if (!string.IsNullOrWhiteSpace(user.Email)) {
				identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
			}
			if (!string.IsNullOrWhiteSpace(user.Lastname)) {
				identity.AddClaim(new Claim(ClaimTypes.Surname, user.Lastname));
			}
			if (!string.IsNullOrWhiteSpace(user.FullName)) {
				identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
			}
			if (user.Class != null && !string.IsNullOrWhiteSpace(user.ClassName)) {
				identity.AddClaim(new Claim("className", user.ClassName));
			} else {
				identity.AddClaim(new Claim("className", "Veřejnost"));
			}
			if (user.School != null && !string.IsNullOrWhiteSpace(user.SchoolName)) {
				identity.AddClaim(new Claim("schoolName", user.SchoolName));
			} else {
				identity.AddClaim(new Claim("schoolName", "Nespecifikovaná škola"));
			}


			identity.AddClaims(roles);

			_context.Authentication.SignIn(new AuthenticationProperties() {
				IsPersistent = rememberMe,
				IssuedUtc = DateTime.UtcNow,
			}, identity);
		}

		public void ExternalSignIn() { }

		public void SignOut()
		{
			_context.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
		}

		public async Task<bool> SwitchTesterAsync()
		{
			var user = _context.Authentication.User;
			if (user?.Identity.IsAuthenticated != true) {
				return false;
			}
			var logged = await user.GetLoggedUserAsync();
			var isTester = user.Identity.IsTester();
			if (logged == null) {
				return false;
			}
			this.SignOut();
			this.SignIn(logged, false, !isTester);
			return true;
		}

		public void Dispose() { }
	}
}