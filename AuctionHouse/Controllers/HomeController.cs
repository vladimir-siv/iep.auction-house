using System;
using System.Web.Mvc;
using AuctionHouse.Models;
using log4net;

namespace AuctionHouse.Controllers
{
	public class HomeController : Controller
	{
		private static readonly ILog log = LogManager.GetLogger("MainLog");

		[HttpGet]
		public ActionResult Index()
		{
			using (var db = new AuctionHouseDB())
			{
				try
				{
					ViewBag.NavIndex = 0;
					ViewBag.RecentAuctions = db.GetCurrentSystemParameters().RecentAuctions;
					return View(db.FindActiveAndCompletedAuctions(true));
				}
				catch (Exception ex)
				{
					log.Error(ex.Message, ex);
					return View("Error");
				}
			}
		}

		[HttpGet]
		public ActionResult Auction()
		{
			try
			{
				if (Session["user"] == null) return HttpNotFound();
				ViewBag.NavIndex = 1;
				return View();
			}
			catch (Exception ex)
			{
				log.Error(ex.Message, ex);
				return View("Error");
			}
		}

		[HttpGet]
		public ActionResult About()
		{
			try
			{
				ViewBag.NavIndex = 2;
				return View();
			}
			catch (Exception ex)
			{
				log.Error(ex.Message, ex);
				return View("Error");
			}
		}

		[HttpGet]
		public ActionResult ViewProfile(string id)
		{
			using (var db = new AuctionHouseDB())
			{
				try
				{
					if (Session["user"] == null) return HttpNotFound();

					User user = null;

					if (Guid.TryParse(id, out var userid)) user = db.FindUserById(userid);
					else user = db.FindUserById(((User)Session["user"]).ID);

					if (user == null) user = Models.User.Dummy;
					else if (user.ID == ((User)Session["user"]).ID) ViewBag.TokenOrders = db.FindUserTokenOrders(user);

					return View(user);
				}
				catch (Exception ex)
				{
					log.Error(ex.Message, ex);
					return View("Error");
				}
			}
		}
	}
}