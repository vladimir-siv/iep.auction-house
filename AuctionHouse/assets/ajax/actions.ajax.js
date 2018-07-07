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

/* =================== [/ACTIONS] =================== */