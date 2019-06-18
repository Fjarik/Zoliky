using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace SharedLibrary
{
	[Android.Runtime.Preserve(AllMembers = true)]
	public class MActionResult<T> where T : class
	{
		[CanBeNull]
		private T _content;

		public T Content
		{
			get => _content;
			set => this._content = value;
		}

		public Exception Exception { get; set; }
		public bool IsSuccess => (Status == StatusCode.OK && Content != null);
		public StatusCode Status { get; set; }
		public Version Version { get; set; } = System.Version.Parse("1.0.0");
		public DateTime Date { get; set; }

		public MActionResult() { }

		public MActionResult(StatusCode status) : this(status, default(T), null) { }

		public MActionResult(StatusCode status, T result) : this(status, result, null) { }

		public MActionResult(StatusCode status, Exception exception) : this(status, default(T), exception)
		{
			if (exception == null) {
				throw new ArgumentNullException("Exception is null");
			}
		}

		public MActionResult(StatusCode status, T result, Exception exception)
		{
			if (result == null) {
				this.Content = default(T);
			} else {
				Type currentType = typeof(T);

				bool isCollection = currentType.IsGenericType &&
									currentType.GetGenericTypeDefinition() == typeof(List<>) &&
									result.GetType().GetGenericArguments().Single().GetInterfaces()
										  .Contains(typeof(IDbObject));

				if (!(currentType.GetInterfaces().Contains(typeof(IDbObject)) || isCollection)) {
					throw new ArgumentException("content",
												"Type must Implement IDbObject be List<>, but was " +
												currentType.FullName);
				}

				this.Content = result;
			}

			this.Status = status;
			this.Exception = exception;
			this.Date = DateTime.Now;
		}

		public string GetStatusMessage()
		{
			switch (this.Status) {
				case StatusCode.OK:
					return "Vše v pořádku";
				case StatusCode.SeeException:
					string s = "Vyskytla se chyba";
					if (this.Exception != null) {
						s += $", podrobnosti: {this.Exception.Message}";
					}
					return s;
				case StatusCode.NoPassword:
					return "Nemáte vytvořené heslo. Použijte funkci zapomenuté heslo";
				case StatusCode.Forbidden:
					return "Zakázáno";
				case StatusCode.NotFound:
					return "Nenalezeno";
				case StatusCode.Timeout:
					return "Vypršel časový limit na danou operaci";
				case StatusCode.InternalError:
					return "Vyskytla se chyba na straně serveru";
				case StatusCode.NotValidID:
					return "Poskytnuté ID není platné";
				case StatusCode.AlreadyExists:
					return "Tento záznam již existuje";
				case StatusCode.InvalidInput:
					return "Neplatné vstupní parametry";
				case StatusCode.NotEnabled:
					return "Uživatel není aktivovaný";
				case StatusCode.WrongPassword:
					return "Neplatné jméno nebo  heslo";
				case StatusCode.ExpiredPassword:
					return "Zadané heslo již není platné";
				case StatusCode.InsufficientPermissions:
					return "Na tutu akci nemáte dostatečné oprávnění";
				case StatusCode.CannotTransfer:
					return "Vybraného žolíka nelze přenést";
				case StatusCode.CannotSplit:
					return "Vybraného žolík nelze rozdělit";
				case StatusCode.JustALittleError:
					return "Vyskytla se chyba, ale pouze malá :)";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}