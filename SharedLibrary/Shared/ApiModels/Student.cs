using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Shared.ApiModels
{
	public class Student : IStudent
	{
		public int ID { get; set; }
		public int? ProfilePhotoID { get; set; }
		public int? ClassID { get; set; }
		public string Name { get; set; }
		public string Lastname { get; set; }
		public string ClassName { get; set; }
		public int ZolikCount { get; set; }
		public int XP { get; set; }

		[JsonIgnore]
		public string FullName => $"{Name} {Lastname}";

		public Student() { }

		public Student(IStudent from)
		{
			this.ID = from.ID;
			this.ProfilePhotoID = from.ProfilePhotoID;
			this.ClassID = from.ClassID;
			this.Name = from.Name;
			this.Lastname = from.Lastname;
			this.ClassName = from.ClassName;
		}
	}

	public class Student<T> : Student, IStudent<T> where T : class, IImage
	{
		public T ProfilePhoto { get; set; }

		public Student() { }

		public Student(IStudent<T> from) : base(from)
		{
			this.ProfilePhoto = from.ProfilePhoto;
		}
	}
}