function WebSocketService($cookieStore, $rootScope) {
  var websocket = chatWebSocket;
  var self = this;
  var socketTimer = 0;

  if (typeof dcodeIO === 'undefined' || !dcodeIO.ProtoBuf) {
    throw (new Error("ProtoBuf.js is not present. Please see www/index.html for manual setup instructions."));
  }
  // Initialize ProtoBuf.js
  var ProtoBuf = dcodeIO.ProtoBuf;
  var ChatProto = ProtoBuf.loadProtoFile("/static/js/chatProtocol.proto").build("gtoken");
  var MessageSend = ChatProto.MessageSendProto;
  var MessageReceive = ChatProto.MessageReceiveProto;
  var Command = ChatProto.Command;

  // KNN this is FUBAR.
  // With $cookies, username doesn't have quotation, but chat_token does
  // With $cookieStore, we got "unexpected token" error because username doesn't have quotation and angular thinks it is an int
  // So I added a hardcode quotation to username on server, e.g ""khang""
  // then angular.fromJson remove the first pair of quotation and leave username a normal string
  self.token = $cookieStore.get('chat_token');
  self.username = angular.fromJson($cookieStore.get('username'));

  var socket;

  function socketconnect() {
    socket = new WebSocket(websocket);
    socket.binaryType = "arraybuffer"; // We are talking binary

    socket.onmessage = function(event) {
      try {
        // Decode the Message
        var msg = MessageReceive.decode(event.data);
        console.log(msg)

        switch (msg.command) {
          case 5:
            $rootScope.$broadcast('ReceiveMessageEvent', msg);
            $rootScope.$broadcast('NotificationChatMessageEvent', msg);
            break;
          case 7:
            $rootScope.$broadcast('FriendListEvent', msg.friendlist);
            break;
          case 9:
          case 10:
            $rootScope.$broadcast('LoginEvent', msg);
            break;
          case 14:
            $rootScope.$broadcast('HistoryEvent', msg.history);
            break;
          case 15:
            $rootScope.$broadcast('TypingEvent', msg);
            break;
          case 18:
            $rootScope.$broadcast('NotificationChatEvent', msg.chatNotification);
            $rootScope.$broadcast('NotificationChatHeaderEvent', msg.chatNotification);
            break;
          case 19:
            $rootScope.$broadcast('NewFriendEvent', msg.friendlist);
            break;
        }

      } catch (error) {
        console.log("Anh Phong oi loi roi" + error);
      }
    }

    socket.onerror = function(event) {
      console.log("Anh Phong oi loi roi " + event);
    }

    socket.onclose = function(event) {
      try {
        if (!self.socketTimer) {
          /* avoid firing a new setInterval, after one has been done */
          self.socketTimer = setInterval(function() {
            socketconnect();
          }, 3000);
        }
        /* that way setInterval will be fired only once after loosing connection */
        ;

      } catch (error) {
        console.log("Anh Phong oi loi roi " + error);
      }
    }

    socket.onopen = function() {
      setInterval(function() {
        if (socket.readyState === WebSocket.OPEN) {
          socket.send("Ping");
        }
      }, 1000 * 30);

      // /* as what was before */
      // if (self.socketTimer) { /* a setInterval has been fired */
      //   clearInterval(self.socketTimer);
      //   self.socketTimer = 0;
      //   self.login();
      //   var windowsNames = $cookieStore.get('chat_windows');
      //   var elms = angular.element(document.getElementsByClassName('chat-window'));

      //   for (var i = 0; i < elms.length; i++) {
      //     var username = elms.attr('data-username');
      //     var elm_msg = elms.find('.chat-message span[data-idx]');
      //     var window = $(elm_msg).scope().findChatWindow(username);
      //     if(window.lastMsgKey.length === 0 || window.lastMsgStatus !== 2){
      //       return;
      //     }
      //     console.log('get history from key: ' + window.lastMsgKey);
      //     self.getHistory(username, 1, window.lastMsgKey);
      //   }
      // }
    }
  }

  socketconnect();

  this.sendMessageOnSocket = function(callback) {
    setTimeout(
      function() {
        if (socket.readyState === 1) {
          console.log("Connection is made");
          callback();
          return;
        } else if (socket.readyState === 3) {
          console.log("socket closed, open new one");
          socketconnect();
          self.sendMessageOnSocket(callback);
        } else {
          console.log("wait for connection...");
          self.sendMessageOnSocket(callback);
        }
      }, 5); // wait 5 milisecond for the connection...
  }

  this.getFriendList = function() {
    self.sendMessageOnSocket(function() {
      if (socket.readyState === WebSocket.OPEN) {
        var msg = new MessageSend({
          "command": "FRIENDS",
          "user": self.username,
          "token": self.token
        });
        var buffer = msg.encode();
        socket.send(buffer.toArrayBuffer());
        console.log(socket.readyState);
      } else {
        console.log("Not connected");
      }
    });
  }

  this.sendInvitation = function(friendUsername, game) {
    self.sendMessageOnSocket(function() {
      if (socket.readyState === WebSocket.OPEN) {
        var msg = new MessageSend({
          "command": "CHAT",
          "user": self.username,
          "token": self.token,
          "friend": friendUsername,
          "type": "GAME",
          "game": {
            "sender": self.username,
            "receiver": friendUsername,
            "id": game.id.toString(),
            "title": game.name,
            "thumb": game.icon_filename,
            "link": game.web_link
          }
        });
        var buffer = msg.encode();
        socket.send(buffer.toArrayBuffer());
      } else {
        console.log("Not connected");
      }
    });
  }

  this.sendMessage = function(friendUsername, message) {
    self.sendMessageOnSocket(function() {
      if (socket.readyState === WebSocket.OPEN) {
        var msg = new MessageSend({
          "command": "CHAT",
          "user": self.username,
          "token": self.token,
          "friend": friendUsername,
          "data": message
        });
        var buffer = msg.encode();
        socket.send(buffer.toArrayBuffer());
      } else {
        console.log("Not connected!");
      }
    });
  }

  this.typing = function(friendUsername) {
    self.sendMessageOnSocket(function() {
      if (socket.readyState === WebSocket.OPEN) {
        var msg = new MessageSend({
          "command": "TYPING",
          "user": self.username,
          "token": self.token,
          "friend": friendUsername
        });
        var buffer = msg.encode();
        socket.send(buffer.toArrayBuffer());
      } else {
        console.log("Not connected!");
      }
    });
  }

  this.getHistory = function(friendUsername, currentPage, fromId) {
    self.sendMessageOnSocket(function() {
      if (socket.readyState === WebSocket.OPEN) {
        var msg = new MessageSend({
          "command": "HISTORY",
          "user": self.username,
          "token": self.token,
          "friend": friendUsername,
          "current": currentPage,
          "fromId": fromId
        });
        var buffer = msg.encode();
        socket.send(buffer.toArrayBuffer());
      } else {
        console.log("Not connected!");
      }
    });
  }

  this.login = function() {
    self.sendMessageOnSocket(function() {
      if (socket.readyState === WebSocket.OPEN) {
        var msg = new MessageSend({
          "command": "LOGIN",
          "user": self.username,
          "token": self.token
        });
        var buffer = msg.encode();
        socket.send(buffer.toArrayBuffer());
      } else {
        console.log("Not connected!");
      }
    });
  }


  this.getNotification = function(friend, action) {
    self.sendMessageOnSocket(function() {
      if (socket.readyState === WebSocket.OPEN) {
        var msg = new MessageSend({
          "command": "NOTIFICATION",
          "user": self.username,
          "token": self.token,
          "friend": friend,
          "notifyAction": action
        });
        var buffer = msg.encode();
        socket.send(buffer.toArrayBuffer());
      } else {
        console.log("Not connected!");
      }
    });
  }

  this.setRead = function(friendUsername, lastId) {
    self.sendMessageOnSocket(function() {
      if (socket.readyState === WebSocket.OPEN) {
        var msg = new MessageSend({
          "command": "CHAT_STATUS",
          "user": self.username,
          "friend": friendUsername,
          "token": self.token,
          "fromId": lastId
        });
        var buffer = msg.encode();
        socket.send(buffer.toArrayBuffer());
      } else {
        console.log("Not connected!");
      }
    });
  }

}


