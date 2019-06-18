using System;
using System.Collections.Generic;

namespace SharedApi.Models
{
	public partial class Changelog
	{
		public int ID { get; set; }
		public int ProjectID { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
		public System.DateTime Date { get; set; }
		public string Version { get; set; }
		public bool Visible { get; set; }

	}

}
