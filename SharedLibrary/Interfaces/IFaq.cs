using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Interfaces
{
	public interface IFaq : IDbEntity
	{
		int CategoryID { get; set; }
		string Question { get; set; }
		string Response { get; set; }
	}
}
