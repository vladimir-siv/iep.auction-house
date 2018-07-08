using System;
using System.Web.Mvc;
using System.IO;
using AuctionHouse.Models;
using System.Data.Entity;

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
			
			Auction auction = new Auction
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
	}
}