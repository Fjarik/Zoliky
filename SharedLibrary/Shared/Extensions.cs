using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Shared
{
	public static class Extensions
	{
#region Attribute extensions

		public static T GetAttributeFrom<T>(this object instance, string propertyName) where T : Attribute
		{
			var attrType = typeof(T);
			var property = instance.GetType().GetProperty(propertyName);
			if (property == null) {
				return null;
			}
			return (T) property.GetCustomAttributes(attrType, false).First();
		}

#endregion

#region Enum extensions

		public static readonly ZolikType[] TesterTypes = {
			ZolikType.Debug,
			ZolikType.DebugJoker
		};

		public static bool IsTesterType(this ZolikType type)
		{
			return TesterTypes.Contains(type);
		}

		public static bool IsSplittable(this ZolikType type)
		{
			return type == ZolikType.Joker || type == ZolikType.DebugJoker;
		}

		public static ZolikType? GetSplitType(this ZolikType type)
		{
			if (!type.IsSplittable()) {
				return null;
			}
			switch (type) {
				case ZolikType.DebugJoker:
					return ZolikType.Debug;
				case ZolikType.Joker:
					return ZolikType.Normal;
			}
			return null;
		}

		public static bool IsNotifyType(this TransactionAssignment type)
		{
			return type != TransactionAssignment.ZerziRemoval && type != TransactionAssignment.Split;
		}

#endregion

#region String extensions

		public static string NormalizeNames(this string s)
		{
			if (string.IsNullOrWhiteSpace(s)) {
				return s;
			}
			var arr = s.Split(' ');
			var res = arr.Select(x => x.NormalizeName());

			return string.Join(" ", res);
		}

		private static string NormalizeName(this string s)
		{
			if (string.IsNullOrWhiteSpace(s)) {
				return s;
			}
			s = s.Trim().ToLower();
			return s.Substring(0, 1).ToUpper() + s.Substring(1);
		}

		public static T Convert<T>(this string input) where T : struct
		{
			if (string.IsNullOrWhiteSpace(input)) {
				return default;
			}
			try {
				var converter = TypeDescriptor.GetConverter(typeof(T));
				if (converter != null) {
					// Cast ConvertFromString(string text) : object to (T)
					return ((T) converter.ConvertFromString(input));
				}
				return default;
			} catch (NotSupportedException) {
				return default;
			}
		}

#endregion

#region Enumerable extensions

		public static int CountZoliks(this IEnumerable<IZolik> enumerable,
									  bool incTester = false,
									  ZolikType? type = null)
		{
			if (!incTester) {
				enumerable = enumerable.Where(x => !x.Type.IsTesterType());
			}
			if (type != null) {
				enumerable = enumerable.Where(x => x.Type == type);
			}
			return enumerable.Count();
		}

		public static IEnumerable<IZolik> SelectZoliks(this IEnumerable<IZolik> enumerable,
													   bool incTester = false,
													   ZolikType? type = null)
		{
			if (!incTester) {
				enumerable = enumerable.Where(x => !x.Type.IsTesterType());
			}
			if (type != null) {
				enumerable = enumerable.Where(x => x.Type == type);
			}
			return enumerable;
		}

		public static string GetStringValue(this IEnumerable<ISettings> enumerable, string key,
											Projects? project = null)
		{
			if (string.IsNullOrWhiteSpace(key)) {
				return null;
			}
			var res = enumerable.Where(x => x.Key == key &&
											x.ProjectID == (int?) project)
								.Select(x => x.Value)
								.FirstOrDefault();
			return res;
		}

		public static T GetValue<T>(this IEnumerable<ISettings> enumerable,
									string key,
									Projects? project = null) where T : struct
		{
			var res = enumerable.GetStringValue(key, project);
			if (string.IsNullOrWhiteSpace(res)) {
				return default;
			}
			return res.Convert<T>();
		}

		public static T GetRankByXP<T>(this IEnumerable<T> enumberable,
									   int xp) where T : class, IRank
		{
			return enumberable.SingleOrDefault(x => x.FromXP <= xp && xp <= (x.ToXP ?? int.MaxValue));
		}

#endregion

#region DateTime extensions

		public static bool IsBetween(this System.DateTime main, System.DateTime min, System.DateTime max)
		{
			return (min <= main && main <= max);
		}

		public static bool IsChristmasTime(this System.DateTime main)
		{
			return (main.IsBetween(min: new DateTime(main.Year, 1, 1), max: new DateTime(main.Year, 1, 6)) ||
					main.IsBetween(min: new DateTime(main.Year, 12, 10), max: new DateTime(main.Year, 12, 31)));
		}

		public static bool IsPrideTime(this System.DateTime main)
		{
			return main.IsBetween(min: new DateTime(main.Year, 6, 1), max: new DateTime(main.Year, 6, 30));
		}

		public static double GetJsTimestamp(this System.DateTime date)
		{
			return date.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
					   .TotalMilliseconds;
		}

#endregion
	}
}