using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IMessage : IDbEntity
	{
		string Message { get; set; }
	}
}
