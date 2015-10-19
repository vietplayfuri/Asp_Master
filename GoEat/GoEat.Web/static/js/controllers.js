var gdineApp = angular.module('gdineApp', ["angucomplete-alt"]);

gdineApp.directive('focus', function ($timeout) {
    return {
        scope: {
            trigger: '@focus'
        },
        link: function (scope, element) {
            scope.$watch('trigger', function (value) {
                if (value === "true") {
                    $timeout(function () {
                        element[0].focus();
                    });
                }
            });
        }
    };
});

gdineApp.directive('selectInputText', function ($timeout, $parse) {
    return {
        restrict: 'A', // Only matches attribute name
        link: function (scope, element, attrs) {
            var model = $parse(attrs.selectInputText);
            scope.$watch(model, function (value) {
                if (value === true) {
                    $timeout(function () {
                        element[0].selectionStart = 0;
                        element[0].selectionEnd = element[0].value.length;
                    });
                }
            });
        }
    };
});

gdineApp.directive('keepValueTo', function ($timeout) {
    /*
    This directive keeps the value of input as is and prevents changes
     */
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var keptValue = attrs.keepValueTo;
            element.val(keptValue);
            element.bind('propertychange keyup paste cut blur', function (blurEvent) {
                $timeout(function () {
                    if (element.val != keptValue) {
                        element.val(keptValue);
                    }
                });
            });
        }
    };
});

gdineApp.config(function ($locationProvider) {
    $locationProvider.html5Mode(true);
});

function TransferCtrl($scope, $http) {
    var csrfToken = $("meta[name='csrf-token']").attr("content");


    // Refactor to one general error handler for all
    var handleTransferFormError = function (errorsData) {
        for (var field in errorsData) {
            if (errorsData.hasOwnProperty(field)) {
                var $inputField = $("[name='" + field + "']");
                var $errorsUl = $inputField.next(".errors-container").find("ul.errors");
                $errorsUl.find(".error").remove();
                $errorsUl.append("<li class='error' data-error-message='" + errorsData[field] + "'>*"
                    + errorsData[field] + "</li>");
            }
        }
    };

    var toggleProgressIndicator = function (state) {
        if (state) {
            $(".transfer-panel .progress-indict").removeClass("hide");
            $(".transfer-panel .panel-filter").removeClass("hide");
        } else {
            $(".transfer-panel .progress-indict").addClass("hide");
            $(".transfer-panel .panel-filter").addClass("hide");
        }
    };

    $scope.submitTransferForm = function () {
        if ($scope.transferFormValid()) {
            toggleProgressIndicator(true);
            $http({
                method: 'post',
                url: '/friend/transfer',
                headers: {
                    'Content-type': 'application/json;',
                    'X-CSRFToken': csrfToken
                },
                data: {
                    receiverId: $scope.receiver.originalObject.id,
                    goplayTokenAmount: $scope.goplayTokenAmount.toString()
                }
            }).success(function (data) {
                if ("correct" in data) {
                    location.href = location.href;
                }
            }).error(function (data) {
                toggleProgressIndicator(false);
                if ("errors" in data) {
                    handleTransferFormError(data["errors"]);
                }
            });
        }
    };

    $scope.transferFormValid = function () {
        return ($scope.receiver
            && $scope.goplayTokenAmount >= 0.01
            && $("ul.errors li.error").length == 0);
    };

    $scope.checkFormData = function () {
        var $goplayTokenInput = $("#goplay-token-amount-input");

        $http({
            method: 'post',
            url: '/friend/transfer',
            headers: {
                'Content-type': 'application/json;',
                'X-CSRFToken': csrfToken
            },
            data: {
                receiverId: $scope.receiver.originalObject.id,
                goplayTokenAmount: $goplayTokenInput.val(),
                forValidate: true
            }
        }).success(function (data) {
            if ("correct" in data && data.correct) {
                $("ul.errors").find(".error").remove();
            }
        }).error(function (data) {
            if ("errors" in data) {
                handleTransferFormError(data["errors"]);
            }
        });
    };

    $scope.customerAccountSearchFormat = function (str) {
        return {
            term: encodeURIComponent(str)
        };
    };
}

