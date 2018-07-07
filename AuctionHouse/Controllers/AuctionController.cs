using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using AuctionHouse.Models;

namespace AuctionHouse.Controllers
{
    public class AuctionController : Controller
    {
		private AuctionHouseDB db = new AuctionHouseDB();

		[HttpGet]
		public ActionResult Show(string id)
		{
			return View();
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
	}
}