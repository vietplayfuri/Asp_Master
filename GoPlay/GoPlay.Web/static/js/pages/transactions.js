$(document).ready(function () {
    $('.close-notification').click(function () {
        $(".exchange-panel .notification-panel").addClass("hide");
        $(".exchange-panel .panel-filter").addClass("hide");
        $(".top-up-panel .notification-panel").addClass("hide");
        $(".top-up-panel .panel-filter").addClass("hide");
    });
});

function ExchangeCtrl($scope, $http, $location, $timeout) {
    var isInit = true;
    $scope.Math = window.Math;
    $scope.scrollBigGuy = true;
    /* MVVM binding */
    var initGames = function (response) {
        var selectedGameIndex;
        $scope.gameList = response.data;
        if ($scope.promotionTab) {
            $scope.gameList = $scope.gameList.filter(function (g) {
                return g.promotion.code != null;
            });
        }
        for (var i = 0, j = $scope.gameList.length; i < j; i++) {
            $scope.gameList[i].isActive = false;
            $scope.gameList[i].gameExchange = [];
            if ($scope.gameList[i].id == $location.search()['game-id']) selectedGameIndex = i;
        }
        if (isInit && selectedGameIndex !== undefined) {
            $scope.gameClickHandler($scope.gameList[selectedGameIndex]);
            isInit = false;
        }
        if ($scope.searchTerm && $scope.gameList.length) {
            $scope.gameList[0].selected = true;
            selectedIndex = 0;
        } else {
            selectedIndex = -1;
        }
        $scope.showLoading = false;
        $scope.showAll = true;
    };

    $scope.searchTerm = '';
    $scope.promotionTab = $location.search()['tab'] !== 'all' && $location.search()['tab'] != null;
    function resetSearch() {
        $scope.showAll = false;
        $scope.showLoading = true;
        $scope.displayConfirm = false;
        $scope.selectedGame = null;
        $scope.scrollBigGuy = true;
        $scope.selectedGame = null;
    }
    $scope.$watch('searchTerm', function (newVal, oldVal) {
        resetSearch();
        $http.get('/game/game-listing?orderKey=recent&term=' + newVal).then(initGames);
    });
    $scope.switchTab = function (val) {
        resetSearch();
        $scope.promotionTab = val;
        $scope.exchangeError = '';
        $location.search('tab', val ? 'promotion' : 'all');
        $location.search('game-id', null);
        $http.get('/game/game-listing?orderKey=recent&term=' + $scope.searchTerm).then(initGames);
    };

    //$http.get('/game/game-listing?orderKey=recent').then(initGames);
    $scope.showAll = true;
    $scope.gameClickHandler = function (game) {
        var index = $scope.gameList.indexOf(game);
        $scope.selectedGame = game;
        $scope.displayConfirm = false;
        $scope.exchangeError = '';
        game.isActive = !game.isActive;
        game.selectedExchange = null;
        if (game.isActive) {
            $location.search('game-id', game.id);
            gameID = game.id;
            $http.get('/game/' + game.id + '/exchange-items').then(function (res) {
                $scope.gameList[index].isActive = true;
                $scope.selectedGame = $scope.gameList[index];
                $scope.gameList[index].gameExchange = res.data;
                $scope.scrollBigGuy = false;
                $scope.showAll = false;
            });
        } else {
            $location.search('game-id', null);
            $scope.scrollBigGuy = true;
            $scope.selectedGame.selected = false;
            $scope.selectedGame = null;
            $scope.showAll = true;
            selectedIndex = 0;
        }
    };
    $scope.selectExchange = function (exchange) {
        $scope.displayConfirm = true;
        $scope.selectedExchange = exchange;
    };

    var selectedIndex = 0;

    /**
     * [select game by up down key]
     * @param  {[Event]} e [up down key event]
     * @return {[void]} 
     */
    $scope.selectGame = function (e) {
        if ($scope.selectedGame) return;
        if (e.which === 13 && selectedIndex >= 0) {
            $scope.gameClickHandler($scope.gameList[selectedIndex]);
            selectedIndex = -1;
            $('#game-result').scrollTop(0);
        } else if (e.which === 40) {
            if (selectedIndex === $scope.gameList.length - 1) {
                return;
            }
            selectedIndex++;
            angular.forEach($scope.gameList, function (game) {
                game.selected = false;
            });
            $scope.gameList[selectedIndex].selected = true;
            selectedIndex > 3 && $('#game-result').scrollTop(62 + $('#game-result').scrollTop());
        } else if (e.which === 38) {
            if (selectedIndex === 0) {
                return;
            }
            selectedIndex--;
            angular.forEach($scope.gameList, function (friend) {
                friend.selected = false;
            });
            $scope.gameList[selectedIndex].selected = true;
            $('#game-result').scrollTop(62 * selectedIndex);
        }
    };

    $scope.submit = function () {
        var csrfToken = $("meta[name='csrf-token']").attr("content");
        $scope.showAll = false;
        $scope.selectedGame.isActive = false;
        $scope.showLoading = true;
        $scope.exchangeError = '';
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
                $scope.showLoading = false;
                $scope.showExchangeSuccess = true;
                setTimeout(function () {
                    window.location.reload();
                }, 1500);
            }
            else {
                if (!data || !data.errors) window.location.href = '/500';
                $scope.displayConfirm = false;
                $scope.showLoading = false;
                $scope.exchangeError = getErrorMessage(data.errors);
                $scope.selectedGame.isActive = true;
            }
        }).error(function (data, code) {
            if (!data || !data.errors) window.location.href = '/500';
            $scope.displayConfirm = false;
            $scope.showLoading = false;
            $scope.exchangeError = getErrorMessage(data.errors);
            $scope.selectedGame.isActive = true;
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
                    if ("success" in data) {
                        exchange.message = '';
                        $scope.selectedExchange = exchange;
                        if ($scope.selectedGame.gameExchange.Package.length > 0) $scope.displayConfirm = true;
                    }
                    else if ('errors' in data) {
                        $scope.displayConfirm = false;
                        exchange.message = getErrorMessage(data.errors);
                    } else {
                        exchange.message = '';
                    }
                }).error(function () {
                    if ('errors' in data) {
                        $scope.displayConfirm = false;
                        exchange.message = getErrorMessage(data.errors);
                    } else {
                        exchange.message = '';
                    }
                });
            }
            else if ('errors' in data) {
                exchange.message = getErrorMessage(data.errors);
            } else {
                exchange.message = '';
            }

        }).error(function (data) {
            if ('errors' in data) {
                exchange.message = getErrorMessage(data.errors);
            } else {
                exchange.message = '';
            }
        });
    };

    var getErrorMessage = function (errorsData) {
        var errorMessage = '';
        for (var field in errorsData) {
            if (errorsData.hasOwnProperty(field)) {
                errorMessage += errorsData[field];
            }
        }
        return errorMessage;
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

function slimScrollDirective() {
    return function (scope, element, attr) {
        $(element).perfectScrollbar();
        scope.$watch(attr.slimScroll, function (enable) {
            if (!enable) {
                $(element).perfectScrollbar('destroy');
            } else {
                $(element).perfectScrollbar();
            }
        });
    }
}

gtokenApp.directive('slimScroll', [slimScrollDirective]);
gtokenApp.controller('ExchangeCtrl', ['$scope', '$http', '$location', '$timeout', ExchangeCtrl]);
