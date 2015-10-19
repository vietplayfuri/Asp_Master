$(document).ready(function () {

    //send feedback
    $('input[id="submit"]').click(function (event) {

        var form = $('form[id="send-feedback"]');
        if (!form.valid())
        {
            event.preventDefault();
            return;
        }

        var feedback = new Object();
        feedback.Name = $('#txtname').val();
        feedback.Email = $('#txtemail').val();
        feedback.Phone = $('#txtphone').val();
        feedback.Message = $('#tamessage').val();

        $.ajax({
            type: "POST",
            url: "/account/contact/send-feedback",
            data: JSON.stringify(feedback),
            contentType: "application/json",
            success: function (returnedData) {
                if (returnedData != null) {
                    alert(returnedData.message);
                }
            }
        });
    });

})