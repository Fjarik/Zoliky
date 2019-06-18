using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess
{
	public class TokenResult
	{
		public Token Token { get; set; }

		public string OriginalCode { get; set; }

		public readonly IList<TokenValidationStatus> Errors = new List<TokenValidationStatus>();

		public bool IsValid => this.Errors.Count == 0;

		public string GetErrorMessages()
		{
			var msg = "";
			foreach (var err in this.Errors) {
				switch (err) {
					case TokenValidationStatus.AlreadyUsed:
						msg += "Token byl již použit. ";
						break;
					case TokenValidationStatus.NotSame:
					case TokenValidationStatus.WrongUser:
					case TokenValidationStatus.WrongType:
					case TokenValidationStatus.WrongPurpose:
						msg += "Token byl se neshoduje s databází. ";
						break;
					case TokenValidationStatus.Expired:
						msg += "Token již není platný. ";
						break;
					default:
						msg += "Token není platný. ";
						break;
				}
			}
			return msg;
		}
	}

	public enum TokenValidationStatus
	{
		InvalidInput,
		WrongId,
		InvalidToken,
		AlreadyUsed,
		NotSame,
		Expired,
		WrongUser,
		WrongPurpose,
		WrongUqid,
		WrongType,
	}
}