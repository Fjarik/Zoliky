using System;
using System.Collections.Generic;
using System.Text;

namespace SharedApi.Models
{
	public partial class UserSetting
	{
		public int ID { get; set; }
		public int? ProjectId { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
		public DateTime Changed { get; set; }
	}
}
