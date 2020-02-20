using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Enums;

namespace SharedLibrary.Interfaces
{
	public interface IZolik : IDbEntity, IEnableable
	{
		int OwnerID { get; set; }
		int SubjectID { get; set; }
		int TeacherID { get; set; }
		int OriginalOwnerID { get; set; }

		string Title { get; set; }
		DateTime OwnerSince { get; set; }
		DateTime Created { get; set; }
		string Lock { get; set; }

		bool IsLocked { get; }
		bool CanBeTransfered { get; }
		bool IsSplittable { get; }
	}

	public interface IZolik<TZolikType> : IZolik where TZolikType : IZolikType
	{
		int TypeID { get; set; }
		TZolikType Type { get; set; }
	}
}