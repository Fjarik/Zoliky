using System;
using System.Collections.Generic;

namespace SharedApi.Models
{
	public partial class Price
	{
		public int ID { get; set; }
		public System.DateTime Date { get; set; }
		public double Value { get; set; }
	}
}
