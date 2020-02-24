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

#region Achievements

		// Poslední kontrola achievementů
		public static string LastAchievementsCheck => "LastAchievementsCheck";

#endregion

#region Statistics

		public static string[] StatisticsKeys => new[] {
			StatZoliksReceived, StatBlackReceived, StatJokersReceived, StatZoliksSent, StatJokersSent,
			StatZoliksAccepted, StatJokersAccepted, StatZoliksGraphics, StatZoliksZerzan
		};

		// Kolik žolíků získal (celkem)
		public static string StatZoliksReceived => "StatZoliksReceived";

		// Kolik černých petrů získal (celkem)
		public static string StatBlackReceived => "StatBlackPetersReceived";

		// Kolik jokérů získal (celkem)
		public static string StatJokersReceived => "StatJokersReceived";

		// Kolik žolíků daroval jinému studentovi
		public static string StatZoliksSent => "StatZoliksSent";

		// Kolik jokérů daroval jinému studentovi
		public static string StatJokersSent => "StatJokersSent";

		// Kolik žolíků přijal od jiného studenta
		public static string StatZoliksAccepted => "StatZoliksAccepted";

		// Kolik jokérů přijal od jiného studenta
		public static string StatJokersAccepted => "StatJokersAccepted";

		// Kolik žolíků získal (od učitele) z grafiky
		public static string StatZoliksGraphics => "StatZoliksGraphics";

		// Kolik žolíků získal od Zerzána
		public static string StatZoliksZerzan => "StatZoliksZerzan";

		public static string GetZolikKey(string typeName, ZolikStat stat)
		{
			var key = $"Stat{typeName}s";
			switch (stat) {
				case ZolikStat.Received:
					key += "Received";
					break;
				case ZolikStat.Accepted:
					key += "Accepted";
					break;
				case ZolikStat.Sent:
					key += "Sent";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
			}
			return key;
		}

#endregion

		public enum ZolikStat
		{
			Received,
			Accepted,
			Sent,
		}
	}
}