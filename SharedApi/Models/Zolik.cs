using System;
using System.Collections.Generic;
using SharedLibrary.Enums;

namespace SharedApi.Models
{
	public partial class Zolik
	{
		public int ID { get; set; }
		public int OwnerID { get; set; }
		public int SubjectID { get; set; }
		public int TeacherID { get; set; }
		public int OriginalOwnerID { get; set; }
		public int TypeID { get; set; }
		public ZolikType Type { get; set; }
		public string Title { get; set; }
		public bool Enabled { get; set; }
		public System.DateTime OwnerSince { get; set; }
		public System.DateTime Created { get; set; }
		public string Lock { get; set; }
		public bool AllowSplit { get; set; }
	}
}