var isTabActive = true;
window.onfocus = function () {
  isTabActive = true; 
}; 

window.onblur = function () {
  isTabActive = false; 
};

function WebSocketService($cookieStore, $rootScope) {
  //var websocket = chatWebSocket;

  //var self = this;
  //var socketTimer = 0;

  //if (typeof dcodeIO === 'undefined' || !dcodeIO.ProtoBuf) {
  //  throw (new Error("ProtoBuf.js is not present. Please see www/index.html for manual setup instructions."));
  //}
  //// Initialize ProtoBuf.js
  //var ProtoBuf = dcodeIO.ProtoBuf;
  //var ChatProto = ProtoBuf.loadProtoFile("/static/js/chatProtocol.proto").build("gtoken");
  //var MessageSend = ChatProto.MessageSendProto;
  //var MessageReceive = ChatProto.MessageReceiveProto;
  //var Command = ChatProto.Command;

  
  //self.token = $cookieStore.get('chat_token');
  //self.username = angular.fromJson($cookieStore.get('username'));
  //self.isLogin = $('#isLogin').length;

  //var socket = null;

  //function socketconnect() {

  //  if (socket != null && socket.readyState === WebSocket.OPEN) {
  //    return;
  //  }
  //  socket = new WebSocket(websocket);
  //  socket.binaryType = "arraybuffer"; // We are talking binary

  //  socket.onmessage = function(event) {
  //    try {
  //      // Decode the Message
  //      var msg = MessageReceive.decode(event.data);

  //      switch (msg.command) {
  //        case 5:
  //          $rootScope.$broadcast('NotificationChatMessageEvent', msg);
  //          $rootScope.$broadcast('ReceiveMessageEvent', msg);
  //          break;
  //        case 7:
  //          $rootScope.$broadcast('FriendListEvent', msg.friendlist);
  //          break;
  //        case 9:
  //        case 10:
  //          $rootScope.$broadcast('LoginEvent', msg);
  //          break;
  //        case 14:
  //          $rootScope.$broadcast('HistoryEvent', msg.history);
  //          break;
  //        case 15:
  //          $rootScope.$broadcast('TypingEvent', msg);
  //          break;
  //        case 18:
  //          $rootScope.$broadcast('NotificationChatEvent', msg.chatNotification);
  //          break;
  //        case 19:
  //          $rootScope.$broadcast('NewFriendEvent', msg.friendlist);
  //          break;
  //      }

  //    } catch (error) {
  //    }
  //  }

  //  socket.onerror = function(event) {
  //  }

  //  socket.onclose = function(event) {
  //    try {
  //      if (!self.socketTimer) {
  //        /* avoid firing a new setInterval, after one has been done */
  //        self.socketTimer = setInterval(function() {
  //          socketconnect();
  //        }, 3000);
  //      }
  //      /* that way setInterval will be fired only once after loosing connection */
  //      ;

  //    } catch (error) {
  //    }
  //  }

  //  socket.onopen = function() {
  //    setInterval(function() {
  //      if (socket.readyState === WebSocket.OPEN) {
  //        socket.send("Ping");
  //      }
  //    }, 1000 * 30);

  //    /* as what was before */
  //    if (self.socketTimer) { /* a setInterval has been fired */
  //      clearInterval(self.socketTimer);
  //      self.socketTimer = 0;
  //      self.login();
  //      var windowsNames = $cookieStore.get('chat_windows');
  //      var elms = angular.element(document.getElementsByClassName('chat-window'));

  //      for (var i = 0; i < elms.length; i++) {
  //        var username = elms.attr('data-username');
  //        var elm_msg = elms.find('.chat-message span[data-idx]');
  //        if($(elm_msg).length == 0) {
  //          continue;
  //        }
  //        var window = $(elm_msg).scope().findChatWindow(username);
  //        if(window == null || window.lastMsgKey == null || window.lastMsgKey.length === 0 || window.lastMsgStatus !== 2){
  //          continue;
  //        }
  //        self.getHistory(username, 1, window.lastMsgKey);
  //      }
  //    }
  //  }
  //}

  //self.isLogin && socketconnect();

  //this.sendMessageOnSocket = function(callback) {
  //  setTimeout(
  //    function() {
  //      if (socket.readyState === 1) {
  //        callback();
  //        return;
  //      } else if (socket.readyState === 3) {
  //        socketconnect();
  //        self.sendMessageOnSocket(callback);
  //      } else {
  //        self.sendMessageOnSocket(callback);
  //      }
  //    }, 5); // wait 5 milisecond for the connection...
  //}

  //this.getFriendList = function() {
  //  self.sendMessageOnSocket(function() {
  //    if (socket.readyState === WebSocket.OPEN) {
  //      var msg = new MessageSend({
  //        "command": "FRIENDS",
  //        "user": self.username,
  //        "token": self.token
  //      });
  //      var buffer = msg.encode();
  //      socket.send(buffer.toArrayBuffer());
  //    } else {
  //    }
  //  });
  //}

  //this.sendInvitation = function(friendUsername, game) {
  //  self.sendMessageOnSocket(function() {
  //    if (socket.readyState === WebSocket.OPEN) {
  //      var msg = new MessageSend({
  //        "command": "CHAT",
  //        "user": self.username,
  //        "token": self.token,
  //        "friend": friendUsername,
  //        "type": "GAME",
  //        "game": {
  //          "sender": self.username,
  //          "receiver": friendUsername,
  //          "id": game.id.toString(),
  //          "title": game.name,
  //          "thumb": game.icon_filename,
  //          "link": game.web_link
  //        }
  //      });
  //      var buffer = msg.encode();
  //      socket.send(buffer.toArrayBuffer());
  //    } else {
  //    }
  //  });
  //}

  //this.sendMessage = function(friendUsername, message) {
  //  self.sendMessageOnSocket(function() {
  //    if (socket.readyState === WebSocket.OPEN) {
  //      var msg = new MessageSend({
  //        "command": "CHAT",
  //        "user": self.username,
  //        "token": self.token,
  //        "friend": friendUsername,
  //        "data": message
  //      });
  //      var buffer = msg.encode();
  //      socket.send(buffer.toArrayBuffer());
  //    } else {
  //    }
  //  });
  //}

  //this.typing = function(friendUsername) {
  //  self.sendMessageOnSocket(function() {
  //    if (socket.readyState === WebSocket.OPEN) {
  //      var msg = new MessageSend({
  //        "command": "TYPING",
  //        "user": self.username,
  //        "token": self.token,
  //        "friend": friendUsername
  //      });
  //      var buffer = msg.encode();
  //      socket.send(buffer.toArrayBuffer());
  //    } else {
  //    }
  //  });
  //}

  //this.getHistory = function(friendUsername, currentPage, fromId) {
  //  self.sendMessageOnSocket(function() {
  //    if (socket.readyState === WebSocket.OPEN) {
  //      var msg = new MessageSend({
  //        "command": "HISTORY",
  //        "user": self.username,
  //        "token": self.token,
  //        "friend": friendUsername,
  //        "current": currentPage,
  //        "fromId": fromId
  //      });
  //      var buffer = msg.encode();
  //      socket.send(buffer.toArrayBuffer());
  //    } else {
  //    }
  //  });
  //}

  //this.login = function() {
  //  self.sendMessageOnSocket(function() {
  //    if (socket.readyState === WebSocket.OPEN) {
  //      var msg = new MessageSend({
  //        "command": "LOGIN",
  //        "user": self.username,
  //        "token": self.token
  //      });
  //      var buffer = msg.encode();
  //      socket.send(buffer.toArrayBuffer());
  //    } else {
  //    }
  //  });
  //}


  //this.notification = function(friend, action) {
  //  self.sendMessageOnSocket(function() {
  //    if (socket.readyState === WebSocket.OPEN) {
  //      var msg = new MessageSend({
  //        "command": "NOTIFICATION",
  //        "user": self.username,
  //        "token": self.token,
  //        "friend": friend,
  //        "notifyAction": action
  //      });
  //      var buffer = msg.encode();
  //      socket.send(buffer.toArrayBuffer());
  //    } else {
  //    }
  //  });
  //}

  //this.setChatRead = function(friendUsername, lastId) {
  //  self.sendMessageOnSocket(function() {
  //    if (socket.readyState === WebSocket.OPEN) {
  //      var msg = new MessageSend({
  //        "command": "CHAT_STATUS",
  //        "user": self.username,
  //        "friend": friendUsername,
  //        "token": self.token,
  //        "fromId": lastId
  //      });
  //      var buffer = msg.encode();
  //      socket.send(buffer.toArrayBuffer());
  //    } else {
  //    }
  //  });
  //}

}