function MessengerService() {
  var self = this;
  self.messengers = {};
  self.typings = {};

  self.getMessenger = function(username) {
    if (!(username in self.messengers)) {
      self.messengers[username] = {
        'window': false,
        'focus': false,
        'lastMsgStatus': 2, //unread,
        'lastMsgKey': '',
        'messages': []
      }
    }
    return self.messengers[username];
  }

  self.getTyping = function(username){
    self.typings[username] = {
      'user': {},
      'chat': {
        'message': '•••',
        'timestamp': new Date().getTime(),
        'typing': true
      },
      'type': 1,
      'typing': true
    }
    return self.typings[username];
  }

}

function LoginService($rootScope, WebSocketService) {
  var self = this;
  self.isLogin = false;

  if (!self.isLogin) {
    WebSocketService.login();
  }

  $rootScope.$on('LoginEvent', function(event, data) {
    if (data.command === 9) {
      console.log("login success");
      self.isLogin = true;
    } else {
      console.log("login fail");
      self.isLogin = true;
    }
  });

  this.onLoginAsync = function(callback) {
    setTimeout(
      function() {
        if (self.isLogin) {
          callback();
          return;
        } else {
          self.onLoginAsync(callback);
        }
      }, 5); // wait 5 milisecond for the connection...
  }

}

