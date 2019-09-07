using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IAchievement : IDbEntity
	{
		string Title { get; set; }
		string Description { get; set; }
		int XP { get; set; }
		bool Enabled { get; set; }
		int? ValueToUnlock { get; set; }
	}
}