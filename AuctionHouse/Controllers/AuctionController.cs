using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using AuctionHouse.Models;
using System.Data.Entity;

using System.Linq;

namespace AuctionHouse.Controllers
{
    public class AuctionController : Controller
    {
		private AuctionHouseDB db = new AuctionHouseDB();

		[HttpGet]
		public ActionResult Show(string id)
		{
			if (!Guid.TryParse(id, out var guid)) return HttpNotFound();
			var auction = db.FindAuctionById(guid);
			if (auction == null) return HttpNotFound();

			if (auction.OpenedOn == null &&
				(!(bool)Session["isAdmin"] && ((User)Session["user"]).ID != auction.Holder))
			{
				return HttpNotFound();
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
		
		[HttpPost]
		public string Create(string title, int time, decimal price)
		{
			if (Session["user"] == null) return string.Empty;

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
				Holder = ((User)Session["user"]).ID
			};

			try
			{
				db.Auctions.Add(auction);
				db.SaveChanges();
			}
			catch { return "#Error: Could not create the auction. Some of the values are invalid."; }

			return auction.ID.ToString();
		}

		[HttpPost]
		public string Manage(string guid, bool approve)
		{
			if (Session["user"] == null || !(bool)Session["isAdmin"]) return "";

			if (string.IsNullOrWhiteSpace(guid) || !Guid.TryParse(guid, out var id))
			{
				return "#Error: Invalid auction id.";
			}

			var auction = db.FindAuctionById(id);

			if (auction == null)
			{
				return "#Error: Could not find auction with such id.";
			}

			if (auction.OpenedOn != null)
			{
				return "#Error: Auction was already managed.";
			}

			auction.OpenedOn = DateTime.Now;
			if (!approve)
			{
				auction.CompletedOn = auction.OpenedOn;
			}

			db.Entry(auction).State = EntityState.Modified;

			try { db.SaveChanges(); }
			catch { return "#Error: Could not manage auction."; }

			return "Auction successfully managed.";
		}

		[HttpPost]
		public string Bid(string guid, decimal amount)
		{
			var user = Session["user"] as User;
			if (user == null) return "#Error: Please, log in!";

			if (!Guid.TryParse(guid, out var id)) return "#Error: Invalid guid.";
			var auction = db.FindAuctionById(id);
			if (auction == null) return "#Error: Auction does not exist (to bid on such).";

			if (auction.OpenedOn == null) return "#Error: Auction is not opened yet.";

			if (auction.CompletedOn != null || DateTime.Now >= auction.OpenedOn.Value.AddSeconds(auction.AuctionTime))
			{
				return "#Error: Auctions is closed.";
			}

			if (auction.Holder == user.ID) return "#Error: Cannot bid on owning auction.";

			var lastBid = auction.LastBid;
			if (lastBid != null)
			{
				if (amount <= lastBid.Amount) return "#Error: Cannot bid with lower price than current.";
			}
			else
			{
				if (amount <= auction.StartingPrice) return "#Error: Cannot bid with lower price than current.";
			}

			if (user.Balance < amount) return "#Error: Insufficient funds.";

			if (lastBid != null)
			{
				lastBid.User.Balance += lastBid.Amount;
				db.Entry(lastBid.User).State = EntityState.Modified;
			}

			user = db.FindUserById(user.ID);
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

			try
			{
				db.Bids.Add(bid);
				db.SaveChanges();
			}
			catch { return "#Error: Unable to register bid."; }

			return "Bidding successful.";
		}
	}
}