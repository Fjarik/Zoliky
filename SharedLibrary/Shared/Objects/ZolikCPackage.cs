using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace SharedLibrary
{
	public class ZolikCPackage : IValidable // Create and Transfer zolik
	{
		public int TeacherId { get; set; }
		public int ToId { get; set; }
		public int SubjectId { get; set; }
		public string Title { get; set; }
		public ZolikType Type { get; set; }
		public bool AllowSplit { get; set; }

		public bool IsValid => TeacherId > 0 &&
							   ToId > 0 &&
							   SubjectId > 0 &&
							   !string.IsNullOrWhiteSpace(Title);
	}
}