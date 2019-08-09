﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Shared
{
	public static class Ext
	{
#region Extensions

		// Ignored ID
		public static int IgnoreId => -1;

		// Petr Zerzan
		public static int ZerzanId => 1;

		// Administrator
		public static int AdminId => 22;

		// Zolik Bank
		public static int BankId => 1124;

		// Profile photo
		public static int DefaultProfilePhotoId => 4;

		// Other subject
		public static int OtherSubjectId => 4;

		// Regex for Username
		public const string UsernameRegEx = "^[a-zA-Z0-9]*$";

		// Firstname and Lastname Regex
		public const string NameRegEx = "^[a-zA-ZěĚšŠčČřŘžŽýÝáÁíÍéÉúÚůŮóÓňŇťŤďĎ ]+$";

		public static DateTime SchoolYearStart
		{
			get
			{
				var today = DateTime.Today;
				if (today < new DateTime(today.Year, 8, 1)) {
					return new DateTime(today.Year - 1, 8, 1);
				}
				return new DateTime(today.Year, 8, 1);
			}
		}

#endregion

#region Code settings

		public static SessionNames Session => new SessionNames();

		public static ApplicationNames Application => new ApplicationNames();

		public static CookieNames Cookies => new CookieNames();

		public static QueryStringNames Queries => new QueryStringNames();

		public static QueryMessages QMessages => new QueryMessages();

#endregion
	}
}