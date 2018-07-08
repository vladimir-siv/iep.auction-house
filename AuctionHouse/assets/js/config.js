var doc = $(document);
var win = $(window);

class Config { constructor() { throw new Error("Static class"); } }
Config.mainPopupId = "popup";
Config.alertPopupId = "alert-popup";

function time_format(seconds)
{
	var hours = Math.floor(seconds / 3600);
	seconds = seconds % 3600;
	var minutes = Math.floor(seconds / 60);
	seconds = seconds % 60;

	return ("0" + hours).slice(-2) + ":" + ("0" + minutes).slice(-2) + ":" + ("0" + seconds).slice(-2);
}