﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileHelpers;

namespace ZolikyWeb.Areas.Admin.Models.Student
{
	[DelimitedRecord(";")]
	public class ImportStudent
	{
		[FieldOrder(30)]
		public string Email { get; set; }

		[FieldOrder(10)]
		public string Name { get; set; }

		[FieldOrder(20)]
		public string Lastname { get; set; }

		[FieldOrder(40)]
		public string Classname { get; set; }
	}
}