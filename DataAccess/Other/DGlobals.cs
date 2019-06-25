using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DataAccess
{
	public static class DGlobals
	{
#region Support

		public const int CodeMin = 10101;
		public const int CodeMax = 98989;

#endregion

		public static string GetIPAddress(System.Web.HttpContext context)
		{
			string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (!string.IsNullOrWhiteSpace(ipAddress)) {
				string[] addresses = ipAddress.Split(',');
				if (addresses.Length != 0) {
					string ip = addresses[0];
					ip = ip.Remove(ip.IndexOf(':'));
					return ip;
				}
			}

			if (context.Request.ServerVariables["REMOTE_ADDR"] == "::1") {
				return "127.0.0.1";
			}

			return context.Request.ServerVariables["REMOTE_ADDR"];
		}

		public static string GetIPAddress(System.Net.Http.HttpRequestMessage msg)
		{
			if (msg == null) {
				return "";
			}

			string key = "MS_HttpContext";
			if (msg.Properties.ContainsKey(key)) {
				object obj = msg.Properties[key];
				if (obj == null) {
					return "";
				}
				string ip = "";

				if (obj is System.Web.HttpContextWrapper context) {
					ip = context.Request.UserHostAddress;
				}

				if (ip == "::1") {
					return "127.0.0.1";
				}

				if (ip == null) {
					return "";
				}
				return ip;
			}

			return "";
		}
	}
}