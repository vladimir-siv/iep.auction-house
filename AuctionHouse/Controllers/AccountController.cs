using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuctionHouse.Models;

namespace AuctionHouse.Controllers
{
    public class AccountController : Controller
    {
		private AuctionHouseDB db = new AuctionHouseDB();

		[HttpPost]
		public string Register([Bind(Include = "FirstName,LastName,Email,Password")] User user)
		{
			try
			{
				if (ModelState.IsValid)
				{
					user.ID = Guid.NewGuid();
					db.EncryptUserPassword(user);

					db.Users.Add(user);
					db.SaveChanges();

					return "Successfully registered!";
				}
				else
				{
					foreach (var state in ModelState.Values)
					{
						foreach (var error in state.Errors)
						{
							return "#Error: " + error.ErrorMessage;
						}
					}

					return "#Error: Unknown error.";
				}
			}
			catch { return "#Error: Could not register. Email already in use."; }
		}

		[HttpPost]
		public string Login(string email, string password)
		{
			if (Session["user"] != null) return "#Error: Could not log in.";
			
			User user = db.FindUserByEmailAndPassword(email, password, out var isAdmin);

			if (user == null) return "#Error: Invalid email/password.";

			Session["user"] = user;
			Session["isAdmin"] = isAdmin;

			return "Successfully logged in.";
		}
		
		public ActionResult Logout()
		{
			if (Session["user"] != null) Session.Clear();
			return Redirect("/Home/Index");
		}
    }
}