using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Shared
{
	public static class SettingKeys
	{

#region Tokens

		public static string MobileToken => "TokenMobile0";

#endregion

#region Emails

		public static string Newsletter => "EmailNewsletter";
		public static string FutureNews => "EmailFutureNews";

#endregion

#region Notifications

		public static string MobileNotification => "NotificationMobile";

#endregion

#region Privacy

		public static string VisibleZolik => "PrivacyZolikVisible";
		public static string LeaderboardZolik => "PrivacyLeaderboardZolik";
		public static string VisibleRank => "PrivacyRankVisible";
		public static string LeaderboardXp => "PrivacyLeaderboardXp";

#endregion

	}
}