using System;
using System.Collections.Generic;
using System.Text;

namespace SharedApi.Models
{
	public partial class Crash
	{
		public int ID { get; set; }
		public int ProjectID { get; set; }
		public Nullable<int> ScreenshotID { get; set; }
		public Nullable<int> UserID { get; set; }
		public int Status { get; set; }
		public string AppVersion { get; set; }
		public Nullable<System.DateTime> Build { get; set; }
		public string LastWindowName { get; set; }
		public string OS { get; set; }
		public Nullable<bool> Is64BitOS { get; set; }
		public Nullable<bool> Is64BitApp { get; set; }
		public System.DateTime Date { get; set; }
		public string Exception { get; set; }
		public string Message { get; set; }
		public string Email { get; set; }
		public string Log { get; set; }
		public bool Enabled { get; set; }
	}
}
