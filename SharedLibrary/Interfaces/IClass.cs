using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IClass : IDbEntity, IEnableable
	{
		string Name { get; set; }
	}
}