//function MessengerService() {
//  var self = this;
//  self.messengers = null;
//  self.typings = null;

//  self.getMessenger = function(username) {
    
//      self.messengers = {
//        'window': false,
//        'focus': false,
//        'lastMsgStatus': 2, //unread,
//        'lastMsgKey': '',
//        'messages': [],
//        'chatMessage': '',
//        'currentPage': 1
//      }
//    return self.messengers;
//  }

//  self.getTyping = function(username){
//    self.typings = {
//      'user': {},
//      'chat': {
//        'message': '•••',
//        'timestamp': new Date().getTime() ,
//        'typing': true
//      },
//      'type': 1,
//      'typing': true
//    }
//    return self.typings;
//  }

//}

function LoginService($rootScope, WebSocketService) {
  //var self = this;
  //self.isLogin = false;

  //if (!self.isLogin) {
  //  WebSocketService.login();
  }

//  $rootScope.$on('LoginEvent', function(event, data) {
//    if (data.command === 9) {
//      self.isLogin = true;
//    } else {
//      self.isLogin = true;
//    }
//  });

//  this.onLoginAsync = function(callback) {
//    setTimeout(
//      function() {
//        if (self.isLogin) {
//          callback();
//          return;
//        } else {
//          self.onLoginAsync(callback);
//        }
//      }, 5); // wait 5 milisecond for the connection...
//  }

//}

//function FriendService($http, $rootScope, WebSocketService) {
//  var self = this;
//  self.friends = {};
//  self.friendChatList = {};
//  self.ready = false;

//  this.getFriendList = function(term) {
//    $http({
//      method: 'post',
//      url: '/friend/friendlist',
//      headers: {
//        'Content-type': 'application/json;',
//      },
//    }).success(function(data) {
//      self.friends = data;
//      for (var username in self.friends) {
//        self.friends[username]['status'] = 'offline';
//      }
//      WebSocketService.getFriendList();
//    });
//  }

//  this.searchFriendForChat = function (keyword) {
//    return $http({
//      method: 'post',
//      url: '/friend/search-friend',
//      data: {'term': keyword || ''},
//      headers: {
//          'Content-type': 'application/json;'
//      }
//    }).then(function (res) {
//      $rootScope.friendChatList = res.data;
//      return $rootScope.friendChatList;
//    });
//  }

//  if (Object.keys(self.friends).length === 0) {
//    this.getFriendList();
//  }

//  $rootScope.onlineCounter = 0;
//  $rootScope.$on('FriendListEvent', function(event, data) {
//    for (index = 0; index < data.length; index++) {
//      username = data[index]['user'];
//      if (username in self.friends) {
//        var status = "offline";
//        switch (data[index]['status']) {
//          case 1:
//            status = 'online';
//            if (status !== self.friends[username]['status']) {
//              $rootScope.onlineCounter++;
//            }
//            break;
//          case 2:
//            status = 'offline';
//            if (status !== self.friends[username]['status']) {
//              $rootScope.onlineCounter--;
//            }
//            break;
//        }

//        //if (self.ready) {
//        //    $rootScope.showNotification(self.friends[username]['avatar'], username + " is " + status);
//        //}
//        self.friends[username]['status'] = status;
//        if (status === 'offline') {
//          $rootScope.moveOfflineToBottom(username);
//        } else {
//          $rootScope.moveOnlineToTop(username);
//        }
//      }
//    }
//    self.ready = true;
//    $rootScope.$apply();
//  });
  
//  $rootScope.moveOnlineToTop = function (username) {
//    if (!$rootScope.friendChatList || $rootScope.friendChatList.length === 1) return;
//    var currentOnline = $rootScope.friendChatList.filter(function (f) { return f.account === username; })[0];
//    var currentOnlineIndex = $rootScope.friendChatList.indexOf(currentOnline);
//    $rootScope.friendChatList.splice(currentOnlineIndex, 1);
//    $rootScope.friendChatList.splice(0, 0, currentOnline);
//  }

//  $rootScope.moveOfflineToBottom = function (username) {
//    if (!$rootScope.friendChatList || $rootScope.friendChatList.length === 1) return;
//    var current = $rootScope.friendChatList.filter(function (f) { return f.account === username; })[0];
//    if(!current) return;
//    var currentIndex = $rootScope.friendChatList.indexOf(current);
//    $rootScope.friendChatList.splice(currentIndex, 1);
//    $rootScope.friendChatList.push(current);
//  }

//  $rootScope.$on('NewFriendEvent', function(event, data) {
//    self.getFriendList();
//  });

