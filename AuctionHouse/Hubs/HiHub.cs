using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace AuctionHouse.Hubs
{
	[HubName("HiHub")]
	public class HiHub : Hub
	{
		[HubMethodName("Hello")]
		public void Hello()
		{
			Clients.All.hello();
		}
	}
}