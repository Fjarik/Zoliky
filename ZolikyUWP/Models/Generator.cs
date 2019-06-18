using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedApi.Models;

namespace ZolikyUWP.Models
{
	public class Generator
	{
		public IList<DefaultTile> GetDefaultTiles(User u)
		{
			IList<DefaultTile> list = new List<DefaultTile>()
			{
				new DefaultTile()
				{

				},
				new DefaultTile()
				{

				}

			};

			return list;
		}
	}
}
