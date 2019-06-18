using System;
using System.Collections.Generic;
using SharedLibrary;
using System.Text;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Interfaces
{
	public interface IConnector<T> where T : class, IDbObject
	{
		MActionResult<T> Get(int id);

	}
}
