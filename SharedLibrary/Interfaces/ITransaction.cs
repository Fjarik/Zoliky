using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Enums;

namespace SharedLibrary.Interfaces
{
	public interface ITransaction : IDbEntity
	{
		int FromID { get; set; }
		int ToID { get; set; }
		int ZolikID { get; set; }
		string Message { get; set; }
		TransactionAssignment Typ { get; set; }

		string From { get; }
		string To { get; }
	}
}