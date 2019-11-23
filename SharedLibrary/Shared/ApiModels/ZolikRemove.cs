using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SharedLibrary.Shared.ApiModels
{
	public class ZolikRemove
	{
		public int ZolikID { get; set; }
		public string Reason { get; set; }

		[JsonIgnore]
		public bool IsValid => this.ZolikID > 0 &&
							   !string.IsNullOrWhiteSpace(this.Reason) &&
							   this.Reason.Length > 2;
	}
}