function register(popupid, firstname, lastname, email, password)
{
	$("#" + popupid + "-popup-info").html("");
	
	var dataValid = true;
	
	if (firstname == "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter first name!"));
		dataValid = false;
	}
	if (lastname == "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter last name!"));
		dataValid = false;
	}
	if (email == "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter e-mail!"));
		dataValid = false;
	}
	if (password == "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter password!"));
		dataValid = false;
	}
	
	if (!dataValid) return;
	
	$.ajax
	({
		url: "http://" + window.location.host + "/Account/Register",
		method: "POST",
        contentType: "application/json; charset=utf-8",
		data: JSON.stringify({ FirstName : firstname, LastName : lastname, Email : email, Password : password }),
		dataType: "html",
		success: function(response)
		{
			if (response.startsWith("#Error: "))
			{
				$("#" + popupid + "-popup-info").append(Alert.New("danger", response.substring(8), true));
				return;
			}
			
			$("#" + popupid + "-popup-info").append(Alert.New("success", response, true));
			window.location.reload();
		}
	});
}

function login(popupid, email, password)
{
	$("#" + popupid + "-popup-info").html("");
	
	var dataValid = true;
	
	if (email == "")
	{
		$("#" + popupid + "-popup-info").append(Alert.New("danger", "<b>Error.</b> You must enter e-mail!"));
		dataValid = false;
	}
	if (password == "")
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
		dataType: "html"
	})
	.done(function(response) 
	{
		if (response.startsWith("#Error: "))
		{
			$("#" + popupid + "-popup-info").append(Alert.New("danger", response.substring(8), true));
			return;
		}
		
		$("#" + popupid + "-popup-info").append(Alert.New("success", response, true));
		window.location.reload();
	});
}

function logout()
{
	$.ajax
	({
		url: "http://" + window.location.host + "/Account/Logout",
		method: "POST",
		dataType: "html"
	})
	.done(function(response)
	{
		window.location.reload();
	});
}