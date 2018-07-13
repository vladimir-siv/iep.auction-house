namespace AuctionHouse.Models
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;

	public partial class AuctionHouseDB : DbContext
	{
		public AuctionHouseDB()
			: base("name=AuctionHouseDB")
		{
		}

		public virtual DbSet<Administrator> Administrators { get; set; }
		public virtual DbSet<Auction> Auctions { get; set; }
		public virtual DbSet<Bid> Bids { get; set; }
		public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
		public virtual DbSet<SystemParameters> SystemParameters { get; set; }
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
				.HasForeignKey(e => e.Auction)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Bid>()
				.Property(e => e.Amount)
				.HasPrecision(24, 12);

			modelBuilder.Entity<SystemParameters>()
				.Property(e => e.SilverPackage)
				.HasPrecision(24, 12);

			modelBuilder.Entity<SystemParameters>()
				.Property(e => e.GoldPackage)
				.HasPrecision(24, 12);

			modelBuilder.Entity<SystemParameters>()
				.Property(e => e.PlatinumPackage)
				.HasPrecision(24, 12);

			modelBuilder.Entity<SystemParameters>()
				.Property(e => e.Currency)
				.IsUnicode(false);

			modelBuilder.Entity<SystemParameters>()
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
				.WithRequired(e => e.User);

			modelBuilder.Entity<User>()
				.HasMany(e => e.Auctions)
				.WithRequired(e => e.User)
				.HasForeignKey(e => e.Holder)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<User>()
				.HasMany(e => e.Bids)
				.WithRequired(e => e.User)
				.HasForeignKey(e => e.Bidder)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<User>()
				.HasMany(e => e.TokenOrders)
				.WithRequired(e => e.User)
				.HasForeignKey(e => e.Buyer)
				.WillCascadeOnDelete(false);
		}

		public SystemParameters GetCurrentSystemParameters()
		{
			var query =
				from parameters in SystemParameters
				select parameters;

			return query.SingleOrDefault();
		}

		public User FindUserById(Guid guid)
		{
			var query =
				from user in Users
				where user.ID == guid
				select user;

			return query.SingleOrDefault();
		}

		public User FindUserByEmail(string email)
		{
			var query =
				from user in Users
				where user.Email == email
				select user;

			return query.SingleOrDefault();
		}

		public User FindUserByEmailAndPassword(string email, string password, out bool isAdmin)
		{
			password = password.ToMD5();

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

		public Auction FindAuctionById(Guid guid)
		{
			var query =
				from auction in Auctions
				where auction.ID == guid
				select auction;

			return query.SingleOrDefault();
		}

		public IEnumerable<Auction> FindReadyAuctions()
		{
			var query =
				from auction in Auctions
				where auction.OpenedOn == null
				orderby auction.CreatedOn descending
				select auction;

			return query.ToList();
		}

		public IEnumerable<Auction> FindActiveAndCompletedAuctions(bool loadLastBids = false)
		{
			var query =
				from auction in Auctions
				where auction.OpenedOn != null
				orderby auction.OpenedOn descending
				select auction;

			if (loadLastBids)
			{
				var results = query.ToList();

				foreach (var auction in results)
				{
					// Eager load last bid & user
					var lastBid = auction.LastBid;
					if (lastBid != null)
					{
						var user = lastBid.User;
					}
				}

				return results;
			}
			else return query.ToList();
		}

		public TokenOrder FindTokenOrderByGuid(Guid guid)
		{
			var query =
				from order in TokenOrders
				where order.ID == guid
				select order;

			return query.SingleOrDefault();
		}

		public IEnumerable<TokenOrder> FindUserTokenOrders(User user)
		{
			var query =
				from order in TokenOrders
				where order.Buyer == user.ID
				select order;

			return query.ToList();
		}
	}
}
