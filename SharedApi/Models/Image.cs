using System;
using System.Collections.Generic;
using System.Text;

namespace SharedApi.Models
{
	public partial class Image
	{
		public int ID { get; set; }
		public Nullable<int> OwnerID { get; set; }
		public string Hash { get; set; }
		public string Base64 { get; set; }
		public string MIME { get; set; }
		public int Size { get; set; }
	}
}