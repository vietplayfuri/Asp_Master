function AccountRegisterCtrl($scope, $http) {
    var csrfToken = $("input[name='__RequestVerificationToken']").val();

    $scope.countriesList = countriesList;
    $scope.formData = {
        "username": "",
        "email": "",
        "confirmEmail": "",
        "password": "",
        "countryCode": "",
        "referralID": "",
        "CustomError": ""
    };
    $scope.countryName = '';
    $scope.$form = $("#accounts_signup");
    var url = $scope.$form.data().endpointUrl;
    $scope.formValid = false;
    $scope.checkValidateTimeout;

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
        }
    };
    var findCountryName = function (countryCode) {
        length = countriesList.length;

        for (var i = 0; i < length; i++) {
            code = countriesList[i]['alpha-2'];
            if (countryCode == code) {
                return countriesList[i]['name'];
            }
        }
        return '';
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
        $scope.formData.countryCode = countryCode;
        $scope.countryName = findCountryName($scope.formData.countryCode);
        $scope.formData.referralID = referralID;
        $scope.formData.email = facebook_email;
        if ($scope.formData.referralID) {
            // KNN might not be the best way to select the field
            $scope.$form.find("[name='referralID']").attr('readonly', true);
        }
        for (var i = 0; i < $scope.countriesList.length; i++) {
            if ($scope.countriesList[i]['alpha-2'] == $scope.formData.countryCode) {
                $scope.userCountry = {};
                $scope.userCountry.originalObject = $scope.countriesList[i];
            }
        }
    };

    $scope.removeFirstTimeFlag = function (event) {
        var $field = $(event.target);
        $field.data().firstTimeEdit = false;
    };

    $scope.checkFormData = function () {
        clearTimeout($scope.checkValidateTimeout);

        $scope.checkValidateTimeout = setTimeout(function () {
            $http({
                method: 'post',
                url: url,
                headers: {
                    'Content-type': 'application/json; charset=utf-8',
                    'X-CSRFToken': csrfToken
                },
                data: {
                    username: $scope.formData.username,
                    email: $scope.formData.email,
                    confirmEmail: $scope.formData.confirmEmail,
                    password: $scope.formData.password,
                    countryCode: $scope.userCountry.originalObject['alpha-2'],
                    referralID: $scope.formData.referralID,
                    acceptTOS: $scope.$form.find("input[name='acceptTOS']:checked").length == 1 ? "y" : ""
                }
            }).success(function (data) {
                if ("correct" in data && data.correct) {
                    handleFormValid();
                } else if ("errors" in data) {
                    handleFormError(data["errors"], false);
                }
            }).error(function (data) {
                if ("errors" in data) {
                    handleFormError(data["errors"], false);
                }
            });
        }, 800);
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
                //url: url + $scope.formData.referralID,
                url: url,// + "?referralId=" + $scope.formData.referralId,
                headers: {
                    'Content-type': 'application/json; charset=utf-8',
                    'X-CSRFToken': csrfToken
                },
                data: {
                    username: $scope.formData.username,
                    email: $scope.formData.email,
                    confirmEmail: $scope.formData.confirmEmail,
                    password: $scope.formData.password,
                    countryCode: $scope.userCountry.originalObject['alpha-2'],
                    referralID: $scope.formData.referralID,
                    countryName: $scope.userCountry.originalObject['name'],
                    //acceptTOS: $scope.$form.find("input[name='acceptTOS']:checked").length == 1 ? "y" : "",
                    submit: "submit"
                }
            }).success(function (data) {
                if ("success" in data && data.success) {
                    location.href = data.redirect;
                } else if ("errors" in data) {
                    handleFormError(data["errors"], false);
                }
            }).error(function (data) {
                if ("errors" in data) {
                    handleFormError(data["errors"], false);
                }
            });
        }
    }

    $scope.passwordReveal = 'password';

    $scope.revealPasswordClick = function (e) {
        $scope.passwordReveal = $scope.passwordReveal === 'text' ? 'password' : 'text';
    };
}

gtokenApp.controller('AccountRegisterCtrl', ['$scope', '$http', AccountRegisterCtrl]);
