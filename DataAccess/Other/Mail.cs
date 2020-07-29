using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using DataAccess.Models;
using JetBrains.Annotations;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

namespace DataAccess
{
	public class Mail
	{
#region ReadOnly Fields

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

		public event SentEventHandler EmailSent;

#endregion

#region Constructor

		public Mail()
		{
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

#region Templates

		public bool AdminReg(User who, [NotNull] string regIP)
		{
			if (who == null) {
				return false;
			}
			string body =
				System.IO.File.ReadAllText(HttpContext
										   .Current.Server.MapPath("~/EmailTemplates/AdminRegistration.html"));
			body = body.Replace("#Username#", who.Username);
			body = body.Replace("#Email#", who.Email);
			body = body.Replace("#FName#", who.Name);
			body = body.Replace("#LName#", who.Lastname);
			body = body.Replace("#Class#", who.Class?.Name);
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
			return Send(email);
		}

		public bool ForgetPwd(User to, [NotNull] string token)
		{
			if (to == null || string.IsNullOrWhiteSpace(token)) {
				return false;
			}

			string url = $"http://www.zoliky.eu/ForgetPwd?{Ext.Queries.Token}={token}";

			string body =
				System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/ForgottenPassword.html"));
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
			return Send(email);
		}

		public bool Register(User to, [NotNull] string token)
		{
			if (to == null || string.IsNullOrWhiteSpace(token)) {
				return false;
			}
			string activateUrl = $"http://www.zoliky.eu/Login?{Ext.Queries.Activate}={token}";
			string body =
				System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/RegisterAccount.html"));
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
			return Send(email);
		}

		public bool NewZolik(User to, Zolik zolik, Transaction tran)
		{
			if (to == null || zolik == null || tran == null) {
				return false;
			}
			string body =
				System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/NewZolicek.html"));
			body = body.Replace("#FullName#", to.FullName);

			string zType = zolik.Type.FriendlyName;

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
			return Send(email);
		}

		public bool PasswordChanged(User to)
		{
			if (to == null || string.IsNullOrWhiteSpace(to.Email)) {
				return false;
			}
			string body =
				System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/PasswordChanged.html"));
			body = body.Replace("#FullName#", to.FullName);

			MailMessage email = new MailMessage() {
				From = _from,
				Subject = "[Žolíky] - Heslo změněno",
				Body = body,
				IsBodyHtml = true,
				Priority = MailPriority.High
			};
			email.To.Add(new MailAddress(to.Email));
			return Send(email);
		}

#endregion

#region Send

		public bool Send([NotNull] MailMessage email)
		{
			try {
				_smtpClient.Send(email);
				return true;
			} catch {
#if DEBUG
				throw;
#endif
				return false;
			}
		}

		public bool Send([NotNull] string subject, [NotNull] string body, [NotNull] MailAddress to,
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
			return Send(email);
		}

		public bool SendToAdmin([NotNull] string subject, [NotNull] string body,
								MailPriority priority = MailPriority.Normal)
		{
			return Send(subject, body, _adminAddress, priority);
		}

#endregion
	}
}