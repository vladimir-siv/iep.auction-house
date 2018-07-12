using System;
using System.Data.Entity;
using System.Web.Http;
using AuctionHouse.Models;
using AuctionHouse.Hubs;

namespace AuctionHouse.Controllers
{
	public class PaymentController : ApiController
	{
		private AuctionHouseDB db = new AuctionHouseDB();

		// POST api/<controller>
		public void Post(string clientId, string status)
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