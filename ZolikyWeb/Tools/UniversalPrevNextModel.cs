using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Tools
{
	public class UniversalPrevNextModel
	{
		public int PreviousID { get; set; }
		public int NextID { get; set; }

		public UniversalPrevNextModel() { }

		public UniversalPrevNextModel(int previousID, int nextID)
		{
			this.PreviousID = previousID;
			this.NextID = nextID;
		}
	}
}