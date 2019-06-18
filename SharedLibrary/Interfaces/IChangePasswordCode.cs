using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IChangePasswordCode : IPasswordable
	{
		string Code { get; set; }
	}
}