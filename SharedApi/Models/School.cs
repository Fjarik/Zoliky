using System;
using System.Collections.Generic;
using System.Text;

namespace SharedApi.Models
{
	public partial class School
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public School()
		{
			this.Subjects = new HashSet<Subject>();
		}

		public int ID { get; set; }
		public int Type { get; set; }
		public string Name { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public bool AllowTransfer { get; set; }
		public bool AllowTeacherRemove { get; set; }
		public bool AllowZolikSplik { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Subject> Subjects { get; set; }
	}
}
