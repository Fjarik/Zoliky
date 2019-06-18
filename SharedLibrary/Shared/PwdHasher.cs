using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
	/*
	[Android.Runtime.Preserve(AllMembers = true)]
	public sealed class PwdHasher
	{
		public static string Hash(string Password)
		{
			if (string.IsNullOrWhiteSpace(Password)) {
				return null;
			}
			string hash;
			using (MD5 md5Hash = MD5.Create()) {
				byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Password));

				StringBuilder sBuilder = new StringBuilder();


				for (int i = 0; i < data.Length; i++) {
					sBuilder.Append(data[i].ToString("x2"));
				}
				hash = sBuilder.ToString();
			}
			return hash;
		}
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public sealed class PwdHasherv2
	{
		private static int saltLengthLimit = 32;
		public static byte[] GetSalt()
		{
			return GetSalt(saltLengthLimit);
		}
		private static byte[] GetSalt(int maximumSaltLength)
		{
			var salt = new byte[maximumSaltLength];
			using (var random = new RNGCryptoServiceProvider()) {
				random.GetNonZeroBytes(salt);
			}

			return salt;
		}

		public static string Hash(string value, byte[] salt)
		{
			return Hash(Encoding.UTF8.GetBytes(value), salt);
		}
		// https://www.youtube.com/watch?v=0dgTf9TUDHU
		public static string Hash(byte[] value, byte[] salt)
		{
			byte[] salted = value.Concat(salt).ToArray();

			byte[] data;

			using (SHA512Managed sha = new SHA512Managed()) {
				data = sha.ComputeHash(salted);
			}

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < data.Length; i++) {
				sb.Append(data[i].ToString("X2"));
			}

			return sb.ToString();
		}

		public static bool Confirm(string input, string hash)
		{
			byte[] hashB = Convert.FromBase64String(hash);
			int size = 64;
			byte[] saltB = new byte[hashB.Length - size];

			for (int i = 0; i < saltB.Length; i++) {
				saltB[i] = hashB[size + i];
			}

			string newhash = Hash(input, saltB);
			return (hash == newhash);

		}
	}
	*/

}
