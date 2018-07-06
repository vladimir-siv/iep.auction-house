namespace AuctionHouse.Models
{
	using System;
	using System.Data.Entity;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;

	public partial class AuctionHouseDB : DbContext
	{
		private string MD5(string input)
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

		public AuctionHouseDB()
			: base("name=AuctionHouseDB")
		{
		}

		public virtual DbSet<Administrator> Administrators { get; set; }
		public virtual DbSet<Auction> Auctions { get; set; }
		public virtual DbSet<Bid> Bids { get; set; }
		public virtual DbSet<SystemParameter> SystemParameters { get; set; }
		public virtual DbSet<TokenOrder> TokenOrders { get; set; }
		public virtual DbSet<User> Users { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Auction>()
				.Property(e => e.Title)
				.IsUnicode(false);

			modelBuilder.Entity<Auction>()
				.Property(e => e.StartingPrice)
				.HasPrecision(24, 12);

			modelBuilder.Entity<Auction>()
				.Property(e => e.Currency)
				.IsUnicode(false);

			modelBuilder.Entity<Auction>()
				.Property(e => e.PriceRate)
				.HasPrecision(24, 12);

			modelBuilder.Entity<Auction>()
				.HasMany(e => e.Bids)
				.WithRequired(e => e.Auction1)
				.HasForeignKey(e => e.Auction);

			modelBuilder.Entity<Bid>()
				.Property(e => e.Amount)
				.HasPrecision(24, 12);

			modelBuilder.Entity<SystemParameter>()
				.Property(e => e.SilverPackage)
				.HasPrecision(24, 12);

			modelBuilder.Entity<SystemParameter>()
				.Property(e => e.GoldPackage)
				.HasPrecision(24, 12);

			modelBuilder.Entity<SystemParameter>()
				.Property(e => e.PlatinumPackage)
				.HasPrecision(24, 12);

			modelBuilder.Entity<SystemParameter>()
				.Property(e => e.Currency)
				.IsUnicode(false);

			modelBuilder.Entity<SystemParameter>()
				.Property(e => e.PriceRate)
				.HasPrecision(24, 12);

			modelBuilder.Entity<TokenOrder>()
				.Property(e => e.Amount)
				.HasPrecision(24, 12);

			modelBuilder.Entity<TokenOrder>()
				.Property(e => e.Currency)
				.IsUnicode(false);

			modelBuilder.Entity<TokenOrder>()
				.Property(e => e.PriceRate)
				.HasPrecision(24, 12);

			modelBuilder.Entity<User>()
				.Property(e => e.FirstName)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.LastName)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Email)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Password)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Balance)
				.HasPrecision(24, 12);

			modelBuilder.Entity<User>()
				.HasOptional(e => e.Administrator)
				.WithRequired(e => e.User)
				.WillCascadeOnDelete();

			modelBuilder.Entity<User>()
				.HasMany(e => e.Bids)
				.WithRequired(e => e.User)
				.HasForeignKey(e => e.Bidder);

			modelBuilder.Entity<User>()
				.HasMany(e => e.TokenOrders)
				.WithRequired(e => e.User)
				.HasForeignKey(e => e.Buyer);
		}

		public void EncryptUserPassword(User user)
		{
			user.Password = MD5(user.Password);
		}

		public User FindUserByEmailAndPassword(string email, string password, out bool isAdmin)
		{
			password = MD5(password);

			var query =
				from user in Users
				where user.Email == email && user.Password == password
				select user;
			
			var result = query.SingleOrDefault();

			if (result != null)
			{
				var adminQuery =
					from admin in Administrators
					where admin.ID == result.ID
					select admin;

				isAdmin = adminQuery.SingleOrDefault() != null;
			}
			else isAdmin = false;

			return result;
		}
	}
}