function FriendService($http, $rootScope, WebSocketService) {
  var self = this;
  self.friends = {};
  self.ready = false;

  this.getFriendList = function() {
    $http({
      method: 'post',
      url: '/friend/friendlist',
      headers: {
        'Content-type': 'application/json;',
      },
    }).success(function(data) {
      self.friends = data;
      for (var username in self.friends) {
        self.friends[username]['status'] = 'offline';
      }
      WebSocketService.getFriendList();
    });
  }

  this.searchFriendForChat = function (keyword) {
    return $http({
      method: 'post',
      url: '/friend/search-friend',
      data: {'term': keyword || ''},
      headers: {
          'Content-type': 'application/json;'
      }
    }).then(function (res) {
      $rootScope.friendChatList = res.data;
      return $rootScope.friendChatList;
    });
  };

  if (Object.keys(self.friends).length === 0) {
    this.getFriendList();
  }

  $rootScope.onlineCounter = 0;
  $rootScope.$on('FriendListEvent', function(event, data) {
    for (index = 0; index < data.length; index++) {
      username = data[index]['user'];
      if (username in self.friends) {
        var status = "offline";
        switch (data[index]['status']) {
          case 1:
            status = 'online';
            if (status !== self.friends[username]['status']) {
              $rootScope.onlineCounter++;
            }
            break;
          case 2:
            status = 'offline';
            if (status !== self.friends[username]['status']) {
              $rootScope.onlineCounter--;
            }
            break;
        }

        self.friends[username]['status'] = status;
        if (status === 'offline') {
          $rootScope.moveOfflineToBottom(username);
        } else {
          $rootScope.moveOnlineToTop(username);
        }
      }
    }
    self.ready = true;
    $rootScope.$apply();
  });

  $rootScope.moveOnlineToTop = function (username) {
    if (!$rootScope.friendChatList || $rootScope.friendChatList.length === 1) return;
    var currentOnline = $rootScope.friendChatList.filter(function (f) { return f.account === username; })[0];
    var currentOnlineIndex = $rootScope.friendChatList.indexOf(currentOnline);
    $rootScope.friendChatList.splice(currentOnlineIndex, 1);
    $rootScope.friendChatList.splice(0, 0, currentOnline);
  }

  $rootScope.moveOfflineToBottom = function (username) {
    if (!$rootScope.friendChatList || $rootScope.friendChatList.length === 1) return;
    var current = $rootScope.friendChatList.filter(function (f) { return f.account === username; })[0];
    var currentIndex = $rootScope.friendChatList.indexOf(current);
    $rootScope.friendChatList.splice(currentIndex, 1);
    $rootScope.friendChatList.splice($rootScope.friendChatList.length, 0, current);
  }

  $rootScope.$on('NewFriendEvent', function(event, data) {
    self.getFriendList();
  });

  this.getFriendAsync = function(username, callback) {
    setTimeout(
      function() {
        if (self.ready) {
          var friend = self.getFriend(username);
          callback(friend);
          return;
        } else {
          self.getFriendAsync(username, callback);
        }
      }, 5); // wait 5 milisecond for the connection...
  }


  this.getFriend = function(username) {
    return username in self.friends ? self.friends[username] : null;
  }
}

function GameService($http, $rootScope) {
  var self = this;
  self.games = {}

  this.getGames = function(keyword, callback) {
    $http({
      method: 'post',
      url: '/game/game-list',
      data: {
        keyword: keyword
      },
      headers: {
        'Content-type': 'application/json;',
      },
    }).success(function(data) {
      self.games = data['games'];
      callback(self.games);
    });
  }
}


function userStatusDirective($cookieStore, $rootScope, LoginService, FriendService) {
  var self = this;

  this.link = function(scope, element, attrs) {
    self.scope = scope;
    self.element = element;
    self.attrs = attrs;

    LoginService.onLoginAsync(function() {
      FriendService.getFriendAsync(attrs.username, function(friend) {
        if (!friend) return;
        scope.friend = friend;
        if (friend.status === 'offline') {
          $rootScope.moveOfflineToBottom(friend.account);
        } else {
          $rootScope.moveOnlineToTop(friend.account);
        }
      });   
    });
  }
  return {
    scope: true,
    link: link
  }
}

