using System;
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
    }
}