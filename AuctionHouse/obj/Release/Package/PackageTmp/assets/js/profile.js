var dynamic_fields = {};

doc.ready(function()
{
	var dynamic_data = $("[data-dynamic]");

	for (var i = 0; i < dynamic_data.length; ++i)
	{
		var dynamic = $(dynamic_data[i]);
		dynamic_fields[dynamic.attr("data-dynamic")] = dynamic;
	}
});

function addOrder(id, amount, currency, pricerate)
{
	dynamic_fields["orders"].prepend(orderView(id, amount, currency, pricerate));
}

function updateOrderStatus(id, status)
{
	dynamic_fields["orders"].find("tr#" + id + " td:nth-child(4)").html
	(
		status ? "<b style=\"color: forestgreen;\">Completed</b>" : "<b style=\"color: red;\">Failed</b>"
	);
}

function orderView(id, amount, currency, pricerate)
{
	return "" +
		"<tr id=\"" + id + "\">" +
			"<td><b>" + amount + " t</b></td>" +
			"<td><b>" + currency + "</b></td>" +
			"<td><b>" + pricerate + "</b></td>" +
			"<td><b style=\"color: deepskyblue;\">Pending</b></td>" +
		"</tr>";
}