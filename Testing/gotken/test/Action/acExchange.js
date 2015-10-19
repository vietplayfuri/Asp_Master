casper.test.comment("Loading Exchange Actions...");

casper.acSelectByText = function(selector, textToFind) {
    casper.then(function() {
        var itemXapth = x('(//div[@class="select-row"]/div/span[contains(text(), "' + textToFind + '")])[1]');
        if (!(casper.exists(itemXapth))) {
            casper.thenClick(selector, function() {
                casper.waitUntilVisible(itemXapth, function() {
                    casper.click(itemXapth);
                    // if (casper.exists(itemXapth)) {
                    //     casper.click(itemXapth);
                    // } else {
                    //     casper.test.fail('Cannot not select Game Item'); 
                    // };    
                }, function(){
                    casper.test.fail('Cannot not select Game Item'); 
                });
            });
        };
    });
}
casper.acInputExchangeAmount = function(game, gamePackage, amount) {
    casper.then(function() {
        if (typeof amount === 'undefined') {
            amount = 'default';
        }
        casper.then(function() {
            casper.acSelectByText(cbxExchangeSelectGame, game);
        });
        casper.then(function() {
            casper.wait(1000, function() {
                casper.acSelectByText(cbxExchangeSelectItem, gamePackage);
            });
        });
        casper.then(function() {
            if (!(amount == 'default')) {
                casper.sendKeys(txtExchangeAmount, amount.toString(), {
                    reset: true
                });
            };
        });

    });
};
casper.acVerifyExchangeComplete = function(AmountGtoken, Gtoken, status) {
    // Check Point for Transaction
    casper.then(function() {
        // Navigate to Paypal Tab
        casper.click(tabTransaction);
        casper.waitForText(sTitleTransactionTab, function() {
            var intAmoutGtoken = this.fetchText(lblAmoutGtoken);
            var intAmoutFreeGtoken = this.fetchText(lblAmoutFreeGtoken);
            var intAmoutTotalGtoken = this.fetchText(lblAmoutTotalGtoken);
            if (status == 'Normal') {
                var intExpectedGtoken = (AmountGtoken[0]*1000 - Gtoken*1000)/1000;
                this.test.assert(intAmoutGtoken == intExpectedGtoken, 'Exchange ' + Gtoken + ' GToken to credit type (' + AmountGtoken[0] + '=>' + intAmoutGtoken + ')');
                this.test.assert(intAmoutFreeGtoken == AmountGtoken[1], 'The Free Gtoken not changed');
            } else if (status == 'Free') {
                var intExpectedGtoken = (AmountGtoken[1]*1000 - Gtoken*1000)/1000;
                this.test.assert(intAmoutFreeGtoken == intExpectedGtoken, 'Exchange ' + Gtoken + ' Free GToken to credit type (' + AmountGtoken[1] + '=>' + intAmoutFreeGtoken + ')');
                this.test.assert(intAmoutGtoken == AmountGtoken[0], 'The Gtoken not changed');
            } else if(status == 'Both'){
                var intExpectedGtoken = (AmountGtoken[0]*1000 - Gtoken*1000)/1000;
                this.test.assert(intAmoutGtoken == intExpectedGtoken, 'Exchange all free gtoken and ' + Gtoken + ' GToken to credit type (' + AmountGtoken[0] + '=>' + intAmoutGtoken + ')');
                this.test.assert(intAmoutFreeGtoken == 0, 'The Free Gtoken has been run out');
            };
            this.test.assert(intAmoutTotalGtoken == (+intAmoutGtoken) + (+intAmoutFreeGtoken), 'The Total Gtoken equals to Gtoken plus Free Gtoken');
        });
    });
};
casper.acOpenExchangeTab = function() {
    var arrAmoutGtoken = new Array();

    // Select one package on transaction page
    casper.then(function() {
        casper.click(tabTransaction);
        casper.waitForSelector(btnExchange, function() {
            // Get Amount of each kind of Gtoken
            arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutGtoken)));
            arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutFreeGtoken)));
            arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutTotalGtoken)));
            casper.click(btnExchange);            
        }, function() {
            casper.test.fail('Fail to open Transaction Tab');
        }, casper.intMaxWaitingTime);
    });
    return arrAmoutGtoken;
};
casper.urlReturnGtoken = function(GToken, FreeGtoken) {
    var url = urlHomepage +'resetdb/reset-balance?play_token=' + GToken + '&free_play_token=' + FreeGtoken;
    return url;
};