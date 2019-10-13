using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Shared
{
	public static class UserRoles
	{
		public const string Teacher = "Teacher";
		public const string Administrator = "Administrator";
		public const string Student = "Student";
		public const string Tester = "Tester";
		public const string Public = "Public";
		public const string FakeStudent = "StudentFake";
		public const string Support = "Support";
		public const string HiddenStudent = "StudentHidden";
		public const string Robot = "Robot";
		public const string Developer = "Developer";
		public const string LoginOnly = "LoginOnly";
		public const string SchoolManager = "SchoolManager";

		// public const string SupportOrAdmin = Support + ", " + Administrator;
		public const string AdminOrDeveloper = Administrator + ", " + Developer;
		public const string AdminOrDeveloperOrTeacher = AdminOrDeveloper + ", " + SchoolManager + ", " + Teacher;
	}
}