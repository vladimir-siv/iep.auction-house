class AuctionPartialViewModel extends DynamicViewModel
{
	constructor(guid, title, timeleft, price, bidderguid, biddername)
	{
		super();
		
		this.guid = guid;
		this.title = title;
		this.timeleft = timeleft < 0 ? 0 : timeleft;
		this.price = price;
		this.bidderguid = bidderguid;
		this.biddername = biddername;
	}
	
	GetFormattedTime()
	{
		return time_format(this.timeleft);
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
	
	SetBidder(bidderguid, biddername)
	{
		this.bidderguid = bidderguid;
		this.biddername = biddername;
		super.dynamics("p", "bidder", biddername);
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
				"<div class=\"border-boxed expanded solid-border-bottom border-bottom-sm border-bottom-gray text-center\">" +
					"<h4><a class=\"hover-text-decor-none\" href=\"/Auction/Show?id=" + this.guid + "\" target=\"_blank\">" + this.title + "</a></h4>" +
				"</div>" +
				"<img src=\"http://" + window.location.host + "/assets/storage/auctions/" + this.guid + "/0.png\" class=\"padding-sm\" width=\"100%\" height=\"150\" />" +
				"<p data-dynamic=\"timeleft\" class=\"no-margin\" style=\"color: deepskyblue;\">" + this.GetFormattedTime() + "</p>" +
				"<p data-dynamic=\"price\" class=\"no-margin\" style=\"color: green\">" + this.price + "t</p>" +
				"<p data-dynamic=\"bidder\" class=\"no-margin\">" + this.biddername + "</p>" +
				"<a data-dynamic=\"action\" class=\"btn btn-primary btn-sm" + (this.timeleft > 0 ? "" : " disabled") + "\" href=\"/Auction/Show?id=" + this.guid + "\" target=\"_blank\">Bid now</a>" +
			"</article>";
	}
}

class AuctionApprovalViewModel extends ViewModel
{
	constructor(guid, title, timeleft, price, createdon, holder)
	{
		super();

		this.guid = guid;
		this.title = title;
		this.timeleft = timeleft < 0 ? 0 : timeleft;
		this.price = price;
		this.createdon = createdon;
		this.holder = holder;
	}

	GetFormattedTime()
	{
		return time_format(this.timeleft);
	}

	AsView()
	{
		return super.AsView() +
			"<article id=\"auction-" + this.guid + "\" style=\"width: 20%;\" class=\"border-boxed d-inline-block solid-border border-sm border-gray margin-sm font-times-new-roman padding-bottom-sm\">" +
				"<div class=\"border-boxed expanded solid-border-bottom border-bottom-sm border-bottom-gray text-center\">" +
					"<h4><a class=\"hover-text-decor-none\" href=\"/Auction/Show?id=" + this.guid + "\" target=\"_blank\">" + this.title + "</a></h4>" +
				"</div>" +
				"<img src=\"http://" + window.location.host + "/assets/storage/auctions/" + this.guid + "/0.png\" class=\"padding-sm\" width=\"100%\" height=\"150\" />" +
				"<p class=\"no-margin\" style=\"color: deepskyblue;\">" + this.GetFormattedTime() + "</p>" +
				"<p class=\"no-margin\" style=\"color: green\">" + this.price + "t</p>" +
				"<p class=\"no-margin\">" + this.createdon + "</p>" +
				"<p class=\"no-margin\">" + this.holder + "</p>" +
				"<button type=\"button\" class=\"btn btn-success btn-sm\" onclick=\"manageAuction('" + this.guid + "', true);\">Approve</button>&emsp;" +
				"<button type=\"button\" class=\"btn btn-danger btn-sm\" onclick=\"manageAuction('" + this.guid + "', false);\">Reject</button>" +
			"</article>";
	}
}

/* =================== [\INIT] =================== */

var auctions = []; // Array of DynamicViewModels to display, server should feed this array
var dynamics = true; // Should timer refresh the time of auctions each second, server may feed this value
var userguid = "?"; // Guid of the current user, server should feed this value (or leave blank if nobody is logged in)

var filter = new XFilter();

doc.ready(function()
{
	filter.reg(titleFilter);
	filter.reg(priceFilter);
	filter.reg(stateFilter);
	filter.reg(usersFilter);

	auctions.reverse();

	var filtered = [];
	for (var i = auctions.length - 1; i >= 0; --i)
	{
		if (dynamics) auctions[i].DynamicPropChanged.reg(auctionPropChanged);

		if (filter.check(auctions[i]))
		{
			filtered.push(auctions[i]);
		}
	}

	setPaginationArticles(filtered);

	if (dynamics) setInterval(refreshTimings, 1000);
});

function refreshTimings()
{
	for (var i = auctions.length - 1; i >= 0; --i)
	{
		if (auctions[i].timeleft === 1) auctions[i].SetAction("Bid now", false);
		if (auctions[i].timeleft > 0) auctions[i].SetTimeleft(auctions[i].timeleft - 1);
	}

	onFilteringNeeded();
}

/* =================== [/INIT] =================== */


/* =================== [\EVENT HANDLING] =================== */

function auctionPropChanged(auction, property)
{
	if (property.name === "price")
	{
		onFilteringNeeded();
	}
}

function onFilteringNeeded()
{
	var filtered = applyFilter();
	setPaginationArticles(filtered);
}

/* =================== [/EVENT HANDLING] =================== */


/* =================== [\LOGIC FUNCTIONS] =================== */

function findAuctionByGuid(guid)
{
	var auction = null;
	for (var i = 0; auction === null && i < auctions.length; ++i)
	{
		if (auctions[i].guid === guid)
		{
			auction = auctions[i];
		}
	}

	return auction;
}

function appendAuction(auction)
{
	if (auction === null) return;

	auctions.push(auction);
	if (dynamics) auction.DynamicPropChanged.reg(auctionPropChanged);

	onFilteringNeeded();
}

function removeAuction(auction)
{
	if (auction === null) return;

	var index = auctions.indexOf(auction);
	if (index >= 0)
	{
		if (dynamics) auction.DynamicPropChanged.unreg(auctionPropChanged);
		auctions.splice(index, 1);

		onFilteringNeeded();
	}
}

function applyFilter()
{
	var filtered = [];
	for (var i = auctions.length - 1; i >= 0; --i)
	{
		if (filter.check(auctions[i]))
		{
			filtered.push(auctions[i]);
		}
	}

	return filtered;
}

/* =================== [/LOGIC FUNCTIONS] =================== */


/* =================== [\FILTERS] =================== */

var titles = [];
function titleFilter(auction)
{
	if (titles.length === 0) return true;

	for (var i = 0; i < titles.length; ++i)
	{
		if (auction.title.toLowerCase().indexOf(titles[i].toLowerCase()) !== -1)
		{
			return true;
		}
	}

	return false;
}

var minprice = -1, maxprice = -1;
function priceFilter(auction)
{
	if (minprice >= 0 && auction.price < minprice) return false;
	if (maxprice >= 0 && auction.price > maxprice) return false;
	return true;
}

var opened = true, completed = true;
function stateFilter(auction)
{
	if (opened && auction.timeleft > 0) return true;
	if (completed && auction.timeleft === 0) return true;
	return false;
}

var usersfilter = false;
function usersFilter(auction)
{
	return !usersfilter || auction.bidderguid === userguid;
}

function setFilters(newtitles, newminprice, newmaxprice, newopened, newcompleted, newusersfilter)
{
	titles = newtitles;
	minprice = newminprice;
	maxprice = newmaxprice;
	opened = newopened;
	completed = newcompleted;
	usersfilter = newusersfilter;
	onFilteringNeeded();
}
function removeFilters()
{
	titles = [];
	minprice = -1;
	maxprice = -1;
	opened = true;
	completed = true;
	usersfilter = false;
	onFilteringNeeded();
}

/* =================== [/FILTERS] =================== */