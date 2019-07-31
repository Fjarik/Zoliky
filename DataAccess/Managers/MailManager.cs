using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DataAccess.Managers.New;
using DataAccess.Models;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace DataAccess.Managers
{
	public sealed class MailManager : IDisposable
	{
#region ReadOnly Fields

		private readonly IOwinContext _context;

		[NotNull]
		private readonly SmtpClient _smtpClient;

		[NotNull]
		private readonly MailAddress _from;

		[NotNull]
		private readonly string _adminEmail = "admin@zoliky.eu";

		[NotNull]
		private readonly string _noReplyEmail = "noreply@zoliky.eu";

		[NotNull]
		private readonly MailAddress _adminAddress;

		[NotNull]
		private readonly string _footer = "<div>" +
										  "----------------------------------------------------------<br />" +
										  "Toto je automaticky vygenerovaný e-mail,<br />" +
										  "prosíme, neodpovídejte na něj." +
										  "</div>";

#endregion

#region Events

		public delegate void SentEventHandler(object sender, AsyncCompletedEventArgs e);

		public event Mail.SentEventHandler EmailSent;

#endregion

#region Constructor

		private MailManager(IOwinContext context)
		{
			this._context = context;

			NetworkCredential netCred = new NetworkCredential() {
				UserName = _noReplyEmail,
				Password = "H0vniv@l"
			};
			_smtpClient = new SmtpClient() {
				Host = "mail.zoliky.eu",
				EnableSsl = false,
				UseDefaultCredentials = true,
				Credentials = netCred,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				Port = 25
			};
			_smtpClient.SendCompleted += (sender, e) => EmailSent?.Invoke(sender, e);
			_from = new MailAddress(_noReplyEmail, "Žolíky.eu");
			_adminAddress = new MailAddress(_adminEmail);
		}

#endregion

#region Static methods

		public static MailManager Create(IdentityFactoryOptions<MailManager> options,
										 IOwinContext context)
		{
			return new MailManager(context);
		}

#endregion

#region Templates

		public async Task<bool> ActivateAccountAsync(IUser to, string url)
		{
			if (to == null || string.IsNullOrWhiteSpace(url)) {
				return false;
			}

			string body = DataAccess.Properties.Resources.ActivateAccount;

			body = body.Replace("#FullName#", to.FullName);
			body = body.Replace("#UserName#", to.Username);
			body = body.Replace("#ActivateUrl#", url);
			MailMessage email = new MailMessage() {
				From = _from,
				Subject = "[Žolíky] - Aktivace účtu",
				Body = body,
				IsBodyHtml = true,
				Priority = MailPriority.Normal
			};
			email.To.Add(new MailAddress(to.Email));
			return await SendAsync(email);
		}

		public async Task<bool> AdminRegAsync(int whoId, string regIP)
		{
			var res = await this.GetUserAsync(whoId);
			if (res == null) {
				return false;
			}
			return await AdminRegAsync(res, regIP);
		}

		public async Task<bool> AdminRegAsync(IUser who, string regIP)
		{
			if (who == null) {
				return false;
			}

			string body = DataAccess.Properties.Resources.AdminRegistration;

			body = body.Replace("#Username#", who.Username);
			body = body.Replace("#Email#", who.Email);
			body = body.Replace("#FName#", who.Name);
			body = body.Replace("#LName#", who.Lastname);
			body = body.Replace("#Class#", who.ClassName);
			body = body.Replace("#IP#", regIP);
			body = body.Replace("#ID#", who.ID.ToString());

			MailMessage email = new MailMessage() {
				From = _from,
				Subject = "[Žolíky] - Nový uživatel",
				Body = body,
				IsBodyHtml = true,
				Priority = MailPriority.Low
			};
			email.To.Add(_adminAddress);
			return await SendAsync(email);
		}

		public async Task<bool> ForgetPwdAsync(int toId, string url)
		{
			var res = await this.GetUserAsync(toId);
			if (res == null) {
				return false;
			}
			return await ForgetPwdAsync(res, url);
		}

		public async Task<bool> ForgetPwdAsync(IUser to, string url)
		{
			if (to == null || string.IsNullOrWhiteSpace(url)) {
				return false;
			}

			//string url = $"https://www.zoliky.eu/ForgetPwd?{SharedGlobals.Queries.Token}={token}";

			string body = DataAccess.Properties.Resources.ForgottenPassword;

			body = body.Replace("#FullName#", to.FullName);
			body = body.Replace("#UserName#", to.Username);
			body = body.Replace("#ResetUrl#", url);

			MailMessage email = new MailMessage() {
				From = _from,
				Subject = "[Žolíky] - Zapomenuté heslo",
				Body = body,
				IsBodyHtml = true,
				Priority = MailPriority.High
			};
			email.To.Add(new MailAddress(to.Email));
			return await SendAsync(email);
		}

		public async Task<bool> RegisterAsync(int toId, string url)
		{
			var res = await this.GetUserAsync(toId);
			if (res == null) {
				return false;
			}
			return await RegisterAsync(res, url);
		}

		public async Task<bool> RegisterAsync(IUser<Class> to, string activateUrl)
		{
			if (to == null || string.IsNullOrWhiteSpace(activateUrl)) {
				return false;
			}
			//string activateUrl = $"https://www.zoliky.eu/Login?{Ext.Queries.Activate}={token}";

			string body = DataAccess.Properties.Resources.RegisterAccount;

			body = body.Replace("#FullName#", to.FullName);
			body = body.Replace("#UserName#", to.Username);
			body = body.Replace("#UserEmail#", to.Email);
			body = body.Replace("#UserClass#", to.Class?.Name);
			body = body.Replace("#ActivateUrl#", activateUrl);
			MailMessage email = new MailMessage() {
				From = _from,
				Subject = "[Žolíky] - Registrace",
				Body = body,
				IsBodyHtml = true,
				Priority = MailPriority.Normal
			};
			email.To.Add(new MailAddress(to.Email));
			return await SendAsync(email);
		}

		public async Task<bool> NewZolikAsync(int userId, IZolik zolik, ITransaction tran)
		{
			var res = await this.GetUserAsync(userId);
			if (res == null) {
				return false;
			}
			return await NewZolikAsync(res, zolik, tran);
		}

		public async Task<bool> NewZolikAsync(IUser to, IZolik zolik, ITransaction tran)
		{
			if (to == null || zolik == null || tran == null) {
				return false;
			}
			string body = DataAccess.Properties.Resources.NewZolicek;

			body = body.Replace("#FullName#", to.FullName);

			string zType = "Žolík";
			switch (zolik.Type) {
				case ZolikType.Black:
				case ZolikType.Joker:
					zType = zolik.Type.GetDescription();
					break;
				case ZolikType.Debug:
					zType += " (testovací)";
					break;
			}

			body = body.Replace("#ZolikType#", zType);
			body = body.Replace("#ZolikName#", zolik.Title);
			body = body.Replace("#SenderFullName#", tran.From);

			string msg = "Odesílatel neuvedl zprávu";
			if (!string.IsNullOrWhiteSpace(tran.Message)) {
				msg = tran.Message;
			}
			body = body.Replace("#SenderMessage#", msg);

			MailMessage email = new MailMessage() {
				From = _from,
				Subject = "[Žolíky] - Máte nového žolíka",
				Body = body,
				IsBodyHtml = true,
				Priority = MailPriority.High
			};
			email.To.Add(new MailAddress(to.Email));
			return await SendAsync(email);
		}

		public async Task<bool> PasswordChangedAsync(IUser to)
		{
			if (to == null || string.IsNullOrWhiteSpace(to.Email)) {
				return false;
			}
			string body = DataAccess.Properties.Resources.PasswordChanged;

			body = body.Replace("#FullName#", to.FullName);

			MailMessage email = new MailMessage() {
				From = _from,
				Subject = "[Žolíky] - Heslo změněno",
				Body = body,
				IsBodyHtml = true,
				Priority = MailPriority.High
			};
			email.To.Add(new MailAddress(to.Email));
			return await SendAsync(email);
		}

		private async Task<IUser<Class>> GetUserAsync(int id)
		{
			var mgr = _context.Get<UserManager>();
			var res = await mgr.GetByIdAsync(id);
			if (res.IsSuccess) {
				return res.Content;
			}
			return null;
		}

#endregion

#region Send

		public async Task<bool> SendAsync([NotNull] MailMessage email)
		{
			try {
				await _smtpClient.SendMailAsync(email);
				return true;
			} catch {
#if DEBUG
				throw;
#endif
				return false;
			}
		}

		public async Task<bool> SendAsync([NotNull] string subject, [NotNull] string body, [NotNull] MailAddress to,
										  MailPriority priority = MailPriority.Normal)
		{
			if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body)) {
				return false;
			}
			MailMessage email = new MailMessage() {
				From = _from,
				Subject = subject,
				Body = body + _footer,
				IsBodyHtml = true,
				Priority = priority
			};
			email.To.Add(to);
			return await SendAsync(email);
		}

		public Task<bool> SendToAdminAsync([NotNull] string subject, [NotNull] string body,
												 MailPriority priority = MailPriority.Normal)
		{
			return SendAsync(subject, body, _adminAddress, priority);
		}

#endregion

		public void Dispose()
		{
			_smtpClient.Dispose();
		}
	}
}