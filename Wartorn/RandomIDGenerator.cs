using System;
using System.Text;

namespace Wartorn.Utility {
	public static class RandomIdGenerator {
		private static char[] _base62chars =
			"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
			.ToCharArray();

		private static Random _random = new Random();

		/// <summary>
		/// Set default length of ID
		/// </summary>
		public static int DefaultIDLength { get; set; } = 10;

		/// <summary>
		/// Get a Base62 string ID
		/// </summary>
		/// <returns></returns>
		public static string GetBase62() {
			return GetBase62(DefaultIDLength);
		}

		/// <summary>
		/// Get a Base62 string ID with given <paramref name="length"/>
		/// </summary>
		/// <param name="length">the length of the string ID</param>
		/// <returns>a Base62 string ID</returns>
		public static string GetBase62(int length) {
			var sb = new StringBuilder(length);

			for (int i = 0; i < length; i++)
				sb.Append(_base62chars[_random.Next(62)]);

			return sb.ToString();
		}

		/// <summary>
		/// Get a Base36 string ID
		/// </summary>
		/// <returns></returns>
		public static string GetBase36() {
			return GetBase36(DefaultIDLength);
		}

		/// <summary>
		/// Get a Base36 string ID with given <paramref name="length"/>
		/// </summary>
		/// <param name="length">the length of the string ID</param>
		/// <returns>a Base36 string ID</returns>
		public static string GetBase36(int length) {
			var sb = new StringBuilder(length);

			for (int i = 0; i < length; i++)
				sb.Append(_base62chars[_random.Next(36)]);

			return sb.ToString();
		}
	}
}
