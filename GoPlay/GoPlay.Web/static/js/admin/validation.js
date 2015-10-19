$(document).ready(function () {
    if ($('form').find(".field-validation-error")) {
        var form = $(this);
        $('label').removeClass('error');
        $('.field-validation-error')
            .each(function () {
                var label = $(this).parent().siblings().find("label");
                $(label).addClass('error');
            });
    }
});