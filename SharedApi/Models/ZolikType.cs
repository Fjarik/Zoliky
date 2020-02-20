using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Interfaces;

namespace SharedApi.Models
{
	public partial class ZolikType : IZolikType
	{
		public int ID { get; set; }
		public int? SplitsToID { get; set; }
		public string Name { get; set; }
		public string FriendlyName { get; set; }
		public bool IsSplittable { get; set; }
		public bool IsTestType { get; set; }
		public bool AllowGive { get; set; }
	}
}