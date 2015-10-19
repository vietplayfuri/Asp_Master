function SearchCtrl($http, $scope) {
    var self = this;
    $scope.users = [];
    $scope.keyword = '';
    $scope.searchResult = false;
    $scope.count = -1;
    $scope.noData = false;
    var timer;

    $scope.quickSearch = function(event) {
        if($scope.keyword.replace(" ","").length < 3) {
            $scope.count = 0;
            $scope.users = [];
            $scope.noData = false;
            return;
        }

        var keyCode = (event.keyCode ? event.keyCode : event.which);
        if(keyCode == 13){
            $scope.buttonSearch();
            return;
        }
        if(keyCode != 8 && (keyCode < 32 || keyCode > 127)){
            return;
        }
        
        if (timer){
            clearTimeout(timer);
        }
        timer = setTimeout(function(){
            $http({
                method: 'post',
                url: '/friend/quick-search',
                data: {term: $scope.keyword},
                headers: {
                    'Content-type': 'application/json;'
                }
            }).success(function(data) {
                $scope.searchResult = true;
                $scope.users = [];
		$scope.count = data.count;
		$scope.noData = false;
                if(data.count == 0){
                    $scope.noData = true;
                    return;
                } 

                for(var i=0;i<data.users.length;i++){
                    $scope.users.push(data.users[i]);

                    // var uid = data.users[i].uid;
                    // $('.friend-bar[data-uid='+uid+']').addClass('matched');
                }
            });
        }, 200);
    };

    $scope.buttonSearch = function(){
        if($scope.keyword.replace(" ","").length == 0) {
            return;
        }

        window.location.href= '/friend/search?term=' + $scope.keyword.split(' ').join('+');;
    }

    $scope.fullSearch = function(){
        var keyCode = (event.keyCode ? event.keyCode : event.which);
        if(keyCode != 13){
            return;
        }
        $scope.buttonSearch();
    }
}

function addFriendDirective($http) {
  var self = this;

  return {
    restrict: 'CA',
    replace: false,
    transclude: false,
    scope: true,
    link: function(scope, element, attrs) {
        scope.waiting = false;
        element.bind('click', function() {
            scope.waiting = true;
            $http({
                method: 'post',
                url: '/friend/add',
                data: {"friend": attrs.username},
                headers: {
                    'Content-type': 'application/json;'
                }
            }).success(function(data) {
            
            });
        });
    }
  }
}

function unFriendDirective($http) {

  return {
    restrict: 'CA',
    replace: false,
    transclude: false,
    scope: true,
    link: function(scope, element, attrs) {
        
        element.bind('click', function() {
            var self = $(element);
            parent = self.parents('div.friend-bar');
            self.remove()
                ;
            $http({
                method: 'post',
                url: '/friend/unfriend',
                data: {"friend": attrs.username},
                headers: {
                    'Content-type': 'application/json;'
                }
            }).success(function(data) {
                if(data.success === true){
                    parent.parent('li').hide('slow', function(){
                        $(this).remove();
                        $('span.friends-count').text(parseInt($('span.friends-count').text()) - 1);
                    });
                }
             });
        });
    }
  }
}

function acceptRequestDirective($http) {
  var self = this;

  return {
    restrict: 'CA',
    replace: false,
    transclude: false,
    scope: true,
    link: function(scope, element, attrs) {
        
        element.bind('click', function() {
            self = $(element);
            parent = self.parents('li');
            self.remove()
                ;
            $http({
                method: 'post',
                url: '/friend/accept',
                data: {"friend": attrs.username},
                headers: {
                    'Content-type': 'application/json;'
                }
            }).success(function(data) {
                if(data.success === true){
                    parent.hide('slow', function(){
                        $(this).remove();

                        if($('.request-panel').find('ul > li').length == 0){
                            $('.request-panel').remove();
                        }
                    });

                }
            });
        });
    }
  }
}

function removeRequestFriendDirective($http) {
  var self = this;

  return {
    restrict: 'CA',
    replace: false,
    transclude: false,
    scope: true,
    link: function(scope, element, attrs) {
        element.bind('click', function() {
            self = $(element);
            parent = self.parents('li');
            self.remove()
                    ;
            $http({
                method: 'post',
                url: '/friend/remove-request',
                data: {"friend": attrs.username},
                headers: {
                    'Content-type': 'application/json;'
                }
            }).success(function(data) {
                if(data.success === true){
                    parent.hide('slow', function(){
                        $(this).remove();
                        $('span.request-count').text(parseInt($('span.request-count').text()) - 1);

                        if($('.request-panel').find('ul > li').length == 0){
                                $('.request-panel').remove();
                        }
                    });
                }
            });
        });
    }
  }
}

gtokenApp.directive('removeRequestFriendDirective', ['$http', removeRequestFriendDirective]);
gtokenApp.directive('acceptRequestDirective', ['$http', acceptRequestDirective]);
gtokenApp.directive('unFriendDirective', ['$http', unFriendDirective]);
gtokenApp.directive('addFriendDirective', ['$http', addFriendDirective]);
gtokenApp.controller('SearchCtrl', ['$http', '$scope', SearchCtrl]);

$(document).ready(function(){

    $('.close-panel').click(function(){
        $('.transfer-panel').addClass('hide');
        $('.search-panel').removeClass('hide');
        $('.request-panel').removeClass('hide');
        $('.friend-panel').removeClass('hide');
    });
    $('.close-notification').click(function() {
        $(".transfer-panel .notification-panel").addClass("hide");
        $(".transfer-panel .panel-filter").addClass("hide");
    });
});

$(document).on('click', '.act-transfer',function(){
        $('.transfer-panel').removeClass('hide');
        $('#friendusername').html($(this).attr('data-username'));
        $('#friendname').html($(this).attr('data-name'));
        $('#receiverIdHiddenInput').val($(this).attr('data-id'));
        $('#friendavatar').attr("src", $(this).attr('data-avatar'));
        $('.search-panel').addClass('hide');
        $('.request-panel').addClass('hide');
        $('.friend-panel').addClass('hide');
    });
