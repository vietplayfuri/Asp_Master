$(document).ready(function () {
    $('.close-notification').click(function () {
        $(".exchange-panel .notification-panel").addClass("hide");
        $(".exchange-panel .panel-filter").addClass("hide");
        $(".top-up-panel .notification-panel").addClass("hide");
        $(".top-up-panel .panel-filter").addClass("hide");
    });

    $('#game-result').slimScroll({
        height: '288px'
    });
});

function ExchangeCtrl($scope, $http, $location, $timeout) {
    var isInit = true;
    $scope.Math = window.Math;
    /* MVVM binding */
    var initGames = function (response) {
        var selectedGameIndex;
        $scope.gameList = response.data;
        for (var i = 0, j = $scope.gameList.length; i < j; i++) {
            $scope.gameList[i].isActive = false;
            $scope.gameList[i].gameExchange = [];
            if ($scope.gameList[i].id == gameID) selectedGameIndex = i;
        }
        if (isInit && selectedGameIndex !== undefined) {
            $timeout(function () {
                $scope.gameClickHandler($scope.gameList[selectedGameIndex]);
            }, 500);
            isInit = false;
        }
    };

    $scope.searchTerm = '';
    $scope.$watch('searchTerm', function (newVal, oldVal) {
        $scope.showAll = true;
        $scope.displayConfirm = false;
        $http.get('/game/game-listing?orderKey=recent&term=' + newVal).then(initGames);
    });

    $http.get('/game/game-listing?orderKey=recent').then(initGames);
    $scope.showAll = true;
    $scope.gameClickHandler = function (game) {
        $scope.selectedGame = game;
        $scope.displayConfirm = false;
        game.isActive = !game.isActive;
        $scope.showAll = !game.isActive;
        game.selectedExchange = null;
        game.isActive && $http.get('/game/' + game.id + '/exchange-items').then(function (res) {
            game.gameExchange = res.data;
            $location.search('game-id', game.id);
            gameID = game.id;
        });
    };
    $scope.selectExchange = function (exchange) {
        $scope.displayConfirm = true;
        $scope.selectedExchange = exchange;
    };
    $scope.submit = function () {
        var csrfToken = $("meta[name='csrf-token']").attr("content");
        $scope.showLoading = true;
        $http({
            method: 'post',
            url: '/transaction/exchange',
            headers: {
                'Content-type': 'application/json;',
                'X-CSRFToken': csrfToken
            },
            data: {
                gameID: $scope.selectedGame.id,
                exchangeOption: $scope.selectedGame.gameExchange.Credit.length ? 'CreditType' : 'Package',
                exchangeOptionID: $scope.selectedExchange.id,
                exchangeAmount: $scope.selectedExchange.number || 1,
                executeExchange: true
            }
        }).success(function (data) {
            if ("success" in data) {
                window.location.reload();
            }
            else if ("errors" in data) {
                if (code === 500) window.location.href = '/500';
                $scope.exchangeError = data.errors.game_id;
            }
        }).error(function (data, code) {
            if (code === 500) window.location.href = '/500';
            $scope.exchangeError = data.errors.game_id;
        });
    };
    $scope.validateNumber = function (exchange) {
        angular.forEach($scope.selectedGame.gameExchange.Credit, function (item, index) {
            if (item !== exchange) item.message = '';
        });
        angular.forEach($scope.selectedGame.gameExchange.Package, function (item, index) {
            item.message = '';
        });
        var csrfToken = $("meta[name='csrf-token']").attr("content");

        $http({
            method: 'post',
            url: '/transaction/exchange',
            headers: {
                'Content-type': 'application/json;',
                'X-CSRFToken': csrfToken
            },
            data: {
                gameID: $scope.selectedGame.id,
                exchangeOption: $scope.selectedGame.gameExchange.Credit.length ? 'CreditType' : 'Package',
                exchangeOptionID: exchange.id,
                exchangeAmount: exchange.number || 1
            }
        }).success(function (data) {
            if ("success" in data) {
                $http({
                    method: 'post',
                    url: '/transaction/exchange-calculate',
                    headers: {
                        'Content-type': 'application/json;',
                        'X-CSRFToken': csrfToken
                    },
                    data: {
                        gameID: $scope.selectedGame.id,
                        exchangeOption: $scope.selectedGame.gameExchange.Credit.length ? 'CreditType' : 'Package',
                        exchangeOptionID: exchange.id,
                        exchangeAmount: exchange.number || 1
                    }
                }).success(function (data) {
                    exchange.message = '';
                    $scope.selectedExchange = exchange;
                    if ($scope.selectedGame.gameExchange.Package.length > 0) $scope.displayConfirm = true;
                });
            } else if ('errors' in data) {
                exchange.message = data.errors.exchange_amount[0];
            } else {
                exchange.message = '';
            }
        }).error(function (data) {
            if ('errors' in data) {
                exchange.message = data.errors.exchange_amount[0];
            } else {
                exchange.message = '';
            }
        });

    };
    $scope.closeMessage = function () {
        $scope.transactionMessage = false;
        $scope.gameClickHandler($scope.selectedGame);
    };
    $scope.reloadPage = function () {
        window.location.reload();
    }
    /* End of MVVM binding */
}
gtokenApp.controller('ExchangeCtrl', ['$scope', '$http', '$location', '$timeout', ExchangeCtrl]);
