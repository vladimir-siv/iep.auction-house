using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuctionHouse.Models;

namespace AuctionHouse.Controllers
{
    public class AdminController : Controller
	{
		private AuctionHouseDB db = new AuctionHouseDB();

		[HttpGet]
        public ActionResult Index()
        {
			if (Session["user"] == null || !(bool)Session["isAdmin"]) return HttpNotFound();
			ViewBag.NavIndex = 3;
			ViewBag.RecentAuctions = db.GetCurrentSystemParameters().RecentAuctions;
			return View(db.FindReadyAuctions());
        }

		[HttpGet]
		public ActionResult System()
		{
			if (Session["user"] == null || !(bool)Session["isAdmin"]) return HttpNotFound();
			ViewBag.NavIndex = 4;
			return View(db.GetCurrentSystemParameters());
		}

		[HttpPost]
		public string ChangeSystem([Bind(Include = "RecentAuctions,DefaultAuctionTime,SilverPackage,GoldPackage,PlatinumPackage,Currency,PriceRate")] SystemParameters parameters)
		{
			if (!ModelState.IsValid) return "#Error: One or more parameters are not valid.";

			var current = db.GetCurrentSystemParameters();

			current.RecentAuctions = parameters.RecentAuctions;
			current.DefaultAuctionTime = parameters.DefaultAuctionTime;
			current.SilverPackage = parameters.SilverPackage;
			current.GoldPackage = parameters.GoldPackage;
			current.PlatinumPackage = parameters.PlatinumPackage;
			current.Currency = parameters.Currency;
			current.PriceRate = parameters.PriceRate;

			try
			{
				db.Entry(current).State = EntityState.Modified;
				db.SaveChanges();

				

				return "Successfully changed parameters!";
			}
			catch { return "#Error: Could change parameters."; }
		}
	}
}