function chatButtonDirective($cookieStore, $rootScope, LoginService, FriendService) {
  var self = this;

  // KNN should use its own template and doesn't need html in index.html
  this.link = function(scope, element, attrs) {
    self.scope = scope;
    self.element = element;
    self.attrs = attrs;

    scope.chatButtonClicked = function() {
      $rootScope.$broadcast('OpenChatWindowEvent', attrs.username);
    }

    scope.chatButtonClickedByUserName = function(username) {
      $rootScope.$broadcast('OpenChatWindowEvent', username);
      $rootScope.$broadcast('NotificationResetCountEvent', username);
    }

    LoginService.onLoginAsync(function() {
      FriendService.getFriendAsync(attrs.username, function(friend) {
        scope.friend = friend;
        $rootScope.$apply();
      });   
    });

    var interval = setInterval(function() {
      if (!LoginService.isLogin) {
        return;
      }

      if (LoginService.isLogin && FriendService.ready) {
        scope.friend = FriendService.getFriend(attrs.username);

        clearInterval(interval);
        $rootScope.$apply();
      }
    }, 5);
  }

  return {
    scope: true,
    link: link
  }
}

function MessengerCtrl($scope, $rootScope, $cookieStore, FriendService, WebSocketService, MessengerService, GameService, NotificationService) {
  var self = this;
  self.username = angular.fromJson($cookieStore.get('username'));
  $scope.username = self.username;

  // KNN why can't use controllerAs
  $scope.window = {};
  $scope.games = [];

  $scope.toDateTime = function(timestamp){
    var today = new Date();
    var day = today.getDate();
    var month = today.getMonth()+1; //January is 0!
    var year = today.getFullYear();

    var currentTimestamp = new Date(year + "-" + month + "-" + day + " 00:00:01").getTime();
    if(timestamp < currentTimestamp){
      return moment(new Date(timestamp)).format("MMM dd");
    }
    return moment(new Date(timestamp)).format("hh:mm a");
  }

  GameService.getGames('', function(data) {
    $scope.games = data;
  });

  $scope.searchGame = function(event, friend){
    var keyword = event.srcElement || event.target;
    if (!event.shiftKey && $.trim(keyword.value) !== '') {
      GameService.getGames(keyword.value, function(data) {
        var window = $scope.findChatWindow(friend);
        $scope.games = data;
      });
    }
  };

  $scope.findChatWindow = function(username) {
    if($scope.window.friend && $scope.window.friend.account === username){
      return $scope.window;
    }
    return null;
  };

  $scope.showChatWindow = function(username) {
    var existing = false;
    var window = $scope.findChatWindow(username);

    if (window == null) {
      window = MessengerService.getMessenger(username);
      window['friend'] = FriendService.getFriend(username);

      if(window['friend'] === null){
        return;
      }
      $scope.window = window;
      WebSocketService.getHistory(username, 1, "0");
      var windowsName = $cookieStore.get('chat_windows_mobile');
      if (windowsName !== undefined && windowsName.indexOf(username, 0) === -1) {
        $cookieStore.put('chat_windows_mobile', windowsName + username + ",");
      } else {
        $cookieStore.put('chat_windows_mobile', username + ",");
      }
    }

    if(!window){
      $('.chat-window[data-username=' +username+']').find('input').focus();
    }

    window.typing = false; // typing of friend
    window.mytyping = false; // the flag for anti spam server when I'm typing
    window.invitegame = false;
    window.blinking = false;
    if (window.timeoutTyping === 'undefined') {
      window.timeoutTyping = 0; //variable of setTimeout to set window.typing = false;
    }
    
    return window;
  };

  $scope.openInviteGameWindow = function(username) {
    var window = $scope.findChatWindow(username);
    if (window == null) {
      return;
    }
    window.invitegame = !window.invitegame;
  };

  function getMessage(obj) {
    if(obj.chat != null){
      return obj.chat;
    } else if(obj.game != null){
      return obj.game;
    } else {
      return null;
    }
  }

  function pushMessage(username, data){
    var msgObj = getMessage(data);
    if(msgObj == null){
      return;
    }
    var window = $scope.findChatWindow(username);
    if(window == null){
      return;
    }

    window = $scope.showChatWindow(username);
    if(window.messages.length == 0){
      window.messages.push(data);
      return window;
    }

    var msgFormLastObj = getMessage(window.messages[window.messages.length - 1]);

    window.typing = false;
    if(msgFormLastObj.typing !== undefined){
      window.messages.splice(window.messages.length -1, 1);
    }

    var msgFormFirstObj = getMessage(window.messages[0]);
    msgFormLastObj = getMessage(window.messages[window.messages.length - 1]);
    var lastTime = msgFormLastObj.timestamp;
    var firstTime = msgFormFirstObj.timestamp;
    var timestamp = msgObj.timestamp;

    if(timestamp < firstTime){
      window.messages.unshift(data);
    } else if(timestamp > lastTime){
      window.messages.push(data);
    } else if(timestamp === lastTime || timestamp === firstTime){
      return window;
    } else {
      for(var i = window.messages.length - 1;i>0;i--){
        
        var msgPreviousObj = getMessage(window.messages[i-1]);
        var msgCurrentObj = getMessage(window.messages[i]);
        var previousTime = msgPreviousObj.timestamp;
        var currentTime = msgCurrentObj.timestamp;

        if(currentTime > timestamp && 
          previousTime < timestamp) {
          window.messages.splice(i, 0, data);
          break;
        }
      }
    }

    return window;
  }

  $scope.onFocusWindow = function(event, username) {
    var text = "";
    if (window.getSelection) {
      text = window.getSelection().toString();
    } else if (document.selection && document.selection.type != "Control") {
      text = document.selection.createRange().text;
    }

    if(text.length == 0){
      $('.chat-window[data-username=' +username+']').find('#input-text').focus();
      $scope.chatFocus(null, username);
    }
  };

  //show chat windows from cookies

  var interval = setInterval(function() {
    if (FriendService.ready) {
      clearInterval(interval);
      $scope.showChatWindow($('.chat-window').attr('data-username'));
      // var windowsName = $cookieStore.get('chat_windows_mobile');
      // if (windowsName !== undefined && windowsName.length > 0) {
      //   var windowsNames = windowsName.split(',');
      //   for (var i = 0; i < windowsNames.length; i++) {
      //     if (windowsNames[i].length > 0) {
      //       $scope.showChatWindow(windowsNames[i]);
      //     }
      //   }
      // }
    }
  }, 10);

  $rootScope.showNotification = function(icon, text, title, tag) {
    if (!("Notification" in window)) {
      return; // This browser does not support desktop notification
    } else if (Notification.permission === "granted") {
      var options = {
        body: text,
        icon: icon,
        dir: "ltr",
        tag : tag
      };
      var notification = new Notification(title || "", options);
      setTimeout(function() {
        notification.close();
      }, 3000);
      notification.onclick = function () {
        $(window).trigger('focus');
      }
    } else if (Notification.permission !== 'defined') {
      Notification.requestPermission(function(permission) {
        if (!('permission' in Notification)) {
          Notification.permission = permission;
        }
      });
    }
  };

  var alertChat = (function() {
    if($rootScope.titlePage == null || $rootScope.titlePage.length == 0){
      $rootScope.titlePage = document.title;
      $rootScope.messageCount = 0;
    }

    var timeoutId;
    var blink = function(username) {
      var msg = username + " messaged you";
      document.title = document.title == msg ? "("+ $rootScope.messageCount + ") " + $rootScope.titlePage : msg;
    };
    var clear = function() {
      clearInterval(timeoutId);
      document.title = $rootScope.titlePage;
      window.onmousemove = null;
      timeoutId = null;
      $rootScope.messageCount = 0;
    };
    return function(username) {
      $rootScope.messageCount = $rootScope.messageCount + 1;
      if (!timeoutId) {
        timeoutId = setInterval(function(){blink(username)}, 1200);
        window.onmousemove = clear;
      }
    };
  }());

  $rootScope.$on('ReceiveMessageEvent', function(event, data) {
    var username = '';
    var sender = '';
    var msgObj = getMessage(data)
    username = msgObj.sender == self.username ? msgObj.receiver : msgObj.sender;
    sender = msgObj.sender;

    data.user = FriendService.getFriend(sender);
    if(data.user == null){
      return;
    }

    var window = pushMessage(username, data);

    if(username === sender && msgObj.status !== 1){
      window.blinking = true;
    }
    
    if(username !== self.username){
      window.lastMsgKey = msgObj.key
      window.lastMsgStatus = msgObj.status;
    }
    
    $rootScope.$apply();
    if(this.username == data.user.account && msgObj.status !== 1){
      //Blink window   
      alertChat(sender);
    }
  });

  $rootScope.$on('HistoryEvent', function(event, data) {
    if (data.length == 0) {
      return;
    }

    var addAll = false;
    var msgReceiveFirstObj = getMessage(data[0]);
    var msgReceiveLastObj = getMessage(data[data.length - 1]);
    var username = msgReceiveFirstObj.sender == self.username ? msgReceiveFirstObj.receiver : msgReceiveFirstObj.sender;

    //Scroll bar
    var elmMessage = $('div[data-username='+username+']').find('.c-message');
    var elmFirst = 0;

    var window = $scope.showChatWindow(username);

    try {

      if(window.messages.length > 0){
        var msgFormFirstObj = getMessage(window.messages[0]);
        var msgFormLastObj = getMessage(window.messages[window.messages.length - 1]);

        //check history in first or previous load
        if (window.messages.length == 0 || (msgReceiveLastObj !== null && msgReceiveLastObj.timestamp < msgFormFirstObj.timestamp) ||
            (msgReceiveLastObj !== null && msgReceiveLastObj.timestamp < msgFormFirstObj.timestamp)) {
          for (var i = data.length - 1; i >= 0; i--) {
            data[i].user = FriendService.getFriend(getMessage(data[i]).sender);
            pushMessage(username, data[i]);
          }

          if(data.length > 0 && username !== self.username){
            window.lastMsgKey = msgFormLastObj.key;
            window.lastMsgStatus = msgFormLastObj.status;
          } 
          $rootScope.$apply();
          return;
        }
      }

      //sync data when it miss match with server
      for (var i = 0; i < data.length; i++) {
        data[i].user = FriendService.getFriend(getMessage(data[i]).sender);
        pushMessage(username, data[i]);
      }
      $rootScope.$apply();

      var msgFormFirstObj = getMessage(window.messages[0]);
      var msgFormLastObj = getMessage(window.messages[window.messages.length - 1]);
      if(data.length > 0 && username !== self.username){
        window.lastMsgKey = msgFormLastObj.key;
        window.lastMsgStatus = msgFormLastObj.status;
      } 
    } finally {
      window.loading = false;

      if(elmMessage.length > 0){
        elmFirst = elmMessage[0];
        var scrollObj = $('div[data-username='+username+']').find('.chat-content')[0];
        scrollObj.scrollTop = $(elmFirst).position().top - scrollObj.offsetHeight - $(elmFirst).parent('.message-container').height();
      }
    }
  });

  $rootScope.$on('TypingEvent', function(event, data) {
    var username = data.user;
    var window = $scope.findChatWindow(username);
    if (window == null) {
      return;
    }
    
    if(!window.typing){
      window.typing = true;
      $scope.addTyping(username);
    }

    clearTimeout(window.timeoutTyping);
    window.timeoutTyping = setTimeout(function() {
      window.typing = false;
      $scope.removeTyping(username);
      $rootScope.$apply();
    }, 1500, username);
    $rootScope.$apply();
  });

  $scope.addTyping = function(username){
    var typingObj = MessengerService.getTyping();
    typingObj.user = FriendService.getFriend(username);
    var window = $scope.findChatWindow(username);
    if(window.messages[window.messages.length - 1].typing !== undefined){
      return;
    }
    
    window.messages.push(typingObj);
    
    $rootScope.$apply();
  }

  $scope.removeTyping = function(username){
    var window = $scope.findChatWindow(username);
    if(window.messages[window.messages.length - 1].typing !== undefined){
      window.messages.splice(window.messages.length -1, 1);
      $rootScope.$apply();
    }
    
  }

  $scope.sendInvitation = function(event, friendUsername, game) {
    WebSocketService.sendInvitation(friendUsername, game);
    // After sending the invitation, dispose the modal
    var window = $scope.findChatWindow(friendUsername);
    window.invitegame = !window.invitegame;
  };

  $scope.sendTyping = function(event, friendUsername) {
    var textarea = event.srcElement || event.target;
    var window = $scope.findChatWindow(friendUsername);

    if (window.mytyping == false) {
      if (textarea.value !== '') {
        WebSocketService.typing(friendUsername);
      }
      setTimeout(function() {
        window.mytyping = false;
      }, 1000);
    }

    window.mytyping = true;
  };

  $scope.sendMessage = function(event, friendUsername) {
    var textarea = event.srcElement || event.target;
    var window = $scope.findChatWindow(friendUsername);

    if ($.trim(textarea.value) === '' && event.keyCode === 13) {
      textarea.value = '';
      event.preventDefault();
      return;
    }
    if (event.keyCode === 13 && !event.shiftKey && $.trim(textarea.value) !== '') { // Enter key, send message
      event.preventDefault();
      window.mytyping = false;

      textarea.value = $.trim(textarea.value);
      if (textarea.value) {
        WebSocketService.sendMessage(friendUsername, textarea.value);
        textarea.value = '';
        event.preventDefault();
      }
    }
  };

  $scope.sendMessageButton = function(friendUsername) {
    var window = $scope.findChatWindow(friendUsername);
    window.mytyping = false;

    var msg = $("#input-text").val();
    if (msg) {
      WebSocketService.sendMessage(friendUsername, msg);
      $("#input-text").val('');
    }
  };

  $scope.chatFocus = function(event, friendUsername) {
    var window = $scope.findChatWindow(friendUsername);
    window.focus = true;

    //if(window.blinking){
      window.blinking = false;
    //}

    if(window.lastMsgKey.length == 0 || window.lastMsgStatus !== 2){
      return;
    }
    
    WebSocketService.setRead(friendUsername, window.lastMsgKey);
    // $rootScope.$broadcast('NotificationResetCountEvent', friendUsername);

    //message status sent to server, it will be set to read status
    window.lastMsgKey = getMessage(window.messages[window.messages.length - 1]).key;
    window.lastMsgStatus = 1;
    $rootScope.$broadcast('NotificationCountDecreaseEvent', friendUsername);
    //NotificationService.getNotificationCount();
  };

  $scope.chatBlur = function(event, friendUsername) {
    var window = $scope.findChatWindow(friendUsername);
    window.focus = false;
  };

  function initFriends () {
    isSearching = false;
    FriendService.searchFriendForChat();
  };

  initFriends();
  
  $scope.openChatPage = function (username) {
    window.location.href = '/chat/chat?friend=' + username;
  }
}


