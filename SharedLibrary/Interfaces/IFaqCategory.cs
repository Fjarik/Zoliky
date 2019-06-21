using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Interfaces
{
	public interface IFaqCategory : IDbEntity
	{
		string Name { get; set; }
		string Type { get; set; }
	}
}