//  this.getFriendAsync = function(username, callback) {
//    setTimeout(
//      function() {
//        if (self.ready) {
//          var friend = self.getFriend(username);
//          callback(friend);
//          return;
//        } else {
//          self.getFriendAsync(username, callback);
//        }
//      }, 5); // wait 5 milisecond for the connection...
//  }


//  this.getFriend = function(username) {
//    return username in self.friends ? self.friends[username] : null;
//  }
//}

//function GameService($http, $rootScope) {
//  var self = this;
//  self.games = {}

//  this.getGames = function(keyword, callback) {
//    //if (Object.keys(self.games).length === 0) {
//      $http({
//        method: 'post',
//        url: '/game/game-list',
//        data: {
//          keyword: keyword
//        },
//        headers: {
//          'Content-type': 'application/json;',
//        },
//      }).success(function(data) {
//        self.games = data['games'];
//        callback(self.games);
//      });
//    //} else {
//    //  callback(self.games);
//    //}
//  }
//}


//function userStatusDirective($cookieStore, $rootScope, LoginService, FriendService) {
//  var self = this;

//  this.link = function(scope, element, attrs) {
//    self.scope = scope;
//    self.element = element;
//    self.attrs = attrs;

//    LoginService.onLoginAsync(function() {
//      FriendService.getFriendAsync(attrs.username, function(friend) {
//        if (!friend) return;
//        scope.friend = friend;
//        if (friend.status === 'offline') {
//          $rootScope.moveOfflineToBottom(friend.account);
//        } else {
//          $rootScope.moveOnlineToTop(friend.account);
//        }
//      });   
//    });
//  }
//  return {
//    scope: true,
//    link: link
//  }
//}

function chatButtonDirective($cookieStore, $rootScope, LoginService, FriendService) {
  var self = this;

  // KNN should use its own template and doesn't need html in index.html
  this.link = function(scope, element, attrs) {
    self.scope = scope;
    self.element = element;
    self.attrs = attrs;

    //scope.chatButtonClicked = function() {
    //  $rootScope.$broadcast('OpenChatWindowEvent', attrs.username);
    //}

    //scope.chatButtonClickedByUserName = function(username) {
    //  $rootScope.$broadcast('OpenChatWindowEvent', username);
    //  $rootScope.$broadcast('NotificationResetCountEvent', username);
    //}

    //LoginService.onLoginAsync(function() {
    //  FriendService.getFriendAsync(attrs.username, function(friend) {
    //    if (!friend) return;
    //    scope.friend = friend;
    //    $rootScope.$apply();
    //  });   
    //});

    //var interval = setInterval(function() {
    //  if (!LoginService.isLogin) {
    //    return;
    //  }

    //  if (LoginService.isLogin && FriendService.ready) {
    //    scope.friend = FriendService.getFriend(attrs.username);

    //    clearInterval(interval);
    //    $rootScope.$apply();
    //  }
    //}, 5);
  }

  return {
    scope: true,
    link: link
  }
}

//function MessengerCtrl($scope, $rootScope, $cookieStore, FriendService, WebSocketService, MessengerService, GameService, NotificationService) {
//  var self = this;
//  self.username = angular.fromJson($cookieStore.get('username'));
//  $scope.username = self.username;

//  // KNN why can't use controllerAs
//  $scope.windows = [];
//  $scope.hiddenWindows = [];
//  $scope.games = [];
//  // minimize friend list
//  $scope.isChatListMinimized = $cookieStore.get('isChatListMinimized');
//  $scope.minimizeFriendList = function () {
//    $scope.isChatListMinimized = !$scope.isChatListMinimized;
//    $cookieStore.put('isChatListMinimized', $scope.isChatListMinimized);
//  }

//  // maximum number of available chat windows on screen
//  var width = $(window).width();
//  $scope.chatWindowNumber = width > 550 && Math.floor((width - 550)/260) || 1;
//  $(window).on('resize.chatWindowNumber', function () {
//    var width = $(this).width();
//    $scope.chatWindowNumber = width > 550 &&  Math.floor((width - 550)/260) || 1;
//    if ($scope.windows.length > $scope.chatWindowNumber) {
//      // move from window pile to hidden window pile
//      for (var i = 0, j= $scope.windows.length - $scope.chatWindowNumber; i < j; i++) {
//        $scope.hiddenWindows.push($scope.windows.pop());
//      }
//    } else if ($scope.windows.length < $scope.chatWindowNumber && $scope.hiddenWindows.length) {
//      // move from hidden window pile to window pile
//      for (var i = 0, j = $scope.chatWindowNumber - $scope.windows.length; i < j; i++) {
//        $scope.windows.push($scope.hiddenWindows.pop());
//      }
//    }
//    !$scope.$$phase && $rootScope.$apply();
//  });

//  $scope.toDateTime = function(timestamp){
//    var today = new Date();
//    var day = today.getDate();
//    var month = today.getMonth()+1; //January is 0!
//    var year = today.getFullYear();

//    var currentTimestamp = new Date(year + "-" + month + "-" + day + " 00:00:01").getTime();
//    if(timestamp < currentTimestamp){
//      return moment(new Date(timestamp)).format("MMM DD");
//    }
//    return moment(new Date(timestamp)).format("hh:mm a");
//  }

//  GameService.getGames('', function(data) {
//    $scope.games = data;
//  });

//  $scope.searchGame = function(event, friend){
//    var keyword = event.srcElement || event.target;
//    if (!event.shiftKey && $.trim(keyword.value) !== '') {
//      GameService.getGames(keyword.value, function(data) {
//        var window = $scope.findChatWindow(friend);
//        window.games = data;
//      });
//    }
    
//  }

//  $scope.findChatWindow = function(username) {
//    return $scope.windows.concat($scope.hiddenWindows).filter(function (w) {
//      return w.friend.account === username;
//    }).shift();
//  };

//  $scope.popChatFromHiddenList = function (hiddenWindow) {
//    $scope.windows.push(hiddenWindow);
//    $scope.hiddenWindows.splice($scope.hiddenWindows.indexOf(hiddenWindow), 1);
//    hiddenWindow.isMinimized = false;
//    hiddenWindow.focus = true;
//    if ($scope.windows.length > 2) {
//      var windowToHide = $scope.windows[$scope.windows.length - 2];
//      $scope.hiddenWindows.push(windowToHide);
//      $scope.windows.splice($scope.windows.length - 2, 1);
//    }
//  };

