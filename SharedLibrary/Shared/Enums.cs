using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace SharedLibrary.Enums
{
	public enum TicketStatus : int
	{
		[Description("Nový")] // Požadavek je nový a ještě není přiřazený podpoře
		New = 0,

		[Description("Otevřený")] // Požadavek byl přiřezen podpoře a ta na něm pracuje/již odpověděla
		Open = 2,

		[Description("Vyřešený")] // Požadavek lze znovuotevřít a připsat nové informace
		Solved = 4,

		[Description("Uzamčený")] // Požadavek je vyřešený, uzavřený a NELZE ho již otevřít (automatiky po 28 dnech)
		Closed = 6
	}

	public enum Projects : int
	{
		[Obsolete("Use WebNew instead")]
		[Description("Stará webová aplikace (Web Forms)")]
		Web = 1,

		[Obsolete("Use Flutter instead")]
		[Description("Android + IOS(Xamarin)")]
		Xamarin = 2,

		[Description("Windows aplikace(UWP)")]
		UWP = 3,

		[Obsolete("Use WebNew instead")]
		[Description("Konzole správce(WPF)")]
		WPF = 4,

		[Description("Testování Api")]
		ApiTest = 5,

		[Description("Mobilní aplikace (Flutter)")]
		Flutter = 6,

		[Obsolete("No longer supported :)")]
		[Description("Blog, Podpora, FAQ")]
		Support = 7,

		[Description("API")]
		Api = 8,

		[Description("Neznámý")]
		Unknown = 9,

		[Description("Webová aplikace (MVC)")]
		WebNew = 10,

		[Description("Mobilní aplikace pro vyučující")]
		TeacherApp = 11,
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

	public enum SchoolTypes : byte
	{
		[Description("Základní škola")]
		Zs = 0,

		[Description("Střední škola")]
		Ss = 1,

		[Description("Vysoká škola/Univerzita")]
		Vs = 2
	}

	public enum NotificationSeverity : byte
	{
		[Description("Normální")]
		Normal = 0,

		[Description("Důležitá")]
		Important = 1,

		[Description("Kritická")]
		Critical = 2
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
		Removal = 3,

		[Description("NFC")]
		NFC = 4,

		[Description("Rozklad Jokéra")]
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

	public enum ZolikTypes : int
	{
		[Description("Normální žolík")]
		Normal = 1, // 0, //10->1

		[Description("Jokér")]
		Joker = 2, //1,//11->2

		[Description("Černý Petr")]
		Black = 3, //2,//12->3

		[Description("Testovací žolík")]
		Debug = 4, //5,//15->4

		[Description("Testovací jokér")]
		DebugJoker = 5, //6,//16->5
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
		Banned = 402,
		Forbidden = 403,
		NotFound = 404,
		Timeout = 408,
		InternalError = 500,
		NotValidID = 600,
		AlreadyExists = 603,
		InvalidInput = 605,
		NotEnabled = 607,
		EmailNotConfirmed = 608,
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
		[Description("Aktivace")]
		Activation = 1,

		[Description("Změna hesla")]
		EmailChange = 8,

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
				throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue));
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