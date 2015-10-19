function ImportRefferralCtrl($scope, $http) {
    var csrfToken = $("meta[name='csrf-token']").attr("content");

    $scope.used = 0;
    $scope.total = 0;
    $scope.percent = 0;
    $scope.referral_campaign = 0;
    $scope.list_campaigns = [];
    $scope.data = null;

    $scope.init = function () {
        $scope.get_referral_campaign();
        $scope.submitForm();
        $scope.change_Referral_campaign();
        $scope.referral_campaign = referral_campaign;
    };

    $scope.submitForm = function (e) {
        $scope.importHistory = [];
        $http({
            method: 'post',
            url: '/admin/referral/import-history-list',
            headers: {
                'Content-type': 'application/json;'
            },
            data: {
            }
        }).success(function (data) {
            if ("success" in data && data.success) {
                angular.forEach(data.importlist, function (value, key) {
                    $scope.importHistory.push(value);
                });
            }
        }).error(function (data) {
            if ("errors" in data) {
                handleFormError(data["errors"], false);
            }
        });
    };

    $scope.change_Referral_campaign = function (e) {
        $http({
            method: 'post',
            url: '/admin/referral/import-referral/' + $scope.referral_campaign.id,
            headers: {
                'Content-type': 'application/json;'
            },
            data: {
            }
        }).success(function (data) {
            if ("success" in data && data.success) {
                $scope.used = data.data.used;
                $scope.total = data.data.total;
                $scope.percent = data.data.percent;
            }
        }).error(function (data) {
            if ("errors" in data) {
                handleFormError(data["errors"], false);
            }
        });
    };


    $scope.get_referral_campaign = function (e) {
        $http({
            method: 'post',
            url: '/admin/referral/import-referral/campaigns',
            headers: {
                'Content-type': 'application/json;'
            },
            data: {
            }
        }).success(function (data) {
            if ("success" in data && data.success) {
                angular.forEach(data.data, function (value, key) {
                    var item = new Object();
                    item.id = value.id;
                    item.name = value.title;
                    $scope.list_campaigns.push(item);
                });

                $scope.referral_campaign = $scope.list_campaigns[0];
                $scope.used = data.data[0].gt_usage;
                $scope.total = data.data[0].quantity;
                $scope.percent = data.data[0].percent;
            }
        }).error(function (data) {
            if ("errors" in data) {
                handleFormError(data["errors"], false);
            }
        });
    };

    $scope.handleFileSelect = function (evt) {
        var file = evt.target.files[0];

        if (file != null && file.name.indexOf(".csv") == -1) {
            $("#csv-file").val("");
            return alert("File is invalid");
        }

        Papa.parse(file, {
            dynamicTyping: true,
            complete: function (results) {
                $scope.data = results;
                console.log($scope.data);
                $scope.validateCSV();
            }
        });
    };

    $scope.validateCSV = function () {
        if ($scope.data.data.length > 0) {
            for (var i = 0; i < $scope.data.data.length; i++) {
                if (i == 0) {
                    if (($scope.data.data[i][0] != "Downloader username") || ($scope.data.data[i][1] != "Referral username")) {
                        $("#csv-file").val("");
                        return alert("File header should be: Downloader username, Referral username");
                    }
                }
            }
            $scope.processImportFile();
        }
    };

    $scope.processImportFile = function () {
        var totalRecord = $scope.data.data.length - 2;        
        $http({
            method: 'post',
            url: '/admin/referral/pro-process-import-file?referralID=' + $scope.referral_campaign.id + '&totalRecord=' + totalRecord,
            headers: {
                'Content-type': 'application/json;'
            },
            data: {
            }
        }).success(function (data) {
            if ("success" in data && data.success) {
                if ("message" in data) {
                    var r = confirm(data.message);
                    if (r == false) {
                        $("#csv-file").val("");
                    }
                }
            }
        }).error(function (data) {
            if ("errors" in data) {
                handleFormError(data["errors"], false);
            }
        });
    };
    $scope.reValidateFile = function () {
        if ($("#csv-file").val() != "")
            $scope.processImportFile();
    };
}
gtokenApp.controller('ImportRefferralCtrl', ['$scope', '$http', ImportRefferralCtrl]);