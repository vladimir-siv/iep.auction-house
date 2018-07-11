using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuctionHouse.Models;
using AuctionHouse.Hubs;

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

		[HttpPost]
		public void PaymentProcessed(string clientId, string status)
		{
			TokenOrder order = null;
			if (Guid.TryParse(clientId, out var id)) order = db.FindTokenOrderByGuid(id);
			if (order == null) return;

			if (order.Status != null) return;

			order.Status = status == "success";
			db.Entry(order).State = EntityState.Modified;

			var user = db.FindUserById(order.Buyer);
			decimal balance = -1;

			if (order.Status.Value)
			{
				user.Balance += order.Amount;
				balance = user.Balance;
				db.Entry(user).State = EntityState.Modified;
			}
			
			db.SaveChanges();

			AuctionHub.HubContext.Clients.All.onTokenOrderCompleted(order.Buyer.ToString(), order.ID.ToString(), balance, order.Status.Value);

			Mailer.SendMail(Settings.SMTPUsername, "Auction House", user.Email, user.FirstName + " " + user.LastName, "Auction House - Token Order", 
				"Dear " + user.FirstName + "," + Environment.NewLine +
				Environment.NewLine +
				"This e-mail has been sent to inform you that your token order" + Environment.NewLine +
				"has been processed and marked as [" + (order.Status.Value ? "COMPLETE" : "FAILED") + "]." + Environment.NewLine +
				Environment.NewLine +
				"Please, do not reply to this e-mail as you will not get any response." + Environment.NewLine +
				Environment.NewLine +
				"Kind regards," + Environment.NewLine +
				"Auction House"
			);
		}
	}
}