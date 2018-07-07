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
	}
}