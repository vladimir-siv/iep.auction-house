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

function datetime_format(datetime)
{
	var dd = datetime.getDate();
	var MM = datetime.getMonth() + 1;
	var yyyy = datetime.getFullYear();

	var HH = datetime.getHours();
	var mm = datetime.getMinutes();
	var ss = datetime.getSeconds();

	return (dd > 9 ? dd : '0' + dd) + "." + (MM > 9 ? MM : '0' + MM) + "." + yyyy + " " + (HH > 9 ? HH : '0' + HH) + ":" + (mm > 9 ? mm : '0' + mm) + ":" + (ss > 9 ? ss : '0' + ss);
}