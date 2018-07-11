/* =================== [\POPUP DEFS] =================== */

class Popup
{
	constructor(id)
	{
		var body = $(document.body);
		this.id = id;
		body.append(this.ToString());
		this.domElement = $("#" + id);
	}
	Feed(popupFeed)
	{
		var vheader = this.domElement.find(".modal-header");
		var vbody = this.domElement.find(".modal-body");
		var vfooter = this.domElement.find(".modal-footer");
		var vinfo = this.domElement.find("#" + this.id + "-popup-info");
		
		vheader.empty();
		vbody.empty();
		vfooter.empty();
		vinfo.empty();
		
		vheader.append(popupFeed.Header());
		vbody.append(popupFeed.Body());
		vfooter.append(popupFeed.Footer());
		vinfo.append(popupFeed.Info());
	}
	Show()
	{
		this.domElement.modal("show");
	}
	Hide()
	{
		this.domElement.modal("hide");
	}
	Toggle()
	{
		this.domElement.modal("toggle");
	}
	ToString()
	{
		return "" +
			"<div id=\"" + this.id + "\" class=\"modal fade above-top-content-fixed\" role=\"dialog\">" +
				"<div class=\"modal-dialog\">" +
					"<div class=\"modal-content\">" +
						"<div class=\"modal-header\"></div>" +
						"<div class=\"modal-body\"></div>" +
						"<div class=\"modal-footer\"></div>" +
						"<div id=\"" + this.id + "-popup-info\" class=\"container-fluid\"></div>" +
					"</div>" +
				"</div>" +
			"</div>";
	}
}

class AlertPopup extends Popup
{
	constructor(id)
	{
		super(id);
	}
	Feed(popupFeed)
	{
		var vbody = this.domElement.find(".modal-dialog");
		vbody.empty();
		vbody.append(popupFeed.Body());
	}
	ToString()
	{
		return "" +
			"<div id=\"" + this.id + "\" class=\"modal fade above-top-content-fixed\" role=\"dialog\">" +
				"<div class=\"modal-dialog\"></div>" +
			"</div>";
	}
}

class PopupFeed
{
	constructor()
	{
		this.popups = [];
		this.current = -1;
	}
	Subscribe(popup)
	{
		this.popups.push(popup);
	}
	Unsubscribe(popup)
	{
		var index = this.popups.indexOf(popup);
		if (index > -1) this.popups.splice(index, 1);
	}
	Feed(i)
	{
		//for (var i = 0; i < this.popups.length; this.popups[i++].Feed(this)) ;
		if (i < 0 || this.popups.length <= i) return false;
		this.current = i;
		this.popups[i].Feed(this);
		return true;
	}
	Show(i)
	{
		if (this.Feed(i)) this.popups[i].Show();
	}
	Hide(i)
	{
		if (this.Feed(i)) this.popups[i].Hide();
	}
	Toggle(i)
	{
		if (this.Feed(i)) this.popups[i].Toggle();
	}
	Header() { throw new Error("Abstract method"); }
	Body() { throw new Error("Abstract method"); }
	Footer() { throw new Error("Abstract method"); }
	Info() { throw new Error("Abstract method"); }
}

class Alert
{
	static New(type, content, dismissable = true, dismiss="alert")
	{
		if (dismissable)
		{
			return "" +
				"<div class=\"alert alert-" + type + " alert-dismissible fade in\">" +
					"<a href=\"#\" class=\"close\" data-dismiss=\"" + dismiss + "\" aria-label=\"close\">&times;</a>" +
					content +
				"</div>";
		}
		else
		{
			return "" +
				"<div class=\"alert alert-" + type + " fade in\">" +
					content +
				"</div>";
		}
	}
}

/* =================== [/POPUP DEFS] =================== */


/* =================== [\POPUPS] =================== */

