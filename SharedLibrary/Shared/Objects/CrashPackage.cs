using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Enums;

namespace SharedLibrary
{
	[Android.Runtime.Preserve(AllMembers = true)]
	public class CrashPackage
	{
		public Projects Project { get; set; }
		public int? FromID { get; set; }
		public string AppVersion { get; set; }
		public DateTime? Build { get; set; }
		public string LastWindows { get; set; }
		public string OS { get; set; }
		public bool? Is64BitOS { get; set; }
		public bool? Is64BitApp { get; set; }
		public DateTime When { get; }
		public Exception Exception { get; set; }
		public string Message { get; set; }
		public string Email { get; set; }
		public string Log { get; set; }

		public CrashPackage() { }

		public CrashPackage(Projects project, int? fromUser, string appVersion, DateTime? build,
							string lastWin, string os, bool? isOS64Bit, bool? isApp64Bit, Exception ex, string log,
							string msg,
							string email)
		{
			this.Project = project;
			this.FromID = fromUser;
			this.AppVersion = appVersion;
			this.Build = build;
			this.LastWindows = lastWin;
			this.OS = os;
			this.Is64BitOS = isOS64Bit;
			this.Is64BitApp = Is64BitApp;
			this.Exception = ex;
			this.Log = log;
			this.Message = msg;
			this.Email = email;
			When = DateTime.Now;
		}
	}
}