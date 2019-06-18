using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Tools
{
	public enum ToastType
	{
		Error,
		Info,
		Success,
		Warning
	}

	[Serializable]
	public class ToastMessage
	{
		public string Title { get; set; }
		public string Message { get; set; }
		public ToastType ToastType { get; set; }
		public bool IsSticky { get; set; }
	}

	[Serializable]
	public class Toastr
	{
		public bool ShowNewestOnTop { get; set; }
		public bool ShowCloseButton { get; set; }
		public bool ShowProgressBar { get; set; }
		public bool Debug { get; set; }
		public List<ToastMessage> ToastMessages { get; set; }

		public ToastMessage AddToastMessage(string title, string message, ToastType type)
		{
			var toast = new ToastMessage() {
				Title = title,
				Message = message,
				ToastType = type
			};
			ToastMessages.Add(toast);
			return toast;
		}

		public Toastr()
		{
			ToastMessages = new List<ToastMessage>();
			ShowNewestOnTop = true;
			ShowCloseButton = false;
			ShowProgressBar = false;
#if (DEBUG)
			Debug = true;
#else
			Debug = false;
#endif
		}
	}
}