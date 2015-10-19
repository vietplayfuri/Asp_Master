function AccountRegisterCtrl($scope, $http) {
    var csrfToken = $("input[name='__RequestVerificationToken']").val();

    $scope.countriesList = countriesList;
    $scope.formData = {
        "username": "",
        "email": "",
        "confirmPassword": "",
        "password": "",
        "countryCode": "",
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
    var handleFormError = function (errorsData, ignoreFirstTime) {
        $scope.$form.find(".errors-container small").remove();
        console.log(errorsData);
        for (fieldName in errorsData) {
            var $field = $scope.$form.find("[name='" + fieldName + "']");
            var $parent = $field.closest('.columns');
            var $errorField = $parent.find(".errors-container");
            enableFieldErrors($field, $errorField, errorsData[fieldName])
        }
    }

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

    $scope.init = function () {
        $scope.formData.countryCode = countryCode;
        $scope.countryName = findCountryName($scope.formData.countryCode);
        $scope.formData.referralID = referralID;

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

    $scope.isFormValid = function () {
        return $scope.accountsRegisterForm.$valid;
        // return ($scope.$form.find(".field-with-errors").length == 0
        //     && $scope.$form.find("input[name='acceptTOS']:checked").length == 1
        //     && $scope.accountsRegisterForm.$valid);
    };

    $scope.submitForm = function (e) {
        if ($scope.isFormValid()) {
            $http({
                method: 'post',
                url: url + "?referralID=" + $scope.formData.referralID,
                headers: {
                    'Content-type': 'application/json; charset=utf-8',
                    'X-CSRFToken': csrfToken
                },
                data: {
                    username: $scope.username,
                    email: $scope.formData.email,
                    confirmPassword: $scope.formData.confirmPassword,
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
}

function usernameExist($http) {
    return {
        require: 'ngModel',
        link: function (scope, element, attributes, ngModel, ctrl) {

            ngModel.$validators.usernameExist = function (username) {
                if (username.length < 3) {
                    return true;
                }

                $http({
                    method: 'post',
                    url: '/account/register/',
                    data: {
                        username: username,
                        email: scope.formData.email,
                        confirmPassword: scope.formData.confirmPassword,
                        password: scope.formData.password,
                        countryCode: scope.userCountry.originalObject['alpha-2'],
                        referralID: scope.formData.referralID
                    },
                    headers: {
                        'Content-type': 'application/json; charset=utf-8',
                        'X-CSRFToken': $("input[name='__RequestVerificationToken']").val()
                    }
                }).success(function (data) {
                    if ("correct" in data && data.correct) {
                        ngModel.$setValidity('usernameExist', true);
                        scope.username = username;
                        return true;
                    }
                    else if ("errors" in data) {
                        if (data.errors.username !== undefined && data.errors.username == "This username is already taken") {
                            ngModel.$setValidity('usernameExist', false);
                            return false;
                        }
                        else {
                            ngModel.$setValidity('usernameExist', true);
                            scope.username = username;
                            return true;
                        }
                    }
                    return true;
                }).error(function (data) {
                    if ("errors" in data && data.errors.username !== undefined && data.errors.username == "This username is already taken") {
                        ngModel.$setValidity('usernameExist', false);
                        return false;
                    } else {
                        ngModel.$setValidity('usernameExist', true);
                        scope.username = username;
                        return true;
                    }
                });

            };
        }
    };
}

function referralIdNotExist($http) {
    return {
        require: 'ngModel',
        link: function (scope, element, attributes, ngModel, ctrl) {

            ngModel.$validators.referralIdNotExist = function (referralID) {
                if (referralID.length < 3) {
                    return true;
                }

                $http({
                    method: 'post',
                    url: '/account/register/',
                    data: {
                        username: scope.username,
                        email: scope.formData.email,
                        confirmPassword: scope.formData.confirmPassword,
                        password: scope.formData.password,
                        countryCode: scope.userCountry.originalObject['alpha-2'],
                        referralID: referralID
                    },
                    headers: {
                        'Content-type': 'application/json; charset=utf-8',
                        'X-CSRFToken': $("input[name='__RequestVerificationToken']").val()
                    }
                }).success(function (data) {
                    if ("correct" in data && data.correct) {
                        ngModel.$setValidity('referralIdNotExist', true);
                        scope.formData.referralID = referralID;
                        return true;
                    } else if ("errors" in data) {
                        if (data.errors.referralID !== undefined && data.errors.referralID == "Referral Code does not exist") {
                            ngModel.$setValidity('referralIdNotExist', false);
                            return false;
                        } else {
                            ngModel.$setValidity('referralIdNotExist', true);
                            scope.formData.referralID = referralID;
                            return true;
                        }
                    }
                    return true;
                }).error(function (data) {
                    if ("errors" in data && data.errors.referralID !== undefined && data.errors.referralID == "Referral Code does not exist") {
                        ngModel.$setValidity('referralIdNotExist', false);
                        return false;
                    } else {
                        ngModel.$setValidity('referralIdNotExist', true);
                        scope.formData.referralID = referralID;
                        return true;
                    }
                });

            };
        }
    };
}

gtokenApp.controller('AccountRegisterCtrl', ['$scope', '$http', AccountRegisterCtrl]);
gtokenApp.directive('usernameExist', ['$http', usernameExist]);
gtokenApp.directive('referralIdNotExist', ['$http', referralIdNotExist]);
