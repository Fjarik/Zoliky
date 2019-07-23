using System;
using System.Collections.Generic;
using System.Text;

namespace SharedApi.Models
{
	public partial class Rank
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public int FromXP { get; set; }
		public int? ToXP { get; set; }
		public string Colour { get; set; }
	}
}
