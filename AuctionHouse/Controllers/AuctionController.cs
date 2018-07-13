using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using AuctionHouse.Models;
using AuctionHouse.Hubs;
using log4net;

namespace AuctionHouse.Controllers
{
    public class AuctionController : Controller
    {
		private static readonly ILog log = LogManager.GetLogger("MainLog");
		private readonly AuctionHouseDB db = new AuctionHouseDB();

		[HttpGet]
		public ActionResult Show(string id)
		{
			try
			{
				if (!Guid.TryParse(id, out var guid)) return HttpNotFound();
				var auction = db.FindAuctionById(guid);
				if (auction == null) return HttpNotFound();

				if (auction.OpenedOn == null &&
					(Session["user"] == null || !(bool)Session["isAdmin"] && ((User)Session["user"]).ID != auction.Holder))
				{
					return HttpNotFound();
				}

				if (auction.OpenedOn != null && auction.CompletedOn == null)
				{
					var completedOn = auction.OpenedOn.Value.AddSeconds(auction.AuctionTime);
					if (DateTime.Now >= completedOn) auction.CompletedOn = completedOn;
				}

				var images = new List<string>(16);
				var path = "/assets/storage/auctions/" + auction.ID.ToString() + "/";
				foreach (var file in Directory.EnumerateFiles(Server.MapPath("~" + path)))
				{
					if (file.EndsWith(".png"))
					{
						images.Add(path + Path.GetFileName(file));
					}
				}

				ViewBag.ImageSources = images;

				var lastBid = auction.LastBid;
				if (lastBid != null)
				{
					ViewBag.Bidder = lastBid.User;
					ViewBag.CurrentPrice = lastBid.Amount;
				}
				else ViewBag.CurrentPrice = auction.StartingPrice;

				return View(auction);
			}
			catch (Exception ex)
			{
				log.Error(ex.Message, ex);
				return View("Error");
			}
		}
		
		[HttpPost]
		public string Create(string title, int time, decimal price)
		{
			if (Session["user"] == null) return string.Empty;

			try
			{
				var sysparams = db.GetCurrentSystemParameters();

				if (title == null || string.IsNullOrWhiteSpace(title)) return "#Error: Invalid title.";
				if (time <= 0) time = sysparams.DefaultAuctionTime;
				if (price < 0) return "#Error: Invalid price.";

				var uploadFailed = true;

				var guid = Guid.NewGuid();

				for (int i = 0; i < Request.Files.Count; ++i)
				{
					if (Request.Files[i].ContentType == "image/png")
					{
						Directory.CreateDirectory(Server.MapPath("~/assets/storage/auctions/" + guid.ToString() + "/"));
						Request.Files[i].SaveAs(Server.MapPath("~/assets/storage/auctions/" + guid.ToString() + "/" + i + ".png"));
						uploadFailed = false;
					}
				}

				if (uploadFailed) return "#Error: You must supply at least one image.";

				var user = (User)Session["user"];

				var auction = new Auction
				{
					ID = guid,
					Title = title,
					AuctionTime = time,
					CreatedOn = DateTime.Now,
					OpenedOn = null,
					CompletedOn = null,
					StartingPrice = price,
					Currency = sysparams.Currency,
					PriceRate = sysparams.PriceRate,
					Holder = user.ID
				};

				try
				{
					db.Auctions.Add(auction);
					db.SaveChanges();
				}
				catch { return "#Error: Could not create the auction. Some of the values are invalid."; }

				AuctionHub.HubContext.Clients.All.onAuctionCreated(auction.ID.ToString(), auction.Title, auction.AuctionTime, auction.StartingPrice, auction.CreatedOn.ToString(Settings.DateTimeFormat), user.FirstName + " " + user.LastName);
				return auction.ID.ToString();
			}
			catch (Exception ex)
			{
				log.Error(ex.Message, ex);
				return "#Error: Unknown error occured.";
			}
		}

