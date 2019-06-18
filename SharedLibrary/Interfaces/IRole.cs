using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IRole : IDbObject, IDbEntity
	{
		string Name { get; set; }
		string Description { get; set; }
	}
}