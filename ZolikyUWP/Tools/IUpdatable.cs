using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZolikyUWP.Tools
{
	public interface IUpdatable
	{
		bool IsLoading { get; set; }
		DateTime LastUpdate { get; set; }

		Task UpdateAsync();
	}
}