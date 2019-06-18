using System;
using System.Collections.Generic;

namespace SharedApi.Models
{
	public partial class Class
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public System.DateTime Since { get; set; }
		public System.DateTime Graduation { get; set; }
		public bool Enabled { get; set; }
	}
}
