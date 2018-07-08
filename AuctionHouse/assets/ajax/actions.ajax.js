/* =================== [\RESPONSE HANDLERS] =================== */

function stdResponsePopupHandler(popupid, response, reload = true)
{
	if (response.startsWith("#Error: "))
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", response.substring(8), true));
		return;
	}
	
	$("#" + popupid + "-popup-info").append(Alert.New("success", response, true));
	
	if (reload) window.location.reload();
}

function stdResponseAlertHandler(response, location = null)
{
	if (response.startsWith("#Error: "))
	{
		var error = new AlertPopupFeed(Alert.New("danger", response.substring(8), true, "modal"));
		error.Subscribe(alertPopup);
		error.Show(0);
	}
	else
	{
		var type = "success";
		
		if (response.startsWith("#Warning: "))
		{
			type = "warning";
			response = response.substring(10);
		}
		
		var message = new AlertPopupFeed(Alert.New(type, response, true, "modal"));
		message.Subscribe(alertPopup);
		message.Show(0);

		if (location !== null)
		{
			if (location === "") window.location.reload();
			else window.location.replace(location);
		}
	}
}

/* =================== [/RESPONSE HANDLERS] =================== */


/* =================== [\ACTIONS] =================== */

function register(popupid, firstname, lastname, email, password)
{
	$("#" + popupid + "-popup-info").html("");
	
	var dataValid = true;
	
	if (firstname === "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter first name!"));
		dataValid = false;
	}
	if (lastname === "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter last name!"));
		dataValid = false;
	}
	if (email === "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter e-mail!"));
		dataValid = false;
	}
	if (password === "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter password!"));
		dataValid = false;
	}
	
	if (!dataValid) return;
	
	$.ajax
	({
		url: "http://" + window.location.host + "/Account/Register",
		method: "POST",
		data: { FirstName : firstname, LastName : lastname, Email : email, Password : password },
		dataType: "text",
		success: function(response) { stdResponsePopupHandler(popupid, response); }
	});
}

function login(popupid, email, password)
{
	$("#" + popupid + "-popup-info").html("");
	
	var dataValid = true;
	
	if (email === "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter e-mail!"));
		dataValid = false;
	}
	if (password === "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter password!"));
		dataValid = false;
	}
	
	if (!dataValid) return;
	
	$.ajax
	({
		url: "http://" + window.location.host + "/Account/Login",
		method: "POST",
		data: { Email : email, Password : password },
		dataType: "text",
		success: function(response) { stdResponsePopupHandler(popupid, response); }
	});
}

function logout()
{
	$.ajax
	({
		url: "http://" + window.location.host + "/Account/Logout",
		method: "GET",
		success: function(response) { window.location.reload(); }
	});
}

function changeAccountInfo(popupid, oldpassword, firstname, lastname, email, password)
{
	$("#" + popupid + "-popup-info").html("");
	
	var dataValid = true;
	
	if (oldpassword === "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter your old password!"));
		dataValid = false;
	}
	
	if (!dataValid) return;
	
	$.ajax
	({
		url: "http://" + window.location.host + "/Account/ChangeInfo",
		method: "POST",
		data: { oldpassword : oldpassword, firstname : firstname, lastname : lastname, email : email, password : password },
		dataType: "text",
		success: function(response) { stdResponsePopupHandler(popupid, response); }
	});
}

function createAuction(title, time, price, files)
{
	var invalidData = "&emsp;<b>Error:</b>";

	files = files.prop("files");
	
	if (title === "")
	{
		invalidData += "<br/>You must enter title!";
	}
	if (time === "")
	{
		invalidData += "<br/>You must enter time!";
	}
	if (price === "")
	{
		invalidData += "<br/>You must enter price!";
	}
	if (files.length === 0)
	{
		invalidData += "<br/>You must supply at least one picture!";
	}
	
	if (invalidData !== "&emsp;<b>Error:</b>")
	{
		var error = new AlertPopupFeed(Alert.New("danger", invalidData));
		error.Subscribe(alertPopup);
		error.Show(0);
		return;
	}
	
	var formData = new FormData();
	
	formData.append("title", title);
	formData.append("time", time);
	formData.append("price", price);
	
	$.each(files, function(index, file) { formData.append('+file-' + index, file, file.name); });

	$.ajax
	({
		url: "http://" + window.location.host + "/Auction/Create",
		method: "POST",
		data: formData,
		processData: false,
		contentType: false,
		dataType: "text",
		success: function(response) { stdResponseAlertHandler(response, "http://" + window.location.host + "/Auction/Show?id=" + response); }
	});
}

function manageAuction(guid, approve)
{
	$.ajax
	({
		url: "http://" + window.location.host + "/Auction/Manage",
		method: "POST",
		data: { guid : guid, approve : approve },
		dataType: "text",
		success: function(response) { stdResponseAlertHandler(response, ""); }
	});
}

/* =================== [/ACTIONS] =================== */