class RegisterPopupFeed extends PopupFeed
{
	constructor(callback)
	{
		super();
		this.callback = callback;
	}
	Header()
	{
		return "" +
			"<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"close\">&times;</button>" +
			"<h4 class=\"modal-title\"><span class=\"glyphicon glyphicon-check\"></span>&emsp;Register</h4>";
	}
	Body()
	{
		return "" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-font\"></i></span>" +
				"<input id=\"register-firstname\" type=\"text\" class=\"form-control\" name=\"register-firstname\" placeholder=\"First name (eg. Vladimir)\">" +
			"</div>" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-bold\"></i></span>" +
				"<input id=\"register-lastname\" type=\"text\" class=\"form-control\" name=\"register-lastname\" placeholder=\"Last name (eg. Sivčev)\">" +
			"</div>" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-envelope\"></i></span>" +
				"<input id=\"register-email\" type=\"email\" class=\"form-control\" name=\"register-email\" placeholder=\"E-mail (eg. vladimirsi@nordeus.com)\">" +
			"</div>" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-lock\"></i></span>" +
				"<input id=\"register-password\" type=\"password\" class=\"form-control\" name=\"register-password\" placeholder=\"Password (eg. @Th!nkBoutAStr0ngP4ss)\">" +
			"</div>";
	}
	Footer()
	{
		return "" +
			"<div class=\"row\">" +
				"<div class=\"col-lg-2 col-md-2 col-sm-2 col-xs-4\">" +
					"<button type=\"button\" class=\"btn btn-primary btn-sm btn-block\" onclick=\"" +
						this.callback +
						"(" +
							"'" + this.popups[this.current].id + "', " +
							"$('#" + this.popups[this.current].id + " #register-firstname')[0].value, " +
							"$('#" + this.popups[this.current].id + " #register-lastname')[0].value, " +
							"$('#" + this.popups[this.current].id + " #register-email')[0].value, " +
							"$('#" + this.popups[this.current].id + " #register-password')[0].value" +
						");" +
					"\">Sign up</button>" +
				"</div>" +
				"<div class=\"col-lg-8 col-md-8 col-sm-8 col-xs-4\"></div>" +
				"<div class=\"col-lg-2 col-md-2 col-sm-2 col-xs-4\">" +
					"<button type=\"button\" class=\"btn btn-danger btn-sm btn-block\" data-dismiss=\"modal\" aria-label=\"close\">Close</button>" +
				"</div>" +
			"</div>";
	}
	Info()
	{
		return "";
	}
}

class LoginPopupFeed extends PopupFeed
{
	constructor(callback)
	{
		super();
		this.callback = callback;
	}
	Header()
	{
		return "" +
			"<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"close\">&times;</button>" +
			"<h4 class=\"modal-title\"><span class=\"glyphicon glyphicon-log-in\"></span>&emsp;Log in</h4>";
	}
	Body()
	{
		return "" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-user\"></i></span>" +
				"<input id=\"login-email\" type=\"text\" class=\"form-control\" name=\"login-email\" placeholder=\"Email\">" +
			"</div>" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-lock\"></i></span>" +
				"<input id=\"login-password\" type=\"password\" class=\"form-control\" name=\"login-password\" placeholder=\"Password\">" +
			"</div>";
	}
	Footer()
	{
		return "" +
			"<div class=\"row\">" +
				"<div class=\"col-lg-2 col-md-2 col-sm-2 col-xs-4\">" +
					"<button type=\"button\" class=\"btn btn-primary btn-sm btn-block\" onclick=\"" +
						this.callback +
						"(" +
							"'" + this.popups[this.current].id + "', " +
							"$('#" + this.popups[this.current].id + " #login-email')[0].value, " +
							"$('#" + this.popups[this.current].id + " #login-password')[0].value" +
						");" +
					"\">Log in</button>" +
				"</div>" +
				"<div class=\"col-lg-8 col-md-8 col-sm-8 col-xs-4\"></div>" +
				"<div class=\"col-lg-2 col-md-2 col-sm-2 col-xs-4\">" +
					"<button type=\"button\" class=\"btn btn-danger btn-sm btn-block\" data-dismiss=\"modal\" aria-label=\"close\">Close</button>" +
				"</div>" +
			"</div>";
	}
	Info()
	{
		return "";
	}
}

class AccountInfoPopupFeed extends PopupFeed
{
	constructor(callback)
	{
		super();
		this.callback = callback;
	}
	Header()
	{
		return "" +
			"<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"close\">&times;</button>" +
			"<h4 class=\"modal-title\"><span class=\"glyphicon glyphicon-info-sign\"></span>&emsp;Account info</h4>";
	}
	Body()
	{
		return "" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-lock\"></i></span>" +
				"<input id=\"info-oldpassword\" type=\"password\" class=\"form-control\" name=\"info-oldpassword\" placeholder=\"Old Password (eg. @Th!nkBoutAStr0ngP4ss)\">" +
			"</div>" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-font\"></i></span>" +
				"<input id=\"info-firstname\" type=\"text\" class=\"form-control\" name=\"info-firstname\" placeholder=\"First name (eg. Vladimir)\">" +
			"</div>" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-bold\"></i></span>" +
				"<input id=\"info-lastname\" type=\"text\" class=\"form-control\" name=\"info-lastname\" placeholder=\"Last name (eg. Sivčev)\">" +
			"</div>" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-envelope\"></i></span>" +
				"<input id=\"info-email\" type=\"email\" class=\"form-control\" name=\"info-email\" placeholder=\"E-mail (eg. vladimirsi@nordeus.com)\">" +
			"</div>" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-lock\"></i></span>" +
				"<input id=\"info-password\" type=\"password\" class=\"form-control\" name=\"info-password\" placeholder=\"Password (eg. @Th!nkBoutAStr0ngP4ss)\">" +
			"</div>";
	}
	Footer()
	{
		return "" +
			"<div class=\"row\">" +
				"<div class=\"col-lg-2 col-md-2 col-sm-2 col-xs-4\">" +
					"<button type=\"button\" class=\"btn btn-primary btn-sm btn-block\" onclick=\"" +
						this.callback +
						"(" +
							"'" + this.popups[this.current].id + "', " +
							"$('#" + this.popups[this.current].id + " #info-oldpassword')[0].value, " +
							"$('#" + this.popups[this.current].id + " #info-firstname')[0].value, " +
							"$('#" + this.popups[this.current].id + " #info-lastname')[0].value, " +
							"$('#" + this.popups[this.current].id + " #info-email')[0].value, " +
							"$('#" + this.popups[this.current].id + " #info-password')[0].value" +
						");" +
					"\">Change</button>" +
				"</div>" +
				"<div class=\"col-lg-8 col-md-8 col-sm-8 col-xs-4\"></div>" +
				"<div class=\"col-lg-2 col-md-2 col-sm-2 col-xs-4\">" +
					"<button type=\"button\" class=\"btn btn-danger btn-sm btn-block\" data-dismiss=\"modal\" aria-label=\"close\">Close</button>" +
				"</div>" +
			"</div>";
	}
	Info()
	{
		return "";
	}
}

