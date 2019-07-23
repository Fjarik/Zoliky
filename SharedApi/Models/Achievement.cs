using System;
using System.Collections.Generic;
using System.Text;

namespace SharedApi.Models
{
	public partial class Achievement
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int XP { get; set; }
		public bool Enabled { get; set; }
		public int? UnlockedImageID { get; set; }
		public int? ImageLockedID { get; set; }

		public virtual Image LockedImage { get; set; }
		public virtual Image UnlockedImage { get; set; }
	}
}
