using System;
using System.Collections.Generic;
using System.Text;
using SharedLibrary.Enums;

namespace SharedLibrary
{
	[Android.Runtime.Preserve(AllMembers = true)]
	public class RegisterUser
	{
		public string Email { get; set; }
		public string Pwd { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public Sex Gender { get; set; }
		public int ClassID { get; set; }
		public string UserName { get; set; }
		public string IP { get; set; }
		public bool TOS { get; set; }
		public bool PP { get; set; }
		public bool Wiki { get; set; }
		public bool Newsletter { get; set; }
		public bool Other { get; set; }
	}
}