//  $scope.minimizeWindow = function (currWindow) {
//    currWindow.isMinimized = !currWindow.isMinimized;
//    var windows = $cookieStore.get('chat_windows');
//    var windowInCookie = windows.filter(function (w) {
//      return w.username === currWindow.friend.account;
//    }).pop();
//    if (windowInCookie) windowInCookie.isMinimized = currWindow.isMinimized;
//    $cookieStore.put('chat_windows', windows);
//  }

//  $scope.showChatWindow = function(username, isMinimized) {
//    var window = $scope.hiddenWindows.filter(function (w) {
//      return w.friend.account === username;
//    }).shift();

//    if (window) {
//      $scope.windows.push(window);
//      $scope.hiddenWindows.splice($scope.hiddenWindows.indexOf(window), 1);
//    } else {
//      var existing = false;
//      window = $scope.findChatWindow(username);

//      if (window == null) {
//        window = MessengerService.getMessenger(username);
//        window['friend'] = FriendService.getFriend(username);

//        if(window['friend'] === null){
//          return;
//        }
//        $scope.windows.push(window);
//        WebSocketService.getHistory(username, 1, "0");
//        var windowsName = $cookieStore.get('chat_windows') || [];
//        var windowName = windowsName.filter(function (w) {
//          return w.username === username;
//        }).pop();
//        if (windowName) {
//          windowName.isMinimized = isMinimized;
//        } else {
//          windowsName.push({username: username, isMinimized: false});
//        }
//        $cookieStore.put('chat_windows', windowsName);
//        if(!$scope.$$phase) {
//          $rootScope.$apply();
//        }
//      }

//      window.window = true; // Boolean, true means the window should display
//      window.typing = false; // typing of friend
//      window.mytyping = false; // the flag for anti spam server when I'm typing
//      window.invitegame = false;
//      window.blinking = false;
//      if (isMinimized != null) window.isMinimized = isMinimized;
//      if (window.timeoutTyping === 'undefined') {
//        window.timeoutTyping = 0; //variable of setTimeout to set window.typing = false;
//      }
      
//    }

//    if ($scope.windows.length > $scope.chatWindowNumber) {
//      var windowToHide = $scope.windows[$scope.windows.length - 2];
//      $scope.hiddenWindows.push(windowToHide);
//      $scope.windows.splice($scope.windows.indexOf(windowToHide), 1);
//    }
//    return window;
//  };

//  $scope.showFriendPile = false;

//  $scope.togglePile = function () {
//    $scope.showFriendPile = !$scope.showFriendPile;
//  };

//  $scope.showPile = function () {
//    $scope.showFriendPile = true;
//  };

//  $scope.closeChatWindow = function(username) {
//    var window = $scope.findChatWindow(username);
//    window.chatMessage = '';
//    if (window == null) {
//      return;
//    }
//    var toRemoveIndex = $scope.windows.indexOf(window);
//    toRemoveIndex >=0 && $scope.windows.splice(toRemoveIndex, 1);

//    toRemoveIndex = $scope.hiddenWindows.indexOf(window);
//    toRemoveIndex >=0 && $scope.hiddenWindows.splice(toRemoveIndex, 1);

//    //window.window = false;
//    var windowsName = $cookieStore.get('chat_windows');
//    var windowInCookie = windowsName.filter(function (w) {
//      return w.username === username;
//    });
//    if (windowsName !== undefined && windowInCookie) {
//      var index = windowsName.indexOf(windowInCookie);
//      windowsName.splice(index, 1);
//    }
//    $cookieStore.put('chat_windows', windowsName);

//    if ($scope.windows.length < $scope.chatWindowNumber && $scope.hiddenWindows.length) {
//      $scope.windows.push($scope.hiddenWindows[0]);
//      $scope.hiddenWindows.splice(0, 1);
//    }
//  };

//  $scope.openInviteGameWindow = function(username) {
//    var window = $scope.findChatWindow(username);
//    if (window == null) {
//      return;
//    }
//    window.games = $scope.games;
//    window.invitegame = !window.invitegame;
//  };

//  function getMessage(obj) {
//    if (!obj) return null;
//    if(obj.chat != null){
//      return obj.chat;
//    } else if(obj.game != null){
//      return obj.game;
//    } else {
//      return null;
//    }
//  }

//  function pushMessage(username, data){
//    var msgObj = getMessage(data);
//    if(msgObj == null){
//      return;
//    }
//    var window = $scope.findChatWindow(username);
//    if(window == null){
//      return;
//    }

//    window = $scope.showChatWindow(username);
//    if(window.messages.length == 0){
//      window.messages.push(data);
//      return window;
//    }

//    var msgFormLastObj = getMessage(window.messages[window.messages.length - 1]);

//    window.typing = false;
//    if(msgFormLastObj.typing !== undefined){
//      window.messages.splice(window.messages.length -1, 1);
//    }

//    var msgFormFirstObj = getMessage(window.messages[0]);
//    msgFormLastObj = getMessage(window.messages[window.messages.length - 1]);
//    var lastTime = msgFormLastObj.timestamp;
//    var firstTime = msgFormFirstObj.timestamp;
//    var timestamp = msgObj.timestamp;

//    if(timestamp < firstTime){
//      window.messages.unshift(data);
//    } else if(timestamp > lastTime){
//      window.messages.push(data);
//    } else if(timestamp === lastTime || timestamp === firstTime){
//      return window;
//    } else {
//      for(var i = window.messages.length - 1;i>0;i--){
        
//        var msgPreviousObj = getMessage(window.messages[i-1]);
//        var msgCurrentObj = getMessage(window.messages[i]);
//        var previousTime = msgPreviousObj.timestamp;
//        var currentTime = msgCurrentObj.timestamp;

//        if(currentTime > timestamp && 
//          previousTime < timestamp) {
//          window.messages.splice(i, 0, data);
//          break;
//        }
//      }
//    }

//    return window;
//  }

//  $scope.onFocusWindow = function(username) {
//    var text = "";
//    if (window.getSelection) {
//      text = window.getSelection().toString();
//    } else if (document.selection && document.selection.type != "Control") {
//      text = document.selection.createRange().text;
//    }

//    if(text.length == 0){
//      $scope.chatFocus(null, username);
//    }
//  };

//  //show chat windows from cookies

//  var interval = setInterval(function() {
//    if (FriendService.ready) {
//      clearInterval(interval);
//      var windowNames = $cookieStore.get('chat_windows') || [];
//      if (windowNames.length === 0) return;
//      for (var i = 0, j = windowNames.length; i < j; i++) {
//        $scope.showChatWindow(windowNames[i].username, windowNames[i].isMinimized);
//      }
//    }
//  }, 10);

