using System;
using System.Collections.Generic;
using SharedLibrary.Enums;

namespace SharedApi.Models
{
	public partial class Transaction
	{
		public int ID { get; set; }
		public int FromID { get; set; }
		public int ToID { get; set; }
		public int ZolikID { get; set; }
		public System.DateTime Date { get; set; }
		public string Message { get; set; }
		public TransactionAssignment Typ { get; set; }
	}
}
