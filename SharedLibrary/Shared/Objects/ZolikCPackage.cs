using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace SharedLibrary
{
	public class ZolikCPackage : IValidable // Create and Transfer zolik
	{
		public int TeacherID { get; set; }
		public int ToID { get; set; }
		public int SubjectID { get; set; }
		public string Title { get; set; }
		public ZolikType Type { get; set; }
		public bool AllowSplit { get; set; }

		public bool IsValid => TeacherID > 0 &&
							   ToID > 0 &&
							   SubjectID > 0 &&
							   !string.IsNullOrWhiteSpace(Title);
	}
}