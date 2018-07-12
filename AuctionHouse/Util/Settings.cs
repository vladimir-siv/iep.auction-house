using System;
using System.Collections.Generic;

namespace AuctionHouse
{
	public static class Settings
	{
		public const string SMTPHost = "smtp.gmail.com";
		public const int SMTPPort = 587;
		public const string SMTPUsername = "si3iep.auctionhouse@gmail.com";
		public const string SMTPPassword = "#vladimir96sivcev@";

		public const string DateTimeFormat = "dd.MM.yyyy. HH':'mm':'ss";
		public const string DecimalFormat = "N4";

		public static T Best<T>(this ICollection<T> collection, Func<T, T, T> comparer)
		{
			T best = default(T);
			bool first = true;

			foreach (var obj in collection)
			{
				best = first ? obj : comparer(best, obj);
				first = false;
			}

			return best;
		}
	}
}