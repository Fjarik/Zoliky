using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace SharedLibrary.Shared
{
	public static class Methods
	{
		public static bool IsEmailValid([NotNull] string email)
		{
			if (string.IsNullOrWhiteSpace(email)) {
				return false;
			}
			try {
				System.Net.Mail.MailAddress mail = new System.Net.Mail.MailAddress(email);
				return email == mail.Address;
			} catch {
				return false;
			}
		}

		public static bool AreNullOrWhiteSpace(params string[] texts)
		{
			return texts.Any(x => x == null || string.IsNullOrWhiteSpace(x));
		}
	}
}