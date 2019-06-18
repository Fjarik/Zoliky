using System;
using System.Collections.Generic;
using System.Text;

namespace SharedApi.Models
{
	public partial class WebEvent
	{
		public int ID { get; set; }
		public int FromProjectID { get; set; }
		public Nullable<int> FromID { get; set; }
		public int ToID { get; set; }
		public int Type { get; set; }
		public System.DateTime Date { get; set; }
		public bool Enabled { get; set; }
		public string Message { get; set; }
	}
}
