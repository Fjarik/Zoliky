using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface INameable
	{
		string Name { get; set; }
		string Lastname { get; set; }
		string FullName { get; }
	}

	public interface ITokenable
	{
		string Token { get; set; }
	}

	public interface IEnableable
	{
		bool Enabled { get; set; }
	}

	public interface IEmailable
	{
		string Email { get; set; }
	}

	public interface IValidable
	{
		bool IsValid { get; }
	}
}