//  $rootScope.showNotification = function(icon, text, title, tag) {
//    if (!("Notification" in window)) {
//      return; // This browser does not support desktop notification
//    } else if (Notification.permission === "granted") {
//      var options = {
//        body: text,
//        icon: icon,
//        dir: "ltr",
//        tag : tag
//      };
//      var notification = new Notification(title || "", options);
//      setTimeout(function() {
//        notification.close();
//      }, 3000);
//      notification.onclick = function () {
//        $(window).trigger('focus');
//      }
//    } else if (Notification.permission !== 'defined') {
//      Notification.requestPermission(function(permission) {
//        if (!('permission' in Notification)) {
//          Notification.permission = permission;
//        }
//      });
//    }
//  };

//  $rootScope.$on('OpenChatWindowEvent', function(event, username) {
//    var window = $scope.showChatWindow(username);
//    window.isMinimized = false;
//    $scope.chatFocus(null, username);
//  });

//  var alertChat = (function() {
//    if($rootScope.titlePage == null || $rootScope.titlePage.length == 0){
//      $rootScope.titlePage = document.title;
//      $rootScope.messageCount = 0;
//    }

//    var timeoutId;
//    var blink = function(username) {
//      var msg = username + " messaged you";
//      document.title = document.title == msg ? "("+ $rootScope.messageCount + ") " + $rootScope.titlePage : msg;
//    };
//    var clear = function() {
//      clearInterval(timeoutId);
//      document.title = $rootScope.titlePage;
//      window.onmousemove = null;
//      timeoutId = null;
//      $rootScope.messageCount = 0;
//    };
//    return function(username) {
//      $rootScope.messageCount = $rootScope.messageCount + 1;
//      if (!timeoutId) {
//        timeoutId = setInterval(function(){blink(username)}, 1200);
//        window.onmousemove = clear;
//      }
//    };
//  }());

//  $rootScope.$on('ReceiveMessageEvent', function(event, data) {
//    var username = '';
//    var sender = '';
//    var msgObj = getMessage(data);
//    username = msgObj.sender == self.username ? msgObj.receiver : msgObj.sender;
//    sender = msgObj.sender;

//    data.user = FriendService.getFriend(sender);
//    if(data.user == null){
//      return;
//    }
//    if(username === data.user.account && msgObj.status !== 1){
//      //Blink window   
//      alertChat(sender);
//    }

//    var window = pushMessage(username, data);
//    if(window == null){
//      return;
//    }

//    if(username === sender && msgObj.status !== 1){
//      window.blinking = true;
//    }
    
//    if(username !== self.username){
//      var msgObj = getMessage(data);
//      window.lastMsgKey = msgObj.key;
//      window.lastMsgStatus = msgObj.status;
//      if(window.focus){
//        $scope.chatFocus(null, username);
//      }
//    } 
    
//    $rootScope.$apply();
//  });

//  $rootScope.$on('HistoryEvent', function(event, data) {
//    if (data.length == 0) {
//      return;
//    }

//    var addAll = false;
//    var msgReceiveFirstObj = getMessage(data[0]);
//    var msgReceiveLastObj = getMessage(data[data.length - 1]);
//    var username = msgReceiveFirstObj.sender == self.username ? msgReceiveFirstObj.receiver : msgReceiveFirstObj.sender;

//    //Scroll bar
//    var elmMessage = $('div[data-username='+username+']').find('.c-message');
//    var elmFirst = 0;

//    var window = $scope.showChatWindow(username);

//    try {

//      if(window.messages.length > 0){
//        var msgFormFirstObj = getMessage(window.messages[0]);
//        var msgFormLastObj = getMessage(window.messages[window.messages.length - 1]);

//        //check history in first or previous load
//        if (window.messages.length == 0 || (msgReceiveLastObj !== null && msgReceiveLastObj.timestamp < msgFormFirstObj.timestamp) ||
//            (msgReceiveLastObj !== null && msgReceiveLastObj.timestamp < msgFormFirstObj.timestamp)) {
//          for (var i = data.length - 1; i >= 0; i--) {
//            data[i].user = FriendService.getFriend(getMessage(data[i]).sender);
//            pushMessage(username, data[i]);
//          }

//          if(data.length > 0 && username !== self.username){
//            window.lastMsgKey = msgFormLastObj.key;
//            window.lastMsgStatus = msgFormLastObj.status;
//          } 
//          $rootScope.$apply();
//          return;
//        }
//      }

//      //sync data when it miss match with server
//      for (var i = 0; i < data.length; i++) {
//        data[i].user = FriendService.getFriend(getMessage(data[i]).sender);
//        pushMessage(username, data[i]);
//      }
//      $rootScope.$apply();

//      var msgFormFirstObj = getMessage(window.messages[0]);
//      var msgFormLastObj = getMessage(window.messages[window.messages.length - 1]);
//      if(data.length > 0 && username !== self.username){
//        window.lastMsgKey = msgFormLastObj.key;
//        window.lastMsgStatus = msgFormLastObj.status;
//      } 
      
//    } finally {
//      window.loading = false;

//      if(elmMessage.length > 0){
//        elmFirst = elmMessage[0];
//        var scrollObj = $('div[data-username='+username+']').find('.chat-content')[0];
//        scrollObj.scrollTop = $(elmFirst).position().top - scrollObj.offsetHeight - $(elmFirst).parent('.message-container').height();
//      }
//    }
    
//  });

//  $rootScope.$on('TypingEvent', function(event, data) {
//    var username = data.user;
//    var window = $scope.findChatWindow(username);
//    if (window == null || !window.window) {
//      return;
//    }
    
//    if(!window.typing){
//      window.typing = true;
//      $scope.addTyping(username);
//    }

//    clearTimeout(window.timeoutTyping);
//    window.timeoutTyping = setTimeout(function() {
//      window.typing = false;
//      $scope.removeTyping(username);
//      $rootScope.$apply();
//    }, 7000, username);
//    $rootScope.$apply();
//  });

//  $scope.addTyping = function(username){
//    var typingObj = MessengerService.getTyping();
//    typingObj.user = FriendService.getFriend(username);
//    var window = $scope.findChatWindow(username);
//    if(window.messages.length && window.messages[window.messages.length - 1].typing !== undefined){
//      return;
//    }

//    window.messages.push(typingObj);
    
//    $rootScope.$apply();
//  }

//  $scope.removeTyping = function(username){
//    var window = $scope.findChatWindow(username);
//    if(window.messages[window.messages.length - 1].typing !== undefined){
//      window.messages.splice(window.messages.length -1, 1);
//      $rootScope.$apply();
//    }
    
//  }

