using System;
using System.Collections.Generic;
using System.Text;

namespace SharedApi.Models
{
	public partial class Ban
	{
		public int ID { get; set; }
		public int UserID { get; set; }
		public System.DateTime From { get; set; }
		public DateTime? To { get; set; }
		public string Reason { get; set; }

	}
}
