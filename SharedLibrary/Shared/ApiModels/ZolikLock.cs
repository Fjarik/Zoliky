using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SharedLibrary.Shared.ApiModels
{
	public class ZolikLock
	{
		public virtual int ZolikId { get; set; }
		public virtual string Lock { get; set; }

		[JsonIgnore]
		public bool IsValid => this.ZolikId > 0 && !string.IsNullOrWhiteSpace(this.Lock);

	}
}
