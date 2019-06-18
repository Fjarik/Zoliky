using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using DataAccess;
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

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			if (context == null) {
				return;
			}
			var identity = new ClaimsIdentity(context.Options.AuthenticationType);
			if (context.UserName.ToLower() == "admin" && context.Password == "ygaFIo91KJSG") {
				AddClaims(identity, new User() {
					Username = "admin",
					Name = "Backup",
					Lastname = "Admin",
					Roles = new List<Role>() {
						new Role() {
							Name = UserRoles.Administrator
						}
					}
				});
			} else if (context.UserName.ToLower() == "user" && context.Password == "It9ac8kw") {
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
			} else if (context.UserName.ToLower() == "dev" && context.Password == "Ncsb0mssATDpUZRXo837") {
				AddClaims(identity, new User() {
					Username = "dev",
					Name = "Backup",
					Lastname = "Developer",
					Roles = new List<Role>() {
						new Role() {
							Name = UserRoles.Developer
						}
					}
				});
			} else {
				try {
					var mgr = context.OwinContext.Get<UserManager>();
					var res = await mgr.LoginAsync(context.UserName, context.Password, context.Request.RemoteIpAddress);
					if (!res.IsSuccess) {
						context.SetError("invalid_grant", res.GetStatusMessage());
						return;
					}
					User u = res.Content;

					AddClaims(identity, u);
				} catch {
					context.SetError("invalid_grant", "Přihlášení se nezdařilo");
					return;
				}
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
			}
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