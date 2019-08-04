using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Interfaces;

namespace ZolikyWeb.Tools.Interfaces
{
	public interface IModel : IValidable
	{
		bool AllowRemove { get; }
		bool AllowEdit { get; set; }

		bool IsCreate { get; set; }

		int PreviousID { get; set; }
		int ID { get; set; }
		int NextID { get; set; }

		string ActionName { get; set; }

		bool IsFirst { get; }
		bool IsLast { get; }

	}
}
