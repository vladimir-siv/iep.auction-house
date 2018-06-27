using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(AuctionHouse.Startup))]
namespace AuctionHouse
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();
		}
	}
}