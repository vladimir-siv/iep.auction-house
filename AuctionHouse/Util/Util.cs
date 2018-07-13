using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AuctionHouse
{
	public static class Util
	{
		public static string ToMD5(this string input)
		{
			var md5 = System.Security.Cryptography.MD5.Create();

			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			var sb = new System.Text.StringBuilder();

			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X"));
			}

			return sb.ToString().ToLower();
		}

		public static DateTime AsUnixTimestamp(this double timestamp)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return origin.AddSeconds(timestamp);
		}

		public static double ToUnixTimestamp(this DateTime date)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan diff = date.ToUniversalTime() - origin;
			return diff.TotalMilliseconds;
		}
	}

	public class DateTimeComparer : IComparer<DateTime>
	{
		public static DateTimeComparer Ascending { get; } = new DateTimeComparer(true);
		public static DateTimeComparer Descending { get; } = new DateTimeComparer(false);

		private bool isAscending;

		private DateTimeComparer(bool isAscending) { this.isAscending = isAscending; }

		public int Compare(DateTime x, DateTime y)
		{
			if (x == y) return 0;
			if (isAscending) return x > y ? 1 : -1;
			else return x < y ? 1 : -1;
		}
	}

	public class TransactionException : Exception
	{
		public TransactionException() { }
		public TransactionException(string message) : base(message) { }
		public TransactionException(string message, Exception innerException) : base(message, innerException) { }
		protected TransactionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}