//  $scope.sendInvitation = function(event, friendUsername, game) {
//    WebSocketService.sendInvitation(friendUsername, game);
//    // After sending the invitation, dispose the modal
//    var window = $scope.findChatWindow(friendUsername);
//    window.invitegame = !window.invitegame;
//  };

//  $scope.sendTyping = function(event, friendUsername) {
//    var textarea = event.srcElement || event.target;
//    var window = $scope.findChatWindow(friendUsername);

//    if (window.mytyping == false) {
//      if (textarea.value !== '') {
//        WebSocketService.typing(friendUsername);
//      }
//      setTimeout(function() {
//        window.mytyping = false;
//      }, 6000);
//    }

//    window.mytyping = true;
//  };

//  $scope.sendMessage = function(event, friendUsername) {
//    var textarea = event.srcElement || event.target;
//    var window = $scope.findChatWindow(friendUsername);

//    if ($.trim(textarea.value) === '' && event.keyCode === 13) {
//      textarea.value = '';
//      event.preventDefault();
//      return;
//    }
//    if (event.keyCode === 13 && !event.shiftKey && $.trim(textarea.value) !== '') { // Enter key, send message
//      event.preventDefault();
//      window.mytyping = false;

//      textarea.value = $.trim(textarea.value);
//      if (textarea.value) {
//        WebSocketService.sendMessage(friendUsername, textarea.value);
//        textarea.value = '';
//        window.chatMessage = '';
//        event.preventDefault();
//        $scope.resizeTextArea(event);
//      }
//    }
//  };

//  var hasExpanded = false, scrollHeight;
//  $scope.resizeTextArea = function (e) {
//    e = e || window.event;
//    var textArea = e.srcElement || e.target;
//    scrollHeight = textArea.scrollHeight;
//    textArea.style.height = 'auto';
//    if (!hasExpanded && textArea.scrollHeight === 34) {
//      textArea.style.height = null;
//      return;
//    }
//    if (textArea.scrollHeight > 48 && textArea.scrollHeight <= 64) {
//      hasExpanded = true;
//      $(textArea).parent()[0].style.height = textArea.scrollHeight - 8 + 'px';
//      $(textArea).parent().prev()[0].style.height = 266 - textArea.scrollHeight + 16 + 'px';
//      textArea.style.height = textArea.scrollHeight - 16 + 'px';
//    } else if (textArea.scrollHeight <= 48) {
//      textArea.style.height = null;
//      if (hasExpanded) {
//        $(textArea).parent()[0].style.height = null;
//        $(textArea).parent().prev()[0].style.height = null;
//      }
//    } else if (textArea.scrollHeight > 64) {
//      $(textArea).parent()[0].style.height = '55px';
//      $(textArea).parent().prev()[0].style.height = '215px';
//      textArea.style.height = '53px';
//    }
//    if (textArea.scrollHeight === 56) {
//      textArea.style.height = '45px';
//      $(textArea).parent()[0].style.height = textArea.scrollHeight - 8 + 'px';
//      $(textArea).parent().prev()[0].style.height = 266 - textArea.scrollHeight + 12 + 'px';
//    }
//    (e.keyCode < 37 || e.keyCode > 40) && e.keyCode !== 35 && e.keyCode !== 36 && $(textArea).scrollTop(scrollHeight);
//  };

//  $scope.chatFocus = function(event, friendUsername) {
//    var window = $scope.findChatWindow(friendUsername);
//    $scope.windows.concat($scope.hiddenWindows).forEach(function (w) {
//      if (w) w.focus = false;
//    });
//    window.focus = true;

//    if(window.blinking){
//      window.blinking = false;
//    }

//    $rootScope.$broadcast('NotificationCountDecreaseEvent', friendUsername);

//    if (window.lastMsgKey && window.lastMsgKey.length == 0 || window.lastMsgStatus !== 2) {
//      return;
//    }
    
//    WebSocketService.setChatRead(friendUsername, window.lastMsgKey);
//    $rootScope.$broadcast('NotificationResetCountEvent', friendUsername);

//    //message status sent to server, it will be set to read status
//    window.lastMsgKey = getMessage(window.messages[window.messages.length - 1])
//    window.lastMsgKey = window.lastMsgKey && window.lastMsgKey.key;
//    window.lastMsgStatus = 1;
//  };

//  $scope.chatBlur = function(event, friendUsername) {
//    var window = $scope.findChatWindow(friendUsername);
//    window.focus = false;
//  };

//  function initFriends () {
//    isSearching = false;
//    FriendService.searchFriendForChat();
//  };

//  initFriends();

//  $scope.chatButtonClick = function(username) {
//    $rootScope.$broadcast('OpenChatWindowEvent', username);
//    $rootScope.$broadcast('NotificationResetCountEvent', username);
//  };

//  $scope.searchFriendForChat = function () {
//    if ($scope.friendSearch.length === 0) {
//      initFriends();
//      return;
//    }
//    if ($scope.friendSearch.length < 3) {
//      return;
//    }
//    FriendService.searchFriendForChat($scope.friendSearch).then(function (friends) {
//      // do not high light any items if there are no items
//      if ($scope.friendChatList.length === 0) return;
//      selectedIndex = 0;
//      $scope.friendChatList[selectedIndex].selected = true;
//      $scope.selectedFriend = $scope.friendChatList[selectedIndex];
//      isSearching = true;
//    });
//  };

//  var selectedIndex = -1, isSearching;
//  $scope.selectFriend = function (e) {
//    if (!isSearching) return;
//    if (e.which === 13) {
//      selectedIndex = selectedIndex < 0? 0: selectedIndex;
//      $scope.showChatWindow($scope.friendChatList[selectedIndex].account, true);
//      selectedIndex = -1;
//      $scope.friendSearch = '';
//      initFriends();
//      isSearching = false;
//    } else if (e.which === 40) {
//      angular.forEach($scope.friendChatList, function (friend) {
//        friend.selected = false;
//      });
//      if (selectedIndex === $scope.friendChatList.length - 1 && selectedIndex !== 0) {
//        selectedIndex = -1;
//        return;
//      }
//      selectedIndex = $scope.friendChatList.length === 1 ? 0 : (selectedIndex + 1);
//      $scope.friendChatList[selectedIndex].selected = true;
//      $scope.selectedFriend = $scope.friendChatList[selectedIndex];
//    } else if (e.which === 38) {
//      angular.forEach($scope.friendChatList, function (friend) {
//        friend.selected = false;
//      });
//      if (selectedIndex === 0) {
//        selectedIndex = -1;
//        return;
//      }
//      selectedIndex = selectedIndex === -1 ? ($scope.friendChatList.length - 1) : (selectedIndex - 1);
//      $scope.friendChatList[selectedIndex].selected = true;
//      $scope.selectedFriend = $scope.friendChatList[selectedIndex];
//    }
//  }

