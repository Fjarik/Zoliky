using System;
using System.Collections.Generic;

namespace SharedApi.Models
{
	public partial class News
	{
		public int ID { get; set; }
        public Nullable<int> ProjectID { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public System.DateTime Created { get; set; }
		public Nullable<System.DateTime> Expiration { get; set; }
	}
}
