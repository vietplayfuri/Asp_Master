function EditProfileController($scope, $http, Upload) {
    var csrfToken = $("meta[name='csrf-token']").attr("content");
    $scope.countriesList = countriesList;
    $scope.countryName = '';
    $scope.invalidDate = false;
    $scope.invalid = false;
    $scope.$form = $("#editProfileForm");

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

    $scope.init = function (dobEpoch, countryCode, referralID) {
        $scope.countryCode = countryCode;
        $scope.countryName = findCountryName(countryCode);
        $scope.theDate = new Date(0);
        $scope.theDate.setUTCSeconds(parseInt(dobEpoch));

        $scope.selectedyear = $scope.theDate.getFullYear();
        $scope.selectedmonth = $scope.theDate.getMonth();
        $scope.selectedday = $scope.theDate.getDate();
        $scope.validateDate()

        for (var i = 0; i < $scope.countriesList.length; i++) {
            if ($scope.countriesList[i]['alpha-2'] == $scope.countryCode) {
                $scope.countryCode = {};
                $scope.countryCode.originalObject = $scope.countriesList[i];
            }
        }
    };

    $scope.validateDate = function () {
        var date = new Date($scope.selectedyear, $scope.selectedmonth, $scope.selectedday);
        var today = new Date();
        if (today - date < 0) {
            $scope.invalidDate = true;
        } else {
            $scope.invalidDate = false;
            $scope.dob = $scope.selectedyear + '-' + (parseInt($scope.selectedmonth) + 1) + '-' + $scope.selectedday
        }
    }

    var enableFieldErrors = function ($field, $errorsUl, errorMessages) {
        for (var i = 0; i < errorMessages.length; i++) {
            var $msg = $("<small></small>");
            $msg.attr("class", "label alert");
            $msg.html(errorMessages[i]);
            $errorsUl.append($msg);
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

    $scope.$watch('avatar', function () {
        if ($scope.avatar && $scope.avatar.length) {
            var isLoading = false;
            var avatar = $scope.avatar[0];
            var $avatarPhoto = $scope.$form.find("#avatar-photo");
            $avatarPhoto.data('original-photo', $avatarPhoto.attr('src'));
            Upload.upload({
                fields: {
                    'csrf_token': csrfToken
                },
                url: '/account/upload-avatar',
                file: avatar
            }).progress(function (evt) {
                if (!isLoading) {
                    isLoading = true;
                    $avatarPhoto.attr("src", "/static/images/loading.gif");
                }
            }).success(function (data, status, headers, config) {
                isLoading = false;
                if (data.success) {
                    $avatarPhoto.attr("src", data.filename + "?rnd=" + Math.floor((Math.random() * 10000) + 1));
                    $scope.avatar = data.filename;
                }
                else
                {
                    $avatarPhoto.attr("src", $avatarPhoto.data('original-photo'));
                }
            }).error(function (data) {
                isLoading = false;
                $avatarPhoto.attr("src", $avatarPhoto.data('original-photo'));
            });
        }
    });



    $scope.editProfile = function (e) {
        if ($scope.invalid || $scope.invalidDate) {
            return false;
        }
        $http({
            method: 'POST',
            url: '/account/edit-profile',
            headers: {
                'Content-type': 'application/json; charset=utf-8',
                'X-CSRFToken': csrfToken
            },
            data: {
                'username': $scope.username,
                'nickname': $scope.nickName,
                'email': $scope.email,
                'country_code': $scope.countryCode.originalObject['alpha-2'],
                'dob': $scope.dob,
                'inviter_username': $scope.inviterUsername,
                'avatar_filename': $scope.avatar,
                'password': $scope.password,
                'new_password': $scope.newPassword,
                'confirm_new_password': $scope.confirmPassword,
                'submit': 'submit'
            }
        }).success(function (data) {
            if (data.success) {
                location.href = data.redirect;
            } else if ("errors" in data) {
                handleFormError(data["errors"], false);
            }
        }).error(function (data) {
            if ("errors" in data) {
                handleFormError(data["errors"], false);
            }
        });
    };
}

gtokenApp.controller('EditProfileController', ['$scope', '$http', 'Upload', EditProfileController]);