//  $scope.goUpDown = function (e) {
//    $scope.friendChatList[0].selected = true;
//  }
//}

//function NotificationService($rootScope, FriendService, WebSocketService) {
//  var self = this;
  
//  this.getNotification = function(friend){
//    WebSocketService.notification(friend, "DATA");
//  }

//  this.getNotificationCount = function(){
//    WebSocketService.notification("", "COUNT");
//  }

//  this.getNotificationLocalCount = function(){
//    WebSocketService.notification("", "COUNT");
//  }

//  this.resetCount = function(){
//    WebSocketService.notification("", "RESET");
//  }

//  this.decreaseCount = function(friend){
//    WebSocketService.notification(friend, "DECREASE");
//  }

//  this.newBox = function(username) {
//    if (!(username in self.notifybox)) {
//      self.notifybox = {};
//      self.notifybox[username] = {
//        'notifications': []
//      }
//    }
//    return self.notifybox[username];
//  }
//}

//function NotificationCtrl($scope, $rootScope, $cookieStore, FriendService, NotificationService) {
//  var self = this;
//  $scope.notifications = [];
//  $scope.haveData = false;
//  $scope.loading = false;
//  $scope.showBox = false;

//  self.username = angular.fromJson($cookieStore.get('username'));
//  if(self.username && self.username.length === 0){
//    return;
//  }

//  //get notification count
//  NotificationService.getNotificationCount();

//  $scope.toDateTime = function(timestamp){
//    var today = new Date();
//    var day = today.getDate();
//    var month = today.getMonth()+1; //January is 0!
//    var year = today.getFullYear();

//    var currentTimestamp = new Date(year + "-" + month + "-" + day + " 00:00:01").getTime();
//    if(timestamp < currentTimestamp){
//      return moment(new Date(timestamp)).format("MMM DD");
//    }
//    return moment(new Date(timestamp)).format("hh:mm a");
//  }

//  $scope.findNotifyBox = function(username){
//    var notifybox = {};
//    for (index = 0; index < $scope.notifications.length; index++) {
//      notifybox = $scope.notifications[index];
//      if (notifybox.account === username) {
//        return notifybox;
//      }
//    }
//    return null;
//  }

//  $scope.goToChat = function () {
//    window.location.href = "/chat/notify";
//  };

//  $scope.getNotifyBoxIndex = function(username){
//    for (var i = 0; i < $scope.notifications.length; i++) {
//      var notifybox = $scope.notifications[i];
//      if (notifybox.userinfo !== null && notifybox.userinfo.account === username) {
//        return i;
//      }
//    }

//    return -1;
//  }

//  $scope.getNotification = function(){
//    if($scope.notifications.length === 0){
//      $scope.loading = true;
//      $scope.noData = false;
//      //NotificationService.getNotification("");
//    }

//    NotificationService.getNotification("");

//    if($scope.count > 0){
//      NotificationService.resetCount();
//    }

//    $scope.count = 0;
//  }

//  $scope.resetCount = function(){
//    NotificationService.resetCount();
//  }

//  $rootScope.$on('NotificationCountDecreaseEvent', function(event, username) {
//    if ($scope.count > 0){
//      $scope.count--;
//    }
//    var index = $scope.getNotifyBoxIndex(username);
//    if(index >= 0){
//      var notifyBox = $scope.notifications[index];
//      if(notifyBox.count > 0){
//          NotificationService.decreaseCount(username);
//      }
//    }
//  });

//  $rootScope.$on('NotificationResetCountEvent', function(event, username) {
//    var index = $scope.getNotifyBoxIndex(username);
//    if(index >= 0){
//      var notifyBox = $scope.notifications[index];
//      notifyBox.count = 0;
//      $scope.notifications.splice(index, 1);
//      $scope.notifications.unshift(notifyBox);
//    }
//  });

//  $rootScope.$on('NotificationCountEvent', function(event) {
//    if($scope.notifications.length === 0){
//      NotificationService.getNotificationCount();
//      return;
//    }
//    var count = 0;
//    for(var i=0;i<$scope.notifications.length;i++){
//      if($scope.notifications[i].count > 0){
//        count++;
//      }
//    }

//    $scope.count = count;
//  });

//  //notification of user will be update by client when it had message
//  $rootScope.$on('NotificationChatMessageEvent', function(event, data) {
//    $scope.loading = false;

//    var username = '';
//    if (data.game != null) {
//      username = data.game.sender;
//    } else if (data.chat != null) {
//      username = data.chat.sender;
//    }

//    if(username === self.username){
//      return;
//    }

//    var index = $scope.getNotifyBoxIndex(username);
//    if(index < 0){
//      NotificationService.getNotificationCount();
//      data.count = 1;
//    } else {
//      var notifyBox = $scope.notifications[index];
//      $scope.notifications.splice(index, 1);
//      data.count = notifyBox.count + 1;
//    }

//    data.userinfo = FriendService.getFriend(username);
//    $scope.notifications.unshift(data);
//    if (!isTabActive) {
//      $rootScope.showNotification('/static/images/favicons/favicon-32x32.png',
//        data.chat && data.chat.message || data.chat.sender + ' sent you a game request.',
//        data.chat && 'New message from ' + data.chat.sender + ':',
//        data.chat.timestamp);
//      $rootScope.notifySound = !$rootScope.notifySound;
//    }
//    $rootScope.$apply();

//    //update count on top bar
//    $rootScope.$broadcast('NotificationCountEvent');
//  });

//  $rootScope.$on('NotificationChatEvent', function(event, data) {
//    $scope.loading = false;

//    if(data.notifyAction === 1) {
//      $scope.count = data.count;
//      $scope.countMobile = $scope.count;
//      $rootScope.$apply();
//      return;
//    } 

//    if(data.chats.length === 0 && $scope.notifications.length === 0){
//      $scope.noData = true;
//      $rootScope.$apply();
//      return;
//    }
//    $scope.noData = false;

//    for(var i=0;i<data.chats.length;i++){
//      var row = data.chats[i];
//      var username = '';
//      if (row.type === 2) {
//        username = row.game.sender == self.username ? row.game.receiver : row.game.sender;
//      } else if (row.type === 1) {
//        username = row.chat.sender == self.username ? row.chat.receiver : row.chat.sender;
//      }
      
