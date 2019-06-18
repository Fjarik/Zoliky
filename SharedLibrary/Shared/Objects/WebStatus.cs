using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using SharedLibrary.Enums;

namespace SharedLibrary
{
	[Android.Runtime.Preserve(AllMembers = true)]
	public class WebStatus
	{
		public PageStatus Status { get; set; }

		public string Message { get; set; }

		public object Content { get; set; }

		public bool CanAccess => (Status == PageStatus.Functional);

		public WebStatus() { }

		public WebStatus(PageStatus status, string msg = null, object content = null)
		{
			this.Status = status;
			this.Message = msg;
			this.Content = content;
		}
	}
}