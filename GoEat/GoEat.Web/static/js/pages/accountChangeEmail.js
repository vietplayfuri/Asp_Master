function AccountChangeEmailCtrl($scope, $http) {
    var csrfToken = $("meta[name='csrf-token']").attr("content");

    $scope.form = {};
    var $form = $("#account-change-email");
    var url = $form.data().endpointUrl;

    var resetFormError = function () {
        var $errorsUl = $form.find(".errors-container ul.errors");
        $errorsUl.find(".error").remove();
    };

    var enableFieldErrors = function($field, $errorsUl, errorMessages) {
        $field.removeClass("field-valid");
        $field.addClass("field-with-errors");

        for (var i = 0; i < errorMessages.length; i++) {
            var $errorLi = $("<li></li>");
            $errorLi.attr("class", "error");
            $errorLi.attr("data-error-message", errorMessages[i]);
            $errorLi.html("*" + errorMessages[i]);
            $errorsUl.append($errorLi);
        }
    };

    var enableFieldValid = function($field) {
        $field.removeClass("field-with-errors");
        $field.addClass("field-valid");
    };

    var handleFormError = function (errorsData, ignoreFirstTime) {
        resetFormError();

        for (var field in errorsData) {
            if (errorsData.hasOwnProperty(field)) {
                var $field = $form.find("[name='" + field + "']");
                var $errorsUl = $field.next(".errors-container").find("ul.errors");

                if (!$field.data().firstTimeEdit || ignoreFirstTime) {
                    enableFieldErrors($field, $errorsUl, errorsData[field]);
                }
            }
        }
    };

    var handleFormValid = function () {
        resetFormError();

        for (var field in $scope.form) {
            if ($scope.form.hasOwnProperty(field)) {
                var $field = $form.find("[name='" + field + "']");
                enableFieldValid($field);
            }
        }
    };

    $scope.removeFirstTimeFlag = function (event) {
        var $field = $(event.target);
        $field.data().firstTimeEdit = false;
    };

    $scope.isFormValid = function () {
        var $errorsUl = $form.find(".errors-container ul.errors");
        return $errorsUl.find(".error").length == 0;
    };

    $scope.validate = function () {
        $http({
            method: 'post',
            url: url,
            headers: {
                'Content-type': 'application/json;',
                'X-CSRFToken': csrfToken
            },
            data: {
                email: $scope.form.email,
                confirmEmail: $scope.form.confirmEmail,
                password: $scope.form.password
            }
        }).success(function(data) {
            if ("correct" in data && data.correct) {
                handleFormValid();
            }
        }).error(function (data) {
            if ("errors" in data) {
                handleFormError(data["errors"], false);
            }
        });
    };

    $scope.submitForm = function () {
        if ($scope.isFormValid()) {
            $http({
                method: 'post',
                url: url,
                headers: {
                    'Content-type': 'application/json;',
                    'X-CSRFToken': csrfToken
                },
                data: {
                    email: $scope.form.email,
                    confirmEmail: $scope.form.confirmEmail,
                    password: $scope.form.password,
                    submit: "submit"
                }
            }).success(function(data) {
                if ("success" in data && data.success) {
                    location.href = data.redirect;
                }
            }).error(function (data) {
                if ("errors" in data) {
                    handleFormError(data["errors"], false);
                }
            });
        }
    };
}
gdineApp.controller('AccountChangeEmailCtrl', ['$scope', '$http', AccountChangeEmailCtrl]);