//      row.userinfo = FriendService.getFriend(username);
//      if (!row.userinfo) continue;
//      var index = $scope.getNotifyBoxIndex(username);
//      if(index >= 0){
//        var notifyBox = $scope.notifications[index];
//        $scope.notifications.splice(index, 1);
//      }
//      $scope.notifications.push(row);
//    }
//    $rootScope.$apply();

//    //call reset count of new notification when it showed
//    NotificationService.resetCount();
//  });

//}

//function scrollChatMessage($timeout, WebSocketService) {
//  return {
//    restrict: 'A',
//    link: function(scope, element, attr) {
//      var raw = element[0];
//      var first = true;
//      $(element).perfectScrollbar();
//      element.bind('scroll', function() {
//        if (0 === raw.scrollTop) {
//          var elmp = element.parent();
//          var window = $(element).scope().findChatWindow(elmp.attr('data-username'));
//          if(window !== null){
//            window.loading = true;

//            setTimeout(function() {
//              window.loading = false;
//            }, 1000);
            
//          }
//          WebSocketService.getHistory(elmp.attr('data-username'), ++window.currentPage, "0");
          
//          $(element.find('.chat-typing')[0]).addClass('float-bottom');
//          return;
//        }

//        if(raw.scrollHeight > raw.offsetHeight){
//          $(element.find('.chat-typing')[0]).removeClass('float-bottom');
//        }
//      });

//      scope.$watchCollection(attr.scroll, function(newVal) {

//        $timeout(function() {
//          if (newVal.length === 0) return;
//          var lastMessage = newVal[newVal.length - 1];
//          // we use the message height plus 30px to scroll
//          // this is kind of hard code, however, we don't have better solution
//          // message type equal to 2 means the message is a game invitation
//          // otherwise just a normal message
//          var heightLast = lastMessage.type === 2 ? 100 : 60;
//          if (first) {
//            raw.scrollTop = raw.scrollHeight;
//            if (raw.scrollHeight > raw.offsetHeight) {
//              first = false;
//            }
//          }
//          if (raw.scrollTop + raw.offsetHeight >= raw.scrollHeight - heightLast) {
//            raw.scrollTop = raw.scrollHeight;
//          }
//        });
//      });
//    }
//  }
//}

//gtokenApp.directive('autofocus', ['$parse', function ($parse) {
//  return {
//    link: function (scope, element, attrs) {
//      scope.$watch(attrs.autofocus, function (val) {
//        if (val) {
//          element.find('textarea').focus();
//          element.find('.name-bar').addClass('focus');
//        } else {
//          element.find('.name-bar').removeClass('focus');
//        }
//      });
//    }
//  }
//}]);

//gtokenApp.directive('myMaxLength', ['$compile', '$log', function($compile, $log) {
//    return {
//      restrict: 'A',
//      require: 'ngModel',
//      link: function (scope, elem, attrs, ctrl) {
//        attrs.$set("ngTrim", "false");
//        var maxlength = parseInt(attrs.myMaxLength, 10);
//        ctrl.$parsers.push(function (value) {
//            if (value.length > maxlength)
//            {
//                value = value.substr(0, maxlength);
//                ctrl.$setViewValue(value);
//                ctrl.$render();
//            }
//            return value;
//        });
//      }
//    };
//  }]);

//gtokenApp.directive('minimizeChatBox', ['$compile', function($compile) {
//    return {
//      restrict: 'A',
//      link: function (scope, elem, attrs, ctrl) {
//        scope.$watch(attrs.minimizeChatBox, function (val) {
//          val? elem.addClass('minimize') : elem.removeClass('minimize');
//        });
//      }
//    };
//  }]);

//gtokenApp.directive('clickOutsideToClose', [function() {
//    return {
//      restrict: 'A',
//      scope: {
//        clickOutsideToClose: '='
//      },
//      link: function (scope, elem, attrs, ctrl) {
//        $(document).click(function (e) {
//          var isInside = $(e.srcElement || e.target);
//          isInside = isInside.closest(elem[0])[0];
//          if (!isInside) {
//            scope.clickOutsideToClose = false;
//            scope.$apply();
//          }
//        })
//      }
//    };
//}]);

//gtokenApp.directive('playAudio', [function() {
//  var isInit = true;
//  return {
//    restrict: 'A',
//    link: function (scope, elem, attrs, ctrl) {
//      scope.$watch(attrs.playAudio, function (val) {
//        !isInit && elem[0].play();
//        isInit = false;
//      });
//    }
//  };
//}]);

gtokenApp.directive('buttonAnimation', [function() {
  return {
    restrict: 'A',
    link: function (scope, elem, attrs, ctrl) {
      var chatButtonClass = 'show-buttons';
      elem.click(function () {
        var wideScreen = $(window).width() >= 760;
        if (elem.hasClass(chatButtonClass)) {
          elem.removeClass(chatButtonClass);
          wideScreen && elem.find('.friend-buttons').css('background-image', 'url(/static/images/prev_icon.png)');
        } else {
          elem.parent().find('li').removeClass(chatButtonClass);
          elem.addClass(chatButtonClass);
          if (wideScreen ) {
            elem.parent().find('.friend-buttons').css('background-image', 'url(/static/images/prev_icon.png)');
            elem.find('.friend-buttons').css('background-image', 'url(/static/images/next_icon.png)');
          }
        }
      });
    }
  };
}]);

//gtokenApp.service('WebSocketService', ['$cookieStore', '$rootScope', WebSocketService]);
//gtokenApp.service('LoginService', ['$rootScope', 'WebSocketService', LoginService]);
//gtokenApp.service('FriendService', ['$http', '$rootScope', 'WebSocketService', FriendService]);
//gtokenApp.service('GameService', ['$http', '$rootScope', GameService]);

//gtokenApp.service('NotificationService', ['$rootScope', 'FriendService', 'WebSocketService', NotificationService]);
//gtokenApp.service('MessengerService', MessengerService);
//gtokenApp.controller('NotificationCtrl', ['$scope', '$rootScope', '$cookieStore', 'FriendService', 'NotificationService', NotificationCtrl]);
//gtokenApp.controller('MessengerCtrl', ['$scope', '$rootScope', '$cookieStore', 'FriendService', 'WebSocketService', 'MessengerService', 'GameService', 'NotificationService', MessengerCtrl]);
//gtokenApp.directive('chatButtonDirective', ['$cookieStore', '$rootScope', 'LoginService', 'FriendService', chatButtonDirective]);
//gtokenApp.directive('userStatusDirective', ['$cookieStore', '$rootScope', 'LoginService', 'FriendService', userStatusDirective]);
//gtokenApp.directive('scroll', ['$timeout', 'WebSocketService', scrollChatMessage]);
