function CustomerSupportCtrl($scope, $http) {
    var $csForm = $("#customer-support-form");
    var csrfToken = $("meta[name='csrf-token']").attr("content");
    var customerSupportURL = $csForm.data().endpointUrl;

    $scope.form = {};
    $scope.form.game = {};
    $scope.form.game.os = {};

    var resetFormError = function () {
        var $errorsUl = $csForm.find(".errors-container ul.errors");
        $errorsUl.find(".error").remove();
    };

    var handleFormError = function (errorsData, ignoreFirstTime) {
        resetFormError();

        for (var field in errorsData) {
            if (errorsData.hasOwnProperty(field)) {
                var $field = $csForm.find("[name='" + field + "']");
                if ($field.is("select")) {
                    var $errorsUl = $field.parents(".select-platform-container").find(".errors-container ul.errors");
                } else {
                    var $errorsUl = $field.next(".errors-container").find("ul.errors");
                }

               // if (!ignoreFirstTime) {
                    var errorMsg = errorsData[field];
                    var $errorLi = $("<li class='error'></li>");
                    $errorLi.attr('data-error-message', errorMsg);
                    $errorLi.html('*' + errorMsg);
                    $errorsUl.append($errorLi);
               // }
            }
        }
    };

    $scope.removeFirstTimeFlag = function (event) {
        var $field = $(event.target);
        $field.data().firstTimeEdit = false;
    };

    $scope.isCsFormValid = function () {
        var $errorsUl = $csForm.find(".errors-container ul.errors");
        return $errorsUl.find(".error").length == 0;
    };

    $scope.validate = function () {
        $http({
            method: 'post',
            url: customerSupportURL,
            headers: {
                'Content-type': 'application/json;',
                'X-CSRFToken': csrfToken
            },
            data: {
                customerName: $scope.form.customerName,
                customerEmail: $scope.form.customerEmail,
                subject: $scope.form.subject,
                message: $scope.form.message,
                platform: $scope.form.platform,
                gameID: $scope.form.game.id || -1,
                gameVersion: $scope.form.game.version || '',
                gameDevice: $scope.form.game.device || '',
                gameOSName: $scope.form.game.os.name || '',
                gameOSVersion: $scope.form.game.os.version || '',
                forValidate: true
            }
        }).success(function (data) {
            if ("correct" in data)
            {
                resetFormError();
            } else if ("errors" in data)
            {
                handleFormError(data['errors'], false);
            }
        }).error(function (data) {
            if ("errors" in data) {
                handleFormError(data['errors'], false);
            }
        });
    };

    $scope.submitCsForm = function () {
        if ($scope.isCsFormValid()) {
            $http({
                method: 'post',
                url: customerSupportURL,
                headers: {
                    'Content-type': 'application/json;',
                    'X-CSRFToken': csrfToken
                },
                data: {
                    customerName: $scope.form.customerName,
                    customerEmail: $scope.form.customerEmail,
                    subject: $scope.form.subject,
                    message: $scope.form.message,
                    platform: $scope.form.platform,
                    gameID: $scope.form.game.id || -1,
                    gameVersion: $scope.form.game.version || '',
                    gameDevice: $scope.form.game.device || '',
                    gameOSName: $scope.form.game.os.name || '',
                    gameOSVersion: $scope.form.game.os.version || ''
                }
            }).success(function (data) {
                if ("success" in data) {
                    location.href = location.href;
                } else if ("errors" in data) {
                    handleFormError(data['errors'], false);
                }
            }).error(function (data) {
                if ("errors" in data) {
                    handleFormError(data['errors'], true);
                }
            });
        }
    };
}
gtokenApp.controller('CustomerSupportCtrl', ['$scope', '$http', CustomerSupportCtrl]);