function NotificationService($rootScope, FriendService, WebSocketService) {
  var self = this;
  
  this.getNotification = function(friend){
    WebSocketService.getNotification(friend, "DATA");
  }

  this.getNotificationCount = function(){
    WebSocketService.getNotification("", "COUNT");
  }

  this.resetCount = function(){
    WebSocketService.getNotification("", "RESET");
  }

  this.decreaseCount = function(friend){
    WebSocketService.getNotification(friend, "DECREASE");
  }

  this.getCount = function(){
    return parseInt($('.notify-count > p').text() || 0);
  }

  this.setCount = function(value){
    if(value <= 0){
      $('.notify-count').hide();
    } else {
      $('.notify-count').show();
    }
    $('.notify-count > p').text(value);
  }

  this.newBox = function(username) {
    if (!(username in self.notifybox)) {
      self.notifybox = {};
      self.notifybox[username] = {
        'notifications': []
      }
    }
    return self.notifybox[username];
  }
}

function NotificationCtrl($scope, $rootScope, $cookieStore, FriendService, NotificationService) {
  var self = this;
  $scope.notifications = [];
  $scope.haveData = false;
  $scope.loading = false;
  $scope.showBox = false;

  self.username = angular.fromJson($cookieStore.get('username'));
  if(self.username.length === 0){
    return;
  }

  $scope.toDateTime = function(timestamp){
    var today = new Date();
    var day = today.getDate();
    var month = today.getMonth()+1; //January is 0!
    var year = today.getFullYear();

    var currentTimestamp = new Date(year + "-" + month + "-" + day + " 00:00:01").getTime();
    if(timestamp < currentTimestamp){
      return moment(new Date(timestamp)).format("MMM dd");
    }
    return moment(new Date(timestamp)).format("hh:mm a");
  }

  $scope.findNotifyBox = function(username){
    var notifybox = {};
    for (index = 0; index < $scope.notifications.length; index++) {
      notifybox = $scope.notifications[index];
      if (notifybox.account === username) {
        return notifybox;
      }
    }
    return null;
  }

  $scope.getNotifyBoxIndex = function(username){
    for (var i = 0; i < $scope.notifications.length; i++) {
      var notifybox = $scope.notifications[i];
      if (notifybox.userinfo.account === username) {
        return i;
      }
    }

    return -1;
  }

  $scope.getNotification = function(){
    if($scope.notifications.length === 0){
      $scope.loading = true;
      $scope.noData = false;
    }

    NotificationService.getNotification("");

    if($scope.count > 0){
      NotificationService.resetCount();
    }
    $scope.count = 0;
  }

  $scope.resetCount = function(){
    NotificationService.resetCount();
  }

  $rootScope.$on('NotificationResetCountEvent', function(event, username) {
    var index = $scope.getNotifyBoxIndex(username);
    if(index >= 0){
      var notifyBox = $scope.notifications[index];
      notifyBox.count = 0;
      $scope.notifications.splice(index, 1);
      $scope.notifications.unshift(notifyBox);
    }
  });

  $rootScope.$on('NotificationCountEvent', function(event) {
    if($scope.notifications.length === 0){
      NotificationService.getNotificationCount();
      return;
    }
    var count = 0;
    for(var i=0;i<$scope.notifications.length;i++){
      if($scope.notifications[i].count > 0){
        count++;
      }
    }

    NotificationService.setCount(count);
  });

  $rootScope.$on('NotificationChatEvent', function(event, data) {
    $scope.loading = false;

    if(data.notifyAction === 1) {
      $scope.count = data.count;
      $scope.countMobile = $scope.count;
      $rootScope.$apply();
      return;
    } 

    if(data.chats.length === 0 && $scope.notifications.length === 0){
      $scope.noData = true;
      $rootScope.$apply();
      return;
    }
    $scope.noData = false;

    for(var i=0;i<data.chats.length;i++){
      var row = data.chats[i];
      var username = '';
      if (row.type === 2) {
        username = row.game.sender == self.username ? row.game.receiver : row.game.sender;
      } else if (row.type === 1) {
        username = row.chat.sender == self.username ? row.chat.receiver : row.chat.sender;
      }
      
      row.userinfo = FriendService.getFriend(username);
      if (!row.userinfo) continue;
      var index = $scope.getNotifyBoxIndex(username);
      if(index >= 0){
        var notifyBox = $scope.notifications[index];
        $scope.notifications.splice(index, 1);
      }
      $scope.notifications.push(row);
    }
    $rootScope.$apply();

    //call reset count of new notification when it showed
    NotificationService.resetCount();
  });

  var interval = setInterval(function() {
    if (FriendService.ready) {
      clearInterval(interval);
      $scope.getNotification();
    }
  }, 10);

}

