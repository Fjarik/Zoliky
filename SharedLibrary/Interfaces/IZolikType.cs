using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IZolikType : IDbEntity
	{
		int? SplitsToID { get; set; }
		string Name { get; set; }
		string FriendlyName { get; set; }
		bool IsSplittable { get; set; }
		bool IsTestType { get; set; }
		bool AllowGive { get; set; }
	}
}