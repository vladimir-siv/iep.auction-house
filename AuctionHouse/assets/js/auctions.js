class AuctionPartialViewModel extends DynamicViewModel
{
	constructor(guid, title, timeleft, price, bidder)
	{
		super();
		
		this.guid = guid;
		this.title = title;
		this.timeleft = timeleft;
		this.price = price;
		this.bidder = bidder;
	}
	
	GetFormattedTime()
	{
		var seconds = this.timeleft;
		
		var hours = Math.floor(seconds / 3600);
		seconds = seconds % 3600;
		var minutes = Math.floor(seconds / 60);
		seconds = seconds % 60;
		
		return ("0" + hours).slice(-2) + ":" + ("0" + minutes).slice(-2) + ":" + ("0" + seconds).slice(-2)
	}
	
	Setup()
	{
		super.Setup();
		
		var holder = $("article#auction-" + this.guid);
		if (holder.length === 1) super.register(holder);
		else super.register(null);
		
		super.fetch("p", "timeleft");
		super.fetch("p", "price");
		super.fetch("p", "bidder");
		super.fetch("a", "action");
	}
	
	SetTimeleft(timeleft)
	{
		this.timeleft = timeleft;
		super.dynamics("p", "timeleft", this.GetFormattedTime());
	}
	
	SetPrice(price)
	{
		this.price = price;
		super.dynamics("p", "price", price + "t");
	}
	
	SetBidder(bidder)
	{
		this.bidder = bidder;
		super.dynamics("p", "bidder", bidder);
	}
	
	SetAction(action, enabled = true)
	{
		if (typeof enabled === "undefined") enabled = true;
		var component = super.dynamics("a", "action", action);
		if (component !== null)
		{
			if (enabled) component.removeClass("disabled");
			else component.addClass("disabled");
		}
	}
	
	AsView()
	{
		return super.AsView() +
			"<article id=\"auction-" + this.guid + "\" style=\"width: 20%;\" class=\"border-boxed d-inline-block solid-border border-sm border-gray margin-sm font-times-new-roman padding-bottom-sm\">" +
				"<div class=\"border-boxed expanded solid-border-bottom border-sm border-bottom-gray text-center\">" +
					"<h4>" + this.title + "</h4>" +
				"</div>" +
				"<img src=\"http://" + window.location.host + "/assets/storage/auctions/" + this.guid + "/0.png\" class=\"padding-top-sm padding-bottom-sm\" width=\"100%\" height=\"150\" />" +
				"<p data-dynamic=\"timeleft\" class=\"no-margin\" style=\"color: deepskyblue;\">" + this.GetFormattedTime() + "</p>" +
				"<p data-dynamic=\"price\" class=\"no-margin\" style=\"color: green\">" + this.price + "t</p>" +
				"<p data-dynamic=\"bidder\" class=\"no-margin\">" + this.bidder + "</p>" +
				"<a data-dynamic=\"action\" href=\"#\" class=\"btn btn-primary btn-sm\">Bid now</a>" +
			"</article>";
	}
}

var auctions;

doc.ready(function()
{
	if (typeof auctions === "undefined") auctions = [];
	setInterval(refreshTimings, 1000);
});

function refreshTimings()
{
	for (var i = 0; i < auctions.length; ++i)
	{
		if (auctions[i].timeleft === 1) auctions[i].SetAction("Bid now", false);
		if (auctions[i].timeleft > 0) auctions[i].SetTimeleft(auctions[i].timeleft - 1);
	}
}