function TransactionCtrl($scope, $http, $location) {
    var csrfToken = $("meta[name='csrf-token']").attr("content");


    $scope.init = function () {
        $scope.topUpMethod = topUpMethod;
        $scope.customPlayTokenQuantity = '';
        $scope.gcoinSelect = 1;
        $scope.scrolled = false;
        $scope.useGToken = false;
        $scope.items = [
                         { id: 1, name: 'foo' },
                         { id: 2, name: 'bar' },
                         { id: 3, name: 'blah' }
        ];

    };

    $scope.changeLocationSearchPart = function (key, value) {
        var searchObject = $location.search();

        if (searchObject[key] != value) {
            if (key === "game-id") {
                $location.search({});
            }
            $location.search(key, value);
        }
    };

    $scope.topUp = function ($event) {
        toggleProgressIndicator(true);
        var $this = $($event.target);
        $this.closest('li').find('form.gtoken-package-form').submit();
    };

    $scope.showTopUpOptsContainer = function () {
        return $scope.topUpMethod == 'paypal' || $scope.topUpMethod == 'creditCard';
    };

    // Refactor to one general error handler for all
    var handleTopupCardFormError = function (errorsData) {
        for (var field in errorsData) {
            var $form = $("#topup_card");
            var $errorsUl = $form.find(".errors-container").find("ul.errors");
            $errorsUl.find(".error").remove();
            $errorsUl.append("<li class='error' data-error-message='" + errorsData[field] + "'>*" + errorsData[field] + "</li>");
        }
    };

    var toggleProgressIndicator = function (state) {
        if (state) {
            $(".top-up-panel .progress-indict").removeClass("hide");
            $(".top-up-panel .panel-filter").removeClass("hide");
        } else {
            $(".top-up-panel .progress-indict").addClass("hide");
            $(".top-up-panel .notification-panel").removeClass("hide");
        }
    };

    $scope.submitTopupCardForm = function () {
        if ($scope.cardNumber && $scope.cardPassword) {
            toggleProgressIndicator(true);

            $http({
                method: 'post',
                url: '/transaction/topup-card',
                headers: {
                    'Content-type': 'application/json;',
                    'X-CSRFToken': csrfToken
                },
                data: {
                    cardNumber: $scope.cardNumber,
                    cardPassword: $scope.cardPassword
                }
            }).success(function (data) {
                if ("success" in data) {
                    //location.href = location.href;                  
                    $('p#messageArea').html(data['message']);
                    toggleProgressIndicator(false);
                }
            }).error(function (data) {
                toggleProgressIndicator(false);
                if ("errors" in data) {
                    handleTopupCardFormError(data["errors"]);
                }
                $(".top-up-panel .panel-filter").addClass("hide");
                $(".top-up-panel .notification-panel").addClass("hide");
            });
        }
    };

    $scope.submitCustomPlayTokenForm = function () {
        if ($scope.customPlayTokenFormValid()) {
            toggleProgressIndicator(true);

            var $customPlayTokenForm = $("form#custom-gtoken-package");
            var action = $customPlayTokenForm.data().endpoint;
            $customPlayTokenForm
                .attr("action", action)
                .submit();
        }
    };

    $scope.customPlayTokenFormValid = function () {
        return 10 <= $scope.customPlayTokenQuantity && $scope.customPlayTokenQuantity <= 4000
            && $scope.customPlayTokenQuantity % 10 == 0;
    };
    /*
    $scope.checkGToken = function($event) {
        var checkbox = $event.target;
        $scope.useGToken = checkbox.checked;
    }
    */
    $scope.changeTransactionState = function (event) {
        var $stateLink = $(event.target);
        $location.url($stateLink.data().pushstateUrl);
    };

    $scope.changeTopupTransactionState = function () {
        $scope.topUpMethod = $scope.topUpMethodItem;
        //if($scope.topUpMethod == 'upoint')
        //$location.url('/topup/upoint/');else
        if ($scope.topUpMethod == 'paypal')
            $location.url('/topup/paypal/');
        else if ($scope.topUpMethod == 'topUpCard')
            $location.url('/topup/topup-card/');
    };

    var handleConvertGCoinFormError = function (errorsData) {
        for (var field in errorsData) {
            var $form = $("#convertPaypal");
            var $errorsUl = $form.find(".errors-container").find("ul.errors");
            $errorsUl.find(".error").remove();
            $errorsUl.append("<li class='error' data-error-message='" + errorsData[field] + "'>*" + errorsData[field] + "</li>");
        }

    };

    var toggleProgressConvertIndicator = function (state) {
        if (state) {
            $(".gcoin-panel .progress-indict").removeClass("hide");
            $(".gcoin-panel .panel-filter").removeClass("hide");
        } else {
            $(".gcoin-panel .progress-indict").addClass("hide");
            $(".gcoin-panel .panel-filter").addClass("hide");
        }
    };

    var clearErrorMessage = function () {
        var $form = $("#convertPaypal");
        var $errorsUl = $form.find(".errors-container").find("ul.errors");
        $errorsUl.find(".error").remove();
    };

    $scope.submitConvert = function () {
        clearErrorMessage();
        toggleProgressLoginIndicator(false);
        if ($scope.gcoinSelect && $scope.paypalEmail) {
            toggleProgressConvertIndicator(true);

            $http({
                method: 'post',
                url: '/transaction/convert-gcoin',
                headers: {
                    'Content-type': 'application/json;',
                    'X-CSRFToken': csrfToken
                },
                data: {
                    gcoin: $scope.gcoinSelect,
                    paypalEmail: $scope.paypalEmail,
                    password: $scope.password
                }
            }).success(function (data) {
                toggleProgressLoginIndicator(false);
                if ("success" in data) {
                    $('#messageArea').html("You converted " + $scope.gcoinSelect + " GCoin to USD");
                    toggleProgressIndicator(false);
                }
                else {
                    toggleProgressConvertIndicator(false);
                    if ("errors" in data) {
                        handleConvertGCoinFormError(data["errors"]);
                    }
                }
            }).error(function (data) {
                toggleProgressConvertIndicator(false);
                toggleProgressLoginIndicator(false);
                if ("errors" in data) {
                    handleConvertGCoinFormError(data["errors"]);
                }
            });
        }

    };
    var toggleProgressLoginIndicator = function (state) {
        if (state) {
            $(".gcoin-panel .panel-popup").removeClass("hide");
            $(".gcoin-panel .panel-filter").removeClass("hide");
        } else {
            $(".gcoin-panel .panel-popup").addClass("hide");
            $(".gcoin-panel .panel-filter").addClass("hide");
        }
    };
    $scope.checkPassword = function () {
        if ($scope.paypalEmail)
            toggleProgressLoginIndicator(true);

        else
            handleConvertGCoinFormError({ 'paypalEmail': 'Paypal email is required' });
    };

    var scrollTo = function ($element) {
        $('html, body').animate({
            scrollTop: $element.offset().top
        }, 500);
    };

    function handleTransactionStateChanged(event) {
        var url = $location.url();

        if (url.indexOf("/topup/") != -1) {
            if (url.indexOf("/paypal/") != -1)
                $scope.topUpMethodItem = "paypal";
                //else if(url.indexOf("/upoint/") != -1)
                //$scope.topUpMethodItem = "upoint";
            else if (url.indexOf("/topup-card/") != -1)
                $scope.topUpMethodItem = "topUpCard";
            if (!$scope.scrolled) {
                scrollTo($(".sect-panel"));
                $scope.scrolled = true;
            }
        } else if (url.indexOf("/exchange/") != -1) {
            if (!$scope.scrolled) {
                scrollTo($(".sect-panel"));
                $scope.scrolled = true;
            }
        } else if (url.indexOf("/gcoin/") != -1) {
            if (!$scope.scrolled) {
                scrollTo($(".sect-panel"));
                $scope.scrolled = true;
            }
        }

        window.scrollBy(0, 1);
        window.scrollBy(0, -1);
    }

    $scope.$on('$locationChangeSuccess', handleTransactionStateChanged);
}

