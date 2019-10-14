using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Admin.Models.Student
{
	public class ImportStudentsModel
	{
		public HttpPostedFileBase File { get; set; }

		public bool HasHeader { get; set; }

		public ImportStudentModel2 Step2 { get; set; }

		public ImportStudentsModel(List<ImportStudent> students)
		{
			this.Step2 = new ImportStudentModel2(students);
		}

		public ImportStudentsModel() : this(new List<ImportStudent>()) { }
	}
}