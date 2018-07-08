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
	}
}

function refreshTime(time)
{
	dynamic_fields["timeleft"].html("<b>" + time_format(time) + "</b>");
}