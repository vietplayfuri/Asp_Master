// Foundation JavaScript
// Documentation can be found at: http://foundation.zurb.com/docs
$(document).foundation();

// LN: auto close flash messages after 3 seconds
window.setTimeout(function () {
    $(".alert-box a.close").trigger("click.fndtn.alert");
}, 3500);
