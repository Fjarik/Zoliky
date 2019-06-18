using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Shared.ApiModels
{
	public class ZolikSplit
	{
		public virtual int ZolikId { get; set; }

		public virtual bool IsValid => ZolikId > 0;
	}
}