class OrderTokensPopupFeed extends PopupFeed
{
	constructor(callback)
	{
		super();
		this.callback = callback;
	}
	Header()
	{
		return "" +
			"<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"close\">&times;</button>" +
			"<h4 class=\"modal-title\"><span class=\"glyphicon glyphicon-certificate\"></span>&emsp;Order tokens</h4>";
	}
	Body()
	{
		return "" +
			"<div class=\"input-group\">" +
				"<span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-cog\"></i></span>" +
				"<div class=\"form-control\">" +
					"<input id=\"order-package\" type=\"radio\" name=\"order-package\" value=\"0\" checked> Silver&emsp;" +
					"<input id=\"order-package\" type=\"radio\" name=\"order-package\" value=\"1\"> Gold&emsp;" +
					"<input id=\"order-package\" type=\"radio\" name=\"order-package\" value=\"2\"> Platinum&emsp;" +
				"</div>" +
			"</div>";
	}
	Footer()
	{
		return "" +
			"<div class=\"row\">" +
				"<div class=\"col-lg-2 col-md-2 col-sm-2 col-xs-4\">" +
					"<button type=\"button\" class=\"btn btn-primary btn-sm btn-block\" onclick=\"" +
						this.callback +
						"(" +
							"'" + this.popups[this.current].id + "', " +
							"$('#" + this.popups[this.current].id + " #order-package:checked')[0].value" +
						");" +
					"\">Order</button>" +
				"</div>" +
				"<div class=\"col-lg-8 col-md-8 col-sm-8 col-xs-4\"></div>" +
				"<div class=\"col-lg-2 col-md-2 col-sm-2 col-xs-4\">" +
					"<button type=\"button\" class=\"btn btn-danger btn-sm btn-block\" data-dismiss=\"modal\" aria-label=\"close\">Close</button>" +
				"</div>" +
			"</div>";
	}
	Info()
	{
		return "";
	}
}


class AlertPopupFeed extends PopupFeed
{
	constructor(initialContent)
	{
		super();
		this.content = initialContent;
	}
	Body()
	{
		return this.content;
	}
}

/* =================== [/POPUPS] =================== */


/* =================== [\INIT] =================== */

var mainPopup;
var alertPopup;

var loginPopupFeed;
var registerPopupFeed;
var accountInfoPopupFeed;
var orderTokensPopupFeed;
var alertPopupFeed;

doc.ready(function()
{
	mainPopup = new Popup(Config.mainPopupId);
	alertPopup = new AlertPopup(Config.alertPopupId);
	
	loginPopupFeed = new LoginPopupFeed("login");
	loginPopupFeed.Subscribe(mainPopup);
	
	registerPopupFeed = new RegisterPopupFeed("register");
	registerPopupFeed.Subscribe(mainPopup);
	
	accountInfoPopupFeed = new AccountInfoPopupFeed("changeAccountInfo");
	accountInfoPopupFeed.Subscribe(mainPopup);

	orderTokensPopupFeed = new OrderTokensPopupFeed("orderTokens");
	orderTokensPopupFeed.Subscribe(mainPopup);

	alertPopupFeed = new AlertPopupFeed(Alert.New("success", "<b>Success!</b> The job is done.", true, "modal"));
	alertPopupFeed.Subscribe(alertPopup);
	
	$('[data-toggle="popover"]').popover();
});

/* =================== [/INIT] =================== */