function HeaderCtrl($scope, $rootScope, $cookieStore, NotificationService) {
  var self = this;

  self.username = angular.fromJson($cookieStore.get('username'));
  if(self.username.length === 0){
    return;
  }

  $rootScope.$on('NotificationChatHeaderEvent', function(event, data) {
    NotificationService.setCount(data.count);
  });

  NotificationService.getNotificationCount();

  $rootScope.$on('NotificationCountDecreaseEvent', function(event, username) {
    var count = NotificationService.getCount();
    if (count <= 0){
      return;
    }

    NotificationService.setCount(--count);
    NotificationService.decreaseCount();
  });

  //notification of user will be update by client when it had message
  $rootScope.$on('NotificationChatMessageEvent', function(event, data) {

    var username = '';
    if (data.game != null) {
      username = data.game.sender;
    } else if (data.chat != null) {
      username = data.chat.sender;
    }

    if(username === self.username){
      return;
    }

    NotificationService.getNotificationCount();
  });
}

function scrollChatMessage($timeout, WebSocketService) {
  return {
    restrict: 'A',
    link: function(scope, element, attr) {
      var raw = element[0];
      var first = true;
      element.bind('scroll', function() {
        if (0 === raw.scrollTop) {
          var elmp = element.parent();
          var window = $(element).scope().findChatWindow(elmp.attr('data-username'));
          if(window !== null){
            window.loading = true;

            setTimeout(function() {
              window.loading = false;
              console.log(window);
            }, 1000);
            
          }
          var current = parseInt(elmp.attr('data-page')) + 1;
          elmp.attr('data-page', current);
          WebSocketService.getHistory(elmp.attr('data-username'), current, "0");
          
          $(element.find('.chat-typing')[0]).addClass('float-bottom');
          return;
        }

        if(raw.scrollHeight > raw.offsetHeight){
          $(element.find('.chat-typing')[0]).removeClass('float-bottom');
        }
      });

      scope.$watchCollection(attr.scroll, function(newVal) {

        $timeout(function() {
          // console.log(raw.scrollTop);
          // console.log(raw.offsetHeight);
          // console.log(raw.scrollHeight);
          var heightLast = $(element.find('div.chat-message:last')[0]).height() + 30;
          if (first) {
            raw.scrollTop = raw.scrollHeight;
            if (raw.scrollHeight > raw.offsetHeight) {
              first = false;
            }
          }
          if (raw.scrollTop + raw.offsetHeight >= raw.scrollHeight - heightLast) {
            raw.scrollTop = raw.scrollHeight;
          }
        });
      });
    }
  }
}

