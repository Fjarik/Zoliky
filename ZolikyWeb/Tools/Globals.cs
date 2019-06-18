using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JetBrains.Annotations;
using SharedLibrary.Enums;

namespace ZolikyWeb.Tools
{
	public static class Globals
	{
#region Project settings

		public static Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

		public static string VersionTrunc => Version.ToString(3);

		public static Version UserVersion => new System.Version(1, 0, 1);

		public static string Name => "Zoliky";

		public static Projects Project => Projects.WebNew;

		public static string CacheBurst => "CacheBurst";

#endregion
	}
}