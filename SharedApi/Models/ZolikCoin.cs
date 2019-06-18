using System;
using System.Collections.Generic;

namespace SharedApi.Models
{
	public partial class ZolikCoin
	{
		public int ID { get; set; }
		public int OwnerID { get; set; }
		public int XP { get; set; }
		public Nullable<System.DateTime> OwnerSince { get; set; }
		public bool Enabled { get; set; }
	}

}
