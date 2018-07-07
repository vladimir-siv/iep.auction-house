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
			ViewBag.navIndex = 0;
			return View();
		}

		[HttpGet]
		public ActionResult Auction()
		{
			if (Session["user"] == null) return HttpNotFound();

			ViewBag.navIndex = 1;
			return View();
		}

		[HttpGet]
		public ActionResult About()
		{
			ViewBag.navIndex = 2;
			return View();
		}

		[HttpGet]
		public ActionResult ViewProfile(string userid)
		{
			if (Session["user"] == null) return HttpNotFound();

			User user = null;
			if (Guid.TryParse(userid, out var id)) user = db.FindUserById(id);
			else user = Session["user"] as User;
			if (user == null) user = Models.User.Dummy;
			return View(user);
		}
	}
}