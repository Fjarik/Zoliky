using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Interfaces
{
	public interface IDbEntity : IDbObject
	{
		int ID { get; set; }
	}
}