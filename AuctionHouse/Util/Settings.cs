using System;
using System.Collections.Generic;

namespace AuctionHouse
{
	public static class Settings
	{
		public const string DateTimeFormat = "dd.MM.yyyy. HH:mm:ss";
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