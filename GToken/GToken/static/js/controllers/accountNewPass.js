function AccountNewPassCtrl($scope, $http) {
    var csrfToken = $("input[name='__RequestVerificationToken']").val();

    $scope.formData = {
        "verifyCode": "",
        "username": "",
        "confirmPassword": "",
        "password": ""
    }; 
    $scope.$form = $("#accounts_new_pass");
    var url = $scope.$form.data().endpointUrl;
    $scope.formValid = false;

    var enableFieldErrors = function($field, $errorsUl, errorMessages) {
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

    var enableFieldValid = function($field, $errorsUl) {
        $field.removeClass("field-with-errors");
        $field.addClass("field-valid");
        $errorsUl.find(".error").remove();
    };

    var resetField = function ($field, $errorsUl) {
        $field.removeClass("field-with-errors");
        $field.removeClass("field-valid");
        $errorsUl.find(".error").remove();
    };


    $scope.init = function () {
        
    };

    $scope.removeFirstTimeFlag = function (event) {
        var $field = $(event.target);
        $field.data().firstTimeEdit = false;
    };

    $scope.isFormValid = function() {
        return $scope.accountsNewPassForm.$valid;
    };

    $scope.submitForm = function(e) {
        if ($scope.isFormValid()) {
            $http({
                method: 'post',
                url: url,
                headers: {
                    'Content-type': 'application/json; charset=utf-8',
                    'X-CSRFToken': csrfToken
                },
                data: {
                    verifyCode: $scope.verifyCode,
                    username: $scope.username,
                    confirmPassword: $scope.formData.confirmPassword,
                    password: $scope.formData.password,
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
    }
}

gtokenApp.controller('AccountNewPassCtrl', ['$scope', '$http', AccountNewPassCtrl]);