gtokenApp.service('WebSocketService', ['$cookieStore', '$rootScope', WebSocketService]);
gtokenApp.service('LoginService', ['$rootScope', 'WebSocketService', LoginService]);
gtokenApp.service('FriendService', ['$http', '$rootScope', 'WebSocketService', FriendService]);
gtokenApp.service('GameService', ['$http', '$rootScope', GameService]);

gtokenApp.service('NotificationService', ['$rootScope', 'FriendService', 'WebSocketService', NotificationService]);
gtokenApp.service('MessengerService', MessengerService);
gtokenApp.controller('NotificationCtrl', ['$scope', '$rootScope', '$cookieStore', 'FriendService', 'NotificationService', NotificationCtrl]);
gtokenApp.controller('MessengerCtrl', ['$scope', '$rootScope', '$cookieStore', 'FriendService', 'WebSocketService', 'MessengerService', 'GameService', 'NotificationService', MessengerCtrl]);
gtokenApp.controller('HeaderCtrl', ['$scope', '$rootScope', '$cookieStore', 'NotificationService', HeaderCtrl]);
gtokenApp.directive('chatButtonDirective', ['$cookieStore', '$rootScope', 'LoginService', 'FriendService', chatButtonDirective]);
gtokenApp.directive('userStatusDirective', ['$cookieStore', '$rootScope', 'LoginService', 'FriendService', userStatusDirective]);
gtokenApp.directive('scroll', ['$timeout', 'WebSocketService', scrollChatMessage]);
gtokenApp.directive('autofocus', ['$parse', function ($parse) {
  return {
    link: function (scope, element, attrs) {
      element[0].focus();
      scope.$watch(attrs.isFocus, function (val) {
        if (val) {
          element[0].focus();
        }
      })
    }
  }
}]);
