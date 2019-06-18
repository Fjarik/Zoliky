using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace SharedLibrary
{
	public sealed class EncrypterDecrypter
	{
		public static string Encrypt(string content, string key)
		{
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider()
			{
				Key = Encoding.UTF8.GetBytes(key),
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			};

			ICryptoTransform trans = des.CreateEncryptor();

			byte[] bytes = Encoding.UTF8.GetBytes(content);
			byte[] result = trans.TransformFinalBlock(bytes, 0, bytes.Length);

			des.Clear();

			return Convert.ToBase64String(result, 0, result.Length);
		}

		public static string Decrypt(string content, string key)
		{

			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider()
			{
				Key = Encoding.UTF8.GetBytes(key),
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			};

			ICryptoTransform trans = des.CreateDecryptor();

			byte[] bytes = Convert.FromBase64String(content);
			byte[] result = trans.TransformFinalBlock(bytes, 0, bytes.Length);

			des.Clear();

			return Encoding.UTF8.GetString(result);
		}

	}
}
