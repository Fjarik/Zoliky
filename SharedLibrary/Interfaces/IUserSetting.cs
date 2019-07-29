using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IUserSetting : IDbObject
	{
		int UserID { get; set; }
		int? ProjectID { get; set; }
		string Key { get; set; }
		string Value { get; set; }
		DateTime Changed { get; set; }
	}
}