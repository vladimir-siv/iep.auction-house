using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuctionHouse.Models;

namespace AuctionHouse.Controllers
{
	public class HomeController : Controller
	{
		private AuctionHouseDB db = new AuctionHouseDB();

		[HttpGet]
		public ActionResult Index()
		{
			ViewBag.NavIndex = 0;
			ViewBag.RecentAuctions = db.GetCurrentSystemParameters().RecentAuctions;
			return View(db.FindActiveAndCompletedAuctions());
		}

		[HttpGet]
		public ActionResult Auction()
		{
			if (Session["user"] == null) return HttpNotFound();

			ViewBag.NavIndex = 1;
			return View();
		}

		[HttpGet]
		public ActionResult About()
		{
			ViewBag.NavIndex = 2;
			return View();
		}

		[HttpGet]
		public ActionResult ViewProfile(string userid)
		{
			if (Session["user"] == null) return HttpNotFound();

			User user = null;

			if (Guid.TryParse(userid, out var id)) user = db.FindUserById(id);
			else user = db.FindUserById(((User)Session["user"]).ID);

			if (user == null) user = Models.User.Dummy;
			else if (user.ID == ((User)Session["user"]).ID) ViewBag.TokenOrders = db.FindUserTokenOrders(user);

			return View(user);
		}
	}
}