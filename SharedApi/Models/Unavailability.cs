using System;
using System.Collections.Generic;
using System.Text;

namespace SharedApi.Models
{
	public partial class Unavailability
	{
		public int ID { get; set; }
		public int ProjectID { get; set; }
		public string Reason { get; set; }
		public string Description { get; set; }
		public System.DateTime From { get; set; }
		public System.DateTime To { get; set; }
	}
}
