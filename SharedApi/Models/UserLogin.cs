﻿using System;
using System.Collections.Generic;
using SharedLibrary.Enums;

namespace SharedApi.Models
{
	public partial class UserLogin
	{
		public int ID { get; set; }
		public int UserID { get; set; }
		public int ProjectID { get; set; }
		public System.DateTime Date { get; set; }
		public LoginStatus Status { get; set; }
		public string Message { get; set; }
		public string IP { get; set; }
	}
}