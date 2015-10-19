function AccountRegisterCtrl($scope, $http) {
    var csrfToken = $("input[name='__RequestVerificationToken']").attr("value");

    $scope.countriesList = countriesList;
    $scope.formData = {
        "username": "",
        "email": "",
        "confirmEmail": "",
        "password": "",
        "referralID": ""
    };
    $scope.countryName = '';
    $scope.$form = $("#accounts_signup");
    var url = $scope.$form.data().endpointUrl;
    $scope.formValid = false;

    var enableFieldErrors = function ($field, $errorsUl, errorMessages) {
        $field.removeClass("field-valid");
        $field.addClass("field-with-errors");

        // Clears all the past errors
        $errorsUl.find(".error").remove();

        for (var i = 0; i < errorMessages.length; i++) {
            var $errorLi = $("<li></li>");
            $errorLi.attr("class", "error");
            $errorLi.attr("data-error-message", errorMessages[i]);
            $errorLi.html("*" + errorMessages[i]);
            $errorsUl.append($errorLi);
        }
    };

    var enableFieldValid = function ($field, $errorsUl) {
        $field.removeClass("field-with-errors");
        $field.addClass("field-valid");
        $errorsUl.find(".error").remove();
    };

    var resetField = function ($field, $errorsUl) {
        $field.removeClass("field-with-errors");
        $field.removeClass("field-valid");
        $errorsUl.find(".error").remove();
    };

    var handleFormError = function (errorsData, ignoreFirstTime) {
        for (var fieldName in $scope.formData) {
            if ($scope.formData.hasOwnProperty(fieldName)) {
                var $field = $scope.$form.find("[name='" + fieldName + "']");
                var $errorsUl = $field.next(".errors-container").find("ul.errors");

                if (!$field.data().firstTimeEdit || ignoreFirstTime) {
                    if (fieldName in errorsData) {
                        enableFieldErrors($field, $errorsUl, errorsData[fieldName]);
                    } else {
                        if (fieldName === "referralID" && $field.val().length == 0) {
                            resetField($field, $errorsUl);
                        } else {
                            enableFieldValid($field, $errorsUl);
                        }
                    }
                }
            }

            if ("CustomError" in errorsData) {
                $($('div.errors-container > ul.errors')[5]).children().children().html(errorsData.CustomError);
                $($('div.errors-container > ul.errors')[5]).show();
            }
            else {
                $($('div.errors-container > ul.errors')[5]).children().children().html('');
                $($('div.errors-container > ul.errors')[5]).hide();
            }
        }
    };
    var handleFormValid = function () {
        for (var fieldName in $scope.formData) {
            if ($scope.formData.hasOwnProperty(fieldName)) {
                var $field = $scope.$form.find("[name='" + fieldName + "']");
                var $errorsUl = $field.next(".errors-container").find("ul.errors");
                enableFieldValid($field, $errorsUl);
            }
        }
    };

    $scope.init = function () {
        $scope.formData.referralID = referralID;

        //if ($scope.formData.referralID) {
        //    $scope.$form.find("[name='referralID']").attr('readonly', true);
        //}
    };

    $scope.removeFirstTimeFlag = function (event) {
        var $field = $(event.target);
        $field.data().firstTimeEdit = false;
    };

    $scope.checkFormData = function () {
        $http({
            method: 'post',
            url: url,
            headers: {
                'Content-Type': 'application/json; charset=utf-8',
                'X-CSRFToken': csrfToken
            },
            data: {
                username: $scope.formData.username,
                email: $scope.formData.email,
                confirmEmail: $scope.formData.confirmEmail,
                password: $scope.formData.password,
                referralID: $scope.formData.referralID,
                acceptTOS: $scope.$form.find("input[name='acceptTOS']:checked").length == 1 ? true : false
            }
        }).success(function (data) {
            if ("correct" in data && data.correct) {
                handleFormValid();
            } else {
                if ("errors" in data) {
                    handleFormError(data["errors"], false);
                }
            }
        }).error(function (data, status) {
            console.log(data);
            //if ("errors" in data) {
            //    handleFormError(data["errors"], false);
            //}
        });
    };

    $scope.isFormValid = function () {
        return ($scope.$form.find(".field-with-errors").length == 0
            && $scope.$form.find("input[name='acceptTOS']:checked").length == 1
            && $scope.accountsRegisterForm.$valid);
    };

    $scope.submitForm = function (e) {
        if ($scope.isFormValid()) {
            $http({
                method: 'post',
                url: url,
                headers: {
                    'Content-Type': 'application/json; charset=utf-8',
                    'X-CSRFToken': csrfToken
                },
                data: {
                    username: $scope.formData.username,
                    email: $scope.formData.email,
                    confirmEmail: $scope.formData.confirmEmail,
                    password: $scope.formData.password,
                    referralID: $scope.formData.referralID,
                    acceptTOS: $scope.$form.find("input[name='acceptTOS']:checked").length == 1 ? true : false,
                    submit: "submit"
                }
            }).success(function (data) {
                if ("success" in data && data.success) {
                    location.href = data.redirect;
                }
                else {
                    if ("errors" in data) {
                        handleFormError(data["errors"], false);
                    }
                }
            }).error(function (data, status) {
                if ("errors" in data) {
                    handleFormError(data["errors"], false);
                }
            });
        }
    }
}
gdineApp.config(function ($interpolateProvider) {
    $interpolateProvider.startSymbol('{$');
    $interpolateProvider.endSymbol('$}');
});
gdineApp.controller('AccountRegisterCtrl', ['$scope', '$http', AccountRegisterCtrl]);
