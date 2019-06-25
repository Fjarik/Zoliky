using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace SharedLibrary
{
	[Android.Runtime.Preserve(AllMembers = true)]
	public class ZolikPackage : IValidable
	{
		public int FromID { get; set; }
		public int ToID { get; set; }
		public int ZolikID { get; set; }
		public string Message { get; set; }
		public TransactionAssignment Type { get; set; }

		[JsonIgnore]
		public bool IsValid => (FromID > 0 && ToID > 0 && ZolikID > 0 && FromID != ToID);

		public ZolikPackage() { }

		public ZolikPackage(int fromId, int toId, int zolikId, string message, TransactionAssignment type)
		{
			FromID = fromId;
			ToID = toId;
			ZolikID = zolikId;
			Message = message;
			Type = type;
		}
	}

}
