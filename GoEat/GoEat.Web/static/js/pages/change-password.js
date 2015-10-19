gdineApp.directive('myMaxlength', ['$compile', '$log', function ($compile, $log) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elem, attrs, ctrl) {
            attrs.$set("ngTrim", "false");
            var maxLength = parseInt(attrs.myMaxlength, 10);
            ctrl.$parsers.push(function (value) {
                if (maxLength > 0 && value.length > maxLength) {
                    value = value.substr(0, maxLength);
                    ctrl.$setViewValue(value);
                    ctrl.$render();
                }
                return value;
            });
        }
    };
}]);

function ProfileInfoCtrl($scope, $http) {
    
    $scope.submitChangePassword = function (event) {
        var form = $('form[id="edit-profile"]');
        if (!form.valid()) {
            event.preventDefault();
            return;
        }

        var editPassword = new Object();
        editPassword.OldPassword = $('input[name="OldPassword"]').val();
        editPassword.NewPassword = $('input[name="NewPassword"]').val();
        editPassword.ConfirmPassword = $('input[name="ConfirmPassword"]').val();

        $.ajax({
            type: "POST",
            url: "/account/change-password",
            data: JSON.stringify(editPassword),
            contentType: "application/json",
            success: function (returnedData) {
                if (returnedData.success) {
                    window.location.href = '/account/profile';
                }
                else {
                    alert(returnedData.message)
                }
            }
        });
    };
}
gdineApp.controller('ProfileInfoCtrl', ['$scope', '$http', ProfileInfoCtrl]);
