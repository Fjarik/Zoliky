using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface ILogins
	{
		string UName { get; set; }
		string Password { get; set; }
		bool RememberMe { get; set; }
		bool IsValid { get; }

		void ClearPassword();
	}
}