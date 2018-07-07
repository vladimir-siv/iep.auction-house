using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuctionHouse.Models;

namespace AuctionHouse.Controllers
{
    public class AuctionController : Controller
    {
		private AuctionHouseDB db = new AuctionHouseDB();

		[HttpPost]
		public string Create(string title, int time, decimal price)
		{
			if (Session["user"] == null) return string.Empty;

			var sysparams = db.GetCurrentSystemParameters();

			if (title == null || string.IsNullOrWhiteSpace(title)) return "#Error: Invalid title.";
			if (time <= 0) time = sysparams.DefaultAuctionTime;
			if (price < 0) return "#Error: Invalid price.";

			Auction auction = new Auction
			{
				ID = Guid.NewGuid(),
				Title = title,
				AuctionTime = time,
				CreatedOn = DateTime.Now,
				OpenedOn = null,
				CompletedOn = null,
				StartingPrice = price,
				Currency = sysparams.Currency,
				PriceRate = sysparams.PriceRate
			};

			try
			{
				db.Auctions.Add(auction);
				db.SaveChanges();
			}
			catch { return "#Error: Could not create the auction. Some of the values are invalid."; }

			return "Auction successfully created.";
		}
	}
}