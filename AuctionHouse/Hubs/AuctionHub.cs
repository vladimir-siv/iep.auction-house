using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace AuctionHouse.Hubs
{
	[HubName("AuctionHub")]
	public class AuctionHub : Hub
	{
		public static IHubContext HubContext { get; } = GlobalHost.ConnectionManager.GetHubContext<AuctionHub>();
	}
}