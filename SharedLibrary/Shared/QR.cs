using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace SharedLibrary.Shared
{
	public static class QR
	{
		public static Version QRVersion => new Version("1.0.0");

		public static string GetQRString(int userTo, int zolikType) =>
			($"{userTo}*{zolikType}*{DateTime.Today:yyyy-MM-dd}*{QRVersion}");


	}
}
