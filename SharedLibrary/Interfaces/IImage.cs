using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IImage : IDbEntity
	{
		int? OwnerID { get; set; }
		string Base64 { get; set; }
		string MIME { get; set; }
		int Size { get; set; }
	}
}