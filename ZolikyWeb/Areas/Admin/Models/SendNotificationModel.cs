using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Admin.Models
{
	public class SendNotificationModel
	{
		public int ToId { get; set; }
		public int ZolikId { get; set; }
	}
}