		[HttpPost]
		public string Manage(string guid, bool approve)
		{
			if (Session["user"] == null || !(bool)Session["isAdmin"]) return string.Empty;

			if (string.IsNullOrWhiteSpace(guid) || !Guid.TryParse(guid, out var id))
			{
				return "#Error: Invalid auction id.";
			}

			using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				try
				{
					var auction = db.FindAuctionById(id);

					if (auction == null)
					{
						throw new TransactionException("Could not find auction with such id.");
					}

					if (auction.OpenedOn != null)
					{
						throw new TransactionException("Auction was already managed.");
					}

					auction.OpenedOn = DateTime.Now;
					if (!approve)
					{
						auction.CompletedOn = auction.OpenedOn;
					}

					db.Entry(auction).State = EntityState.Modified;
					
					db.SaveChanges();
					transaction.Commit();

					try { AuctionHub.HubContext.Clients.All.onAuctionManaged(auction.ID.ToString(), auction.Title, approve ? auction.AuctionTime : 0, auction.StartingPrice, string.Empty, "[No bidder]", "<b>" + auction.OpenedOn.Value.ToString(Settings.DateTimeFormat) + "</b>", auction.CompletedOn != null ? "<b>" + auction.CompletedOn.Value.ToString(Settings.DateTimeFormat) + "</b>" : "<b style=\"color: red;\">Not complete</b>"); }
					catch (Exception ex) { log.Error(ex); }

					return "Auction successfully managed.";
				}
				catch (TransactionException ex)
				{
					transaction.Rollback();
					return "#Error: " + ex.Message;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					log.Error(ex.Message, ex);
					return "#Error: Unknown error occured.";
				}
			}
		}

		[HttpPost]
		public string Bid(string guid, decimal amount)
		{
			var user = Session["user"] as User;
			if (user == null) return "#Error: Please, log in!";

			if (!Guid.TryParse(guid, out var id)) return "#Error: Invalid guid.";

			using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				try
				{
					var auction = db.FindAuctionById(id);
					if (auction == null) throw new TransactionException("Auction does not exist (to bid on such).");

					if (auction.OpenedOn == null) throw new TransactionException("Auction is not opened yet.");

					if (auction.CompletedOn != null || DateTime.Now >= auction.OpenedOn.Value.AddSeconds(auction.AuctionTime))
					{
						throw new TransactionException("Auctions is closed.");
					}

					if (auction.Holder == user.ID) throw new TransactionException("Cannot bid on owning auction.");

					var lastBid = auction.LastBid;
					if (lastBid != null)
					{
						if (amount <= lastBid.Amount) throw new TransactionException("Cannot bid with lower price than current.");
					}
					else
					{
						if (amount <= auction.StartingPrice) throw new TransactionException("Cannot bid with lower price than current.");
					}

					user = db.FindUserById(user.ID);
					if (user.Balance < amount) throw new TransactionException("Insufficient funds.");

					if (lastBid != null)
					{
						lastBid.User.Balance += lastBid.Amount;
						db.Entry(lastBid.User).State = EntityState.Modified;
					}

					user.Balance -= amount;
					db.Entry(user).State = EntityState.Modified;

					var bid = new Bid
					{
						ID = Guid.NewGuid(),
						Bidder = user.ID,
						Auction = auction.ID,
						BidOn = DateTime.Now,
						Amount = amount
					};

					db.Bids.Add(bid);
					db.SaveChanges();
					transaction.Commit();

					try { AuctionHub.HubContext.Clients.All.onBid(auction.ID.ToString(), user.ID.ToString(), user.FirstName + " " + user.LastName, bid.BidOn.ToString(Settings.DateTimeFormat), amount); }
					catch (Exception ex) { log.Error(ex); }

					return "Bidding successful.";
				}
				catch (TransactionException ex)
				{
					transaction.Rollback();
					return "#Error: " + ex.Message;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					log.Error(ex.Message, ex);
					return "#Error: Unknown error occured.";
				}
			}
		}

		[HttpPost]
		public string Claim(string guid)
		{
			var user = Session["user"] as User;
			if (user == null) return string.Empty;

			using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				try
				{
					Auction auction = null;
					if (Guid.TryParse(guid, out var id)) auction = db.FindAuctionById(id);

					if (auction == null) throw new TransactionException("Invalid auction.");

					if (auction.Holder != user.ID) throw new TransactionException("Can't claim auction prize.");

					if (auction.OpenedOn == null) throw new TransactionException("Auction is not opened.");

					var now = DateTime.Now;

					if (now < auction.OpenedOn.Value.AddSeconds(auction.AuctionTime)) throw new TransactionException("Auction is not finished yet.");

					if (auction.CompletedOn != null) throw new TransactionException("Auction is completed, no prize left to claim.");

					auction.CompletedOn = now;
					db.Entry(auction).State = EntityState.Modified;

					var lastBid = auction.LastBid;

					if (lastBid != null)
					{
						user = db.FindUserById(user.ID);
						user.Balance += lastBid.Amount;
						db.Entry(user).State = EntityState.Modified;
					}

					db.SaveChanges();
					transaction.Commit();

					return "Successfully claimed auction prize. Please, check your balance.";
				}
				catch (TransactionException ex)
				{
					transaction.Rollback();
					return "#Error: " + ex.Message;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					log.Error(ex.Message, ex);
					return "#Error: Unknown error occured.";
				}
			}
		}
	}
}