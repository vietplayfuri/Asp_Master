'use strict';

/**
 * @ngdoc overview
 * @name gtokenApp
 * @description
 * # gtokenApp
 *
 * Main module of the application.
 */

var gtokenApp = angular.module('gtokenApp', ["ngAnimate", "angucomplete-alt", 'mrPageEnterAnimate', 'ngFileUpload']);
gtokenApp.controller('LoginController', function ($scope) {
    $scope.login = function () {
        if ($scope.loginForm.$invalid) {
            return false;
        }
    };
})
.controller('TransactionController', function ($scope) {
    $('input[type=datetime]').fdatetimepicker();
    //$("#queryForm").submit(function (event) {
    //    $("form #model_time_zone").val(moment().zone());
    //    return true;
    //})
    $scope.changPaging = function (event) {
        //$("form #model_time_zone").val(moment().zone());
        var formData = $("#queryForm").serialize();
        window.location = "/referral/index?model.page=" + $(event.target).data("id") + "&" + formData;
    }
})
.controller('ProfileController', function ($scope) {
    var client = new ZeroClipboard(document.getElementById('copy-button'));

    client.on("ready", function (readyEvent) {
        client.on("aftercopy", function (e) {

            console.log("Copied text to clipboard: " + e.data["text/plain"]);
        });
    });

    $scope.copyToClipboard = function (event) {
        $scope.copied = true;

        $(event.target).addClass('active');

        setTimeout(function () {
            $(event.target).removeClass('active');
            $scope.copy = false;
        }, 3000);
    };
})
.controller('ResetPasswordController', function ($scope) {

    $scope.resetPassword = function () {
        if ($scope.resetPasswordForm.$invalid) {
            return false;
        }

        $scope.resetPasswordForm.isSubmitted = true;

    };
})
.controller('LocaleSelectController', function ($scope) {
    $scope.locale = 'en';

    $scope.updateLocale = function () {
        $("#update-locale-form input[name='locale']").val($scope.locale);
        $("#update-locale-form").submit();
    };
})
.directive("compareTo", function () {
    return {
        require: "ngModel",
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, element, attributes, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue == scope.otherModelValue;
            };

            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
});

gtokenApp.config(function ($interpolateProvider) {
    $interpolateProvider.startSymbol('{$');
    $interpolateProvider.endSymbol('$}');
});

$(document).foundation({
    orbit: {
        slide_number: false,
        bullets: false
    }
});

$(document).ready(function () {
    $('.orbit-container').on('click', '.orbit-nav', function () {
        var $container = $(this).closest('.orbit-container');
        var bullets = $container.find('.orbit-bullets').children();
        var arrows = $container.find('.orbit-nav');
        var isNext = $(this).hasClass('next') ? true : false;

        if (isNext) {
            $container.find('.orbit-next').trigger('click');
        } else {
            $container.find('.orbit-prev').trigger('click');
        }

        // var isFirst = bullets.filter('.active').index() == 0;
        // var isLast = bullets.filter('.active').index() === bullets.length - 1;

        // if (isFirst) {
        //     arrows.filter('.prev').addClass('disabled');
        // } else {
        //     arrows.filter('.prev').removeClass('disabled');
        // }

        // if (isLast) {
        //     arrows.filter('.next').addClass('disabled');
        // } else {
        //     arrows.filter('.next').removeClass('disabled');
        // }
        return false;
    });

    // $('.orbit-container').on('click', '.orbit-nav.next', function() {
    //     var $container = $(this).closest('.orbit-container');
    //     var bullets = $container.find('.orbit-bullets').children();

    //     $container.find('.orbit-next').trigger('click');

    //     var isLast = bullets.filter('.active').index() === bullets.length - 1;

    //     if (isLast) {
    //         $(this).addClass('disabled');
    //     } else {
    //         $(this).removeClass('disabled');
    //     }

    //     return false;
    // });
});

