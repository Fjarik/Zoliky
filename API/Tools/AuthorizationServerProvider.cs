using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using DataAccess;
using DataAccess.Managers;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

#pragma warning disable CS1998
namespace API.Tools
{
	public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
	{
		public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated();
		}

		public override async Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
		{
			if (context == null) {
				return;
			}
			var identity = new ClaimsIdentity(context.Options.AuthenticationType);
			if (context.GrantType != "Facebook" && context.GrantType != "Google") {
				context.SetError("invalid_grant", "Přihlášení se nezdařilo");
				return;
			}
			if (context.Parameters.All(x => x.Key != "key")) {
				context.SetError("invalid_grant", "Musíte poskytou autorizační klíč pro externí síť");
				return;
			}
			// Get project
			var project = Projects.Unknown;
			if (context.Request.Headers.TryGetValue("projectId", out string[] inputs) && inputs.Length > 0) {
				if (int.TryParse(inputs[0], out int projectId) && Enum.IsDefined(typeof(Projects), projectId)) {
					project = (Projects) projectId;
				}
			}
			var ip = context.Request.RemoteIpAddress;
			var key = context.Parameters.Get("key");
			var mgr = context.OwinContext.Get<UserManager>();
			try {
				// Normal login
				var res = new MActionResult<User>(StatusCode.InternalError);
				switch (context.GrantType) {
					case "Facebook":
						res = await mgr.FbLoginAsync(key, ip, project);
						break;
					case "Google":
						res = await mgr.GoogleLoginAsync(key, ip, project);
						break;
				}
				if (!res.IsSuccess) {
					context.SetError(((int) res.Status).ToString(), res.GetStatusMessage());
					return;
				}
				AddClaims(identity, res.Content);
			} catch {
				context.SetError("invalid_grant", "Přihlášení se nezdařilo");
				return;
			}
			context.Validated(identity);
		}

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			if (context == null) {
				return;
			}
			var identity = new ClaimsIdentity(context.Options.AuthenticationType);
			// Login only - Token
			if (context.UserName.ToLower() == "user" && context.Password == "It9ac8kw") {
				AddClaims(identity, new User() {
					Username = "user",
					Name = "Login",
					Lastname = "Only",
					Roles = new List<Role>() {
						new Role() {
							Name = UserRoles.LoginOnly
						}
					}
				});
				context.Validated(identity);
				return;
			}
			// Get project
			var project = Projects.Unknown;
			if (context.Request.Headers.TryGetValue("projectId", out string[] inputs) && inputs.Length > 0) {
				if (int.TryParse(inputs[0], out int projectId) && Enum.IsDefined(typeof(Projects), projectId)) {
					project = (Projects) projectId;
				}
			}

			var ip = context.Request.RemoteIpAddress;
			var mgr = context.OwinContext.Get<UserManager>();
			try {
				// Normal login
				var res = await mgr.LoginAsync(context.UserName, context.Password, ip, project);
				if (!res.IsSuccess) {
					context.SetError(((int) res.Status).ToString(), res.GetStatusMessage());
					return;
				}
				AddClaims(identity, res.Content);
			} catch {
				context.SetError("invalid_grant", "Přihlášení se nezdařilo");
				return;
			}
			context.Validated(identity);
		}

		private void AddClaims(ClaimsIdentity identity, User u)
		{
			if (u.UQID != Guid.Empty) {
				identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, u.UQID.ToString()));
			}
			if (!string.IsNullOrWhiteSpace(u.Username)) {
				identity.AddClaim(new Claim(ClaimTypes.Name, u.Username));
			}
			if (!string.IsNullOrWhiteSpace(u.Name)) {
				identity.AddClaim(new Claim(ClaimTypes.GivenName, u.Name));
			}
			if (!string.IsNullOrWhiteSpace(u.Lastname)) {
				identity.AddClaim(new Claim(ClaimTypes.Surname, u.Lastname));
			}
			if (!string.IsNullOrWhiteSpace(u.Email)) {
				identity.AddClaim(new Claim(ClaimTypes.Email, u.Email));
			}
			if (u.Class != null && !string.IsNullOrWhiteSpace(u.ClassName)) {
				identity.AddClaim(new Claim("className", u.ClassName));
			} else {
				identity.AddClaim(new Claim("className", "Veřejnost"));
			}
			if (u.School != null && !string.IsNullOrWhiteSpace(u.SchoolName)) {
				identity.AddClaim(new Claim("schoolName", u.SchoolName));
			} else {
				identity.AddClaim(new Claim("schoolName", "Nespecifikovaná škola"));
			}
			identity.AddClaim(new Claim("schoolId", u.SchoolID.ToString()));
			identity.AddClaim(new Claim(ClaimTypes.Gender, u.Sex.ToString()));
			identity.AddClaim(new Claim("publicId", u.ID.ToString()));

			var roles = u.Roles.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToList();
			if (roles.Any()) {
				identity.AddClaims(roles);
			}
		}
	}
}
#pragma warning restore CS1998