function LocaleSelectCtrl($scope, $http) {
    $scope.locale = 'en';

    $scope.updateLocale = function () {
        $("#update-locale-form input[name='locale']").val($scope.locale);
        $("#update-locale-form").submit();
    }
}

function LocaleSelectCtrl($scope, $http) {
    $scope.locale = 'en';

    $scope.updateLocale = function () {
        $("#update-locale-form input[name='locale']").val($scope.locale);
        $("#update-locale-form").submit();
    }
}

function HomeCtrl($scope, $http) {
    $scope.init = function () {
        var show = false;
        $http({
            method: 'post',
            url: '/account/get-session/',
            headers: {
                'Content-type': 'application/json;'
            },
            data: {
            }
        }).success(function (data) {
            if ("success" in data) {
                show = data['success'];
                if (show == true)
                    $("#popupWindow").removeClass("popuphide");
            }
        });
    }
    $scope.closePopup = function () {
        $("#popupWindow").remove();
    }
}

gdineApp.controller('HomeCtrl', ['$scope', '$http', HomeCtrl]);
gdineApp.controller('TransferCtrl', ['$scope', '$http', TransferCtrl]);
gdineApp.controller('TransactionCtrl', ['$scope', '$http', '$location', TransactionCtrl]);
gdineApp.controller('LocaleSelectCtrl', ['$scope', '$http', LocaleSelectCtrl]);
