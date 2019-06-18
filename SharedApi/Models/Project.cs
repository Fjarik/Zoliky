using System;
using System.Collections.Generic;

namespace SharedApi.Models
{
	public partial class Project
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }
		public Nullable<int> ImageID { get; set; }
	}
}
