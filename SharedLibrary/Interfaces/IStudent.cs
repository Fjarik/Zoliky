using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IStudent : IDbObject, IDbEntity, INameable
	{
		int? ProfilePhotoID { get; set; }
		int? ClassID { get; set; }
		int SchoolID { get; set; }
		string ClassName { get; }
	}

	public interface IStudent<TImage> : IStudent where TImage : class, IImage
	{
		TImage ProfilePhoto { get; set; }
	}
}