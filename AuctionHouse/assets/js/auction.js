var timeleft = 0;
var timeout = true;

var dynamic_fields = {};

doc.ready(function()
{
	var dynamic_data = $("[data-dynamic]");

	for (var i = 0; i < dynamic_data.length; ++i)
	{
		var dynamic = $(dynamic_data[i]);
		dynamic_fields[dynamic.attr("data-dynamic")] = dynamic;
	}
	
	refreshTime(timeleft < 0 ? 0 : timeleft);
	if (timeout) setInterval(timer, 1000);
});

function timer()
{
	if (timeleft > 0)
	{
		refreshTime(--timeleft);
		if (timeleft === 0) dynamic_fields["completed-on"].html("<b>" + datetime_format(new Date(Date.now())) + "</b>");
	}
}

function refreshTime(time)
{
	dynamic_fields["timeleft"].html("<b>" + time_format(time) + "</b>");
}

function addBidder(bidderguid, biddername, bidon, amount)
{
	dynamic_fields["bids"].prepend(bidderView(bidderguid, biddername, bidon, amount));
}

function bidderView(bidderguid, biddername, bidon, amount)
{
	return "" +
		"<tr>" +
		"<td><b><a class=\"hover-text-decor-none\" href=\"/Home/ViewProfile?id=" + bidderguid + "\">" + biddername + "</a></b></td>" +
			"<td><b>" + bidon + "</b></td>" +
			"<td><b>" + amount + " t </b></td>" +
		"</tr>";
}