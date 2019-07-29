using System;
using System.Collections.Generic;
using System.Text;

namespace SharedApi.Models
{
	public partial class UserSetting
	{
		public int UserID { get; set; }
		public int? ProjectID { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
		public DateTime Changed { get; set; }
	}
}
