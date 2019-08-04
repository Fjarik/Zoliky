using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IClass : IDbEntity, IEnableable
	{
		int SchoolID { get; set; }
		string Name { get; set; }
		System.DateTime Since { get; set; }
		System.DateTime Graduation { get; set; }
	}
}