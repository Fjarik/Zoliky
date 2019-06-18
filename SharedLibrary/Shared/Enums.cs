using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace SharedLibrary.Enums
{
	public enum CrashStatus : byte
	{
		[Description("Crash report předán vývojářům")]
		Created = 1,

		[Description("Probíhá kontrolování reportu")]
		Checking = 3,

		[Description("Vývojáři pracují na opravě")]
		WorkingOn = 5,

		[Description("Probáhá testování oprav")]
		Testing = 10,

		[Description("Chyba byla opravena")]
		Repaired = 15,

		[Description("Chyba nemůže být opravena")]
		CantRepair = 20,
	}

	public enum Projects : int
	{
		[Description("Webová aplikace")]
		Web = 1,

		[Description("Android + IOS(Xamarin)")]
		Xamarin = 2,

		[Description("Windows aplikace(UWP)")]
		UWP = 3,

		[Description("Konzole správce(WPF)")]
		WPF = 4,

		[Description("Testování Api")]
		ApiTest = 5,

		[Description("Nová mobilní aplikace(Flutter)")]
		Flutter = 6,

		[Description("Blog, Podpora, FAQ")]
		Support = 7,

		[Description("API")]
		Api = 8,

		[Description("Neznámý")]
		Unknown = 9,

		[Description("Nová webová aplikace (MVC)")]
		WebNew = 10,
	}

	public enum Subjects : int
	{
		[Description("Počítačová grafika")]
		PF = 1,

		[Description("Tělesná výchova")]
		TV = 2,

		[Description("Cvičení z počítačové grafiky")]
		CG = 2,

		[Description("Jiný")]
		Other = 4,
	}

	public enum Terms : int
	{
		[Description("Privacy policy")]
		PP = 1,

		[Description("Terms of usage")]
		TOS = 2,

		[Description("Newsletter")]
		News = 3,

		[Description("DELTA Wiki")]
		Wiki = 4,

		[Description("Planned features")]
		Planned = 5,

		[Description("Testers")]
		Testers = 6
	}

	public enum TransactionAssignment : byte
	{
		[Description("Dárek")]
		Gift = 0,

		[Description("QR kód")]
		QR = 1,

		[Description("Přiřazení")]
		NewAssignment = 2,

		[Description("Odebrání")]
		ZerziRemoval = 3,

		[Description("NFC")]
		NFC = 4,

		[Description("Rozklad jokéra")]
		Split = 5
	}

	public enum Sex : byte
	{
		[Description("Nechci sdělovat své pohlaví")]
		NotKnown = 0,

		[Description("Muž")]
		Male = 1,

		[Description("Žena")]
		Female = 2,

		[Description("Jiné")]
		NotApplicable = 9
	}

	public enum UserPermission : byte
	{
		[Description("Normální uživatel")]
		User = 0,

		[Description("Učitel")]
		Teacher = 1,

		[Description("Vývojář")]
		Dev = 3
	}

	public enum ZolikType : byte
	{
		[Description("Normální žolík")]
		Normal = 0,

		[Description("Jokér")]
		Joker = 1,

		[Description("Černý Petr")]
		Black = 2,

		[Description("Testovací žolík")]
		Debug = 5,

		[Description("Testovací jokér")]
		DebugJoker = 6,
	}

	public enum NotificationType : int
	{
		[Description("Uživatelská notifikace")]
		User = 0,

		[Description("Notifikace pro celý projekt")]
		Project = 5,

		[Description("Globální notifikace")]
		Global = 10
	}

	public enum PageStatus : int
	{
		[Description("je plně funkční")]
		Functional = 0,

		[Description("funguje pouze v omezeném režimu")]
		Limited = 1,

		[Description("je aktuálně nefunkční. Na opravě se pracuje")]
		Unfunctional = 2,

		[Description("je dočasně v údržbě")]
		NotAvailable = 3
	}

	public enum StatusCode : int
	{
		OK = 200,
		SeeException = 303,
		Forbidden = 403,
		NotFound = 404,
		Timeout = 408,
		InternalError = 500,
		NotValidID = 600,
		AlreadyExists = 603,
		InvalidInput = 605,
		NotEnabled = 607,
		WrongPassword = 610,
		NoPassword = 611,
		ExpiredPassword = 613,
		InsufficientPermissions = 615,
		CannotTransfer = 617,
		CannotSplit = 618,
		JustALittleError = 620,
	}

	public enum TokenPurpose : int
	{
		[Description("Jiné")]
		Other = 0,

		[Description("Aktivace")]
		Activation = 1,

		[Description("Resetovaní hesla")]
		PasswordReset = 10,
	}

	public enum LoginStatus : int
	{
		[Description("Úspěch")]
		Success = 200,

		[Description("Špatné heslo")]
		BadPass = 403,

		[Description("Error")]
		Error = 500
	}

	public enum MobileType : int
	{
		Android = 1,
		IOS = 5,
		PC = 7,
		Unknown = 9,
		Other = 10
	}

	public enum WhatToCheck : int
	{
		Username = 1,
		Email = 2,
		Guid = 3
	}

	public enum Teachers : int
	{
		Zerzan = 1,
		Admin = 22,
	}

	public enum ApiUrl : byte
	{
		Localhost = 1,
		Zoliky = 2
	}

	public static class Enumeration
	{
		public static Dictionary<object, string> GetAll<TEnum>() where TEnum : struct
		{
			var enumerationType = typeof(TEnum);

			if (!enumerationType.IsEnum)
				throw new ArgumentException("Enumeration type is expected.");

			var dictionary = new Dictionary<object, string>();

			foreach (var value in Enum.GetValues(enumerationType)) {
				var name = Enum.GetName(enumerationType, value);
				dictionary.Add(value, name);
			}

			return dictionary;
		}

		public static string GetDescription<T>(this T enumerationValue) where T : struct
		{
			Type type = enumerationValue.GetType();
			if (!type.IsEnum) {
				throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
			}

			//Tries to find a DescriptionAttribute for a potential friendly name for the enum
			MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
			if (memberInfo != null && memberInfo.Length > 0) {
				object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attrs != null && attrs.Length > 0) {
					//Pull out the description value
					return ((DescriptionAttribute) attrs[0]).Description;
				}
			}
			//If we have no description attribute, just return the ToString of the enum
			return enumerationValue.ToString();
		}
	}
}