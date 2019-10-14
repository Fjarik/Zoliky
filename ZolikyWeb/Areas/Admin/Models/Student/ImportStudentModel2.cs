using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Admin.Models.Student
{
	public class ImportStudentModel2
	{
		public List<ImportStudent> Students { get; set; }
		public string Password { get; set; }

		public ImportStudentModel2(List<ImportStudent> students)
		{
			this.Students = students;
		}

		public ImportStudentModel2() : this(new List<ImportStudent>()) { }
	}
}