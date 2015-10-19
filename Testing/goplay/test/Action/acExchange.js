casper.test.comment("Loading Exchange Actions...");

// casper.acSelectByText = function(selector, textToFind) {
//     casper.then(function() {
//         var itemXapth = x('(//div[@class="select-row"]/div/span[contains(text(), "' + textToFind + '")])[1]');
//         if (!(casper.exists(itemXapth))) {
//             casper.thenClick(selector, function() {
//                 casper.waitUntilVisible(itemXapth, function() {
//                     casper.click(itemXapth);
//                     // if (casper.exists(itemXapth)) {
//                     //     casper.click(itemXapth);
//                     // } else {
//                     //     casper.test.fail('Cannot not select Game Item'); 
//                     // };    
//                 }, function(){
//                     casper.test.fail('Cannot not select Game Item'); 
//                 });
//             });
//         };
//     });
// }

casper.acSelectGameItem = function(game, gamePackage, amount) {
    casper.then(function() {
        if (typeof amount === 'undefined') {
            amount = 'default';
        }
        casper.then(function() {
            casper.sendKeys(txtSearchGame, game);
        });

        casper.wait(3000, function(){
            casper.waitUntilVisible(lnkGame(game),function(){
                casper.thenClick(lnkGame(game), function(){
                    casper.waitUntilVisible(lnkGamePackage(gamePackage), function() {
                        casper.click(lnkGamePackage(gamePackage));
                    }, function(){
                        casper.test.fail('Cannot load package list!');
                    }); 
                });
            }, function(){
                casper.test.fail('Cannot load game list!');
            });
        });

        casper.then(function() {
            if (!(amount == 'default')) {
                casper.sendKeys(txtGameCredit(gamePackage), amount.toString(), {
                    reset: true
                });
            };
        });

    });
};

casper.acOpenExchangeTab = function() {
    var arrAmoutGtoken = new Array();

    // Select one package on transaction page
    casper.then(function() {
        casper.click(tabTransaction);
        casper.waitForSelector(pnl_exchange, function() {
            // Get Amount of each kind of Gtoken
            arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutGtoken)));
            arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutFreeGtoken)));
            casper.click(btnExchange);            
        }, function() {
            casper.test.fail('Fail to open Transaction Tab');
        }, casper.intMaxWaitingTime);
    });
    return arrAmoutGtoken;
};
casper.urlReturnGtoken = function(playToken, FreePlayToken) {
    var url = urlHomepage +'resetdb/reset-balance?play_token=' + playToken + '&free_play_token=' + FreePlayToken;
    return url;
};