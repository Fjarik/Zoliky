using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IRank : IDbEntity
	{
		string Title { get; set; }
		int FromXP { get; set; }
		int? ToXP { get; set; }
		string Colour { get; set; }
	}
}