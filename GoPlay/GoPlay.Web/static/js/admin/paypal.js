
var csrfToken = $("meta[name='csrf-token']").attr("content");
var cancelkey = function() {
	if(window.confirm('Are you sure?'))
	{
		$.ajax({
			method: 'post',
			url: '/admin/paypal/PreapprovalCancel/',
			headers: {
				'Content-type': 'application/json;',
				'X-CSRFToken': csrfToken
			},
			data: {
			}
		}).success(function (data) {
		    if ("error" in data) {
		        $("div.error").html(data['error']);
		    }
		    else if ("success" in data) {
		        location.href = location.href;
		    }
		}).error(function (data) {
			if ("error" in data) {
				$("div.error").html(data['error']);
			}
		});
	}
};

