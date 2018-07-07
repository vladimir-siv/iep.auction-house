using System;
using System.Data.Entity;
using System.Text;
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
					user.Password = user.Password.ToMD5();

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

		[HttpGet]
		public ActionResult Logout()
		{
			if (Session["user"] != null) Session.Clear();
			return Redirect("/Home/Index");
		}

		[HttpPost]
		public string ChangeInfo(string oldpassword, string firstname, string lastname, string email, string password)
		{
			if (Session["user"] == null) return string.Empty;

			if (string.IsNullOrWhiteSpace(oldpassword)) return "#Error: You must supply your old password!";

			User user = (User)Session["user"];
			if (user.Password != oldpassword.ToMD5()) return "#Error: Old password does not match your current one.";

			user = db.FindUserById(user.ID);

			StringBuilder sb = new StringBuilder("Success: [");

			if (!string.IsNullOrWhiteSpace(firstname))
			{
				user.FirstName = firstname;
				sb.Append("First Name,");
			}

			if (!string.IsNullOrWhiteSpace(lastname))
			{
				user.LastName = lastname;
				sb.Append("Last Name,");
			}

			if (!string.IsNullOrWhiteSpace(email) && db.FindUserByEmail(email) == null)
			{
				user.Email = email;
				sb.Append("Email,");
			}

			if (!string.IsNullOrWhiteSpace(password))
			{
				user.Password = password.ToMD5();
				sb.Append("Password,");
			}

			sb[sb.Length - 1] = ']';
			
			db.Entry(user).State = EntityState.Modified;
			db.SaveChanges();

			Session["user"] = user;
			return sb.ToString();
		}
	}
}