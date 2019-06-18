using System;
using System.Collections.Generic;
using System.Text;

// ReSharper disable once CheckNamespace
namespace SharedLibrary.Errors
{
	[Serializable]
	public class ExpiredTokenException : Exception
	{
		public ExpiredTokenException() { }

		public ExpiredTokenException(string message) : base(message) { }

		public ExpiredTokenException(string message, Exception innerException) : base(message, innerException) { }
	}
}
