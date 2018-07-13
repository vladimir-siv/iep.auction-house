using System;
using System.Data.Entity;
using System.Text;
using System.Web.Mvc;
using AuctionHouse.Models;
using AuctionHouse.Hubs;
using log4net;

namespace AuctionHouse.Controllers
{
	public class AccountController : Controller
	{
		private static readonly ILog log = LogManager.GetLogger("MainLog");

		[HttpPost]
		public string Register([Bind(Include = "FirstName,LastName,Email,Password")] User user)
		{
			if (!ModelState.IsValid)
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

			using (var db = new AuctionHouseDB())
			{
				try
				{
					user.ID = Guid.NewGuid();
					user.Password = user.Password.ToMD5();

					db.Users.Add(user);
					db.SaveChanges();

					return "Successfully registered!";
				}
				catch (Exception ex)
				{
					log.Warn(ex.Message, ex);
					return "#Error: Could not register. Email already in use.";
				}
			}
		}

		[HttpPost]
		public string Login(string email, string password)
		{
			if (Session["user"] != null) return "#Error: Could not log in.";

			using (var db = new AuctionHouseDB())
			{
				try
				{
					User user = db.FindUserByEmailAndPassword(email, password, out var isAdmin);

					if (user == null) return "#Error: Invalid email/password.";

					Session["user"] = user;
					Session["isAdmin"] = isAdmin;

					return "Successfully logged in.";
				}
				catch (Exception ex)
				{
					log.Error(ex.Message, ex);
					return "#Error: Unknown error occured.";
				}
			}
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

			using (var db = new AuctionHouseDB())
			{
				try
				{
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
						user.Password = password;
						sb.Append("Password,");
					}

					sb[sb.Length - 1] = ']';

					if (!ModelState.IsValid)
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

					user.Password = user.Password.ToMD5();
					db.Entry(user).State = EntityState.Modified;

					try { db.SaveChanges(); }
					catch { return "#Error: One or more fields are not in a correct format (eg. invalid email)."; }

					Session["user"] = user;
					return sb.ToString();
				}
				catch (Exception ex)
				{
					log.Error(ex.Message, ex);
					return "#Error: Unknown error occured.";
				}
			}
		}

		[HttpPost]
		public string OrderTokens(int package)
		{
			if (Session["user"] == null) return string.Empty;

			using (var db = new AuctionHouseDB())
			{
				try
				{
					var parameters = db.GetCurrentSystemParameters();

					decimal amount = 0;

					switch (package)
					{
						case 0: amount = parameters.SilverPackage; break;
						case 1: amount = parameters.GoldPackage; break;
						case 2: amount = parameters.PlatinumPackage; break;
						default: return "#Error: Such package does not exist.";
					}

					var order = new TokenOrder
					{
						ID = Guid.NewGuid(),
						Buyer = ((User)Session["user"]).ID,
						Amount = amount,
						Currency = parameters.Currency,
						PriceRate = parameters.PriceRate,
						Status = null
					};

					try
					{
						db.TokenOrders.Add(order);
						db.SaveChanges();
					}
					catch (Exception ex)
					{
						log.Error(ex.Message, ex);
						return "#Error: Could not initiate order. Please, try again.";
					}

					AuctionHub.HubContext.Clients.All.onTokenOrderCreated(order.Buyer.ToString(), order.ID.ToString(), order.Amount.ToString(Settings.DecimalFormat), order.Currency, order.PriceRate.ToString(Settings.DecimalFormat));

					return "<a id=\"c-mobile-payment-widget\" href=\"https://stage.centili.com/payment/widget?apikey=b23180535003ba668fe3d1d2876ad928&reference=" + order.ID + "&country=rs&package=" + package + "\" target=\"_blank\"><img src=\"https://www.centili.com/images/centili-widget-button.png\"/></a>";
				}
				catch (Exception ex)
				{
					log.Error(ex.Message, ex);
					return "#Error: Unknown error occured.";
				}
			}
		}
	}
}