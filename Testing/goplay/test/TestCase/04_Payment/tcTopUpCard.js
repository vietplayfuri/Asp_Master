/**
 * tcTopUpCard.js - testing buying gtoken by topup card.
 * casperjs test TestCase/04_Payment/tcTopUpCard.js --xunit=Reports/Report_TopUpCard.xml
 * cd gtoken-python
 * . script/set_env.sh
 * psql < sql/test_topupcard.sql
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');
phantom.injectJs('./Interface/itfTopUpTab.js');

//Variables
var pathOriginalTopUpCard = pathTestDataFolder + 'originalTopupCards.json';
var pathTempTopUpCard = pathTestDataFolder + 'tempTopupCards.json';
var sGtokenTopUpCard = 'valid_cards';
var sFreeTopUpCard = 'valid_free_cards';
var sUsedTopUpCard = 'used_cards';
var sExpiredTopUpCard = 'expired_cards';
var jsonTopUpCard;

// Actions
var acOpenTabTopUp = function() {
    var arrAmoutGtoken = new Array();

    // Select one package on transaction page
    casper.then(function() {
        casper.click(tabTransaction);
        casper.waitForSelector(pnlTransactionHistory, function() {
            // Get Amount of each kind of Gtoken
            arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutGtoken)));
            arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutFreeGtoken)));

            // Navigate to Paypal Tab
            this.click(btnTopUp);
            this.waitForSelector(cbxPaymentMethod, function(){
                this.evaluate(function(selector){
                    document.querySelector(selector).selectedIndex = 1;
                    $(selector).change()
                    return true;
                }, cbxPaymentMethod);
            }, function(){
                test.fail('Cannot see the Payment Method cbx!');
            });
        }, function() {
            casper.test.fail('Fail to open TopUp page');
        }, casper.intWaitingTime * 5);
    });
    return arrAmoutGtoken;
};
var acGetTopUpCard = function() {
    var jsonLines = fs.read(pathOriginalTopUpCard);
    fs.write(pathTempTopUpCard, jsonLines, 'w');
};
var acGetTopUpObj = function(typeOfCard) {
    var intCardNumber;
    var jsonTopUpCard = fs.read(pathTempTopUpCard);
    var objTopUpCard = JSON.parse(jsonTopUpCard);
    var objCard;
    if (typeOfCard === sGtokenTopUpCard) {
        objCard = objTopUpCard.valid_cards;
    } else if (typeOfCard === sFreeTopUpCard) {
        objCard = objTopUpCard.valid_free_cards;
    } else if (typeOfCard === sUsedTopUpCard) {
        objCard = objTopUpCard.used_cards;
    } else if (typeOfCard === sExpiredTopUpCard) {
        objCard = objTopUpCard.expired_cards;
    };
    for (var i = objCard.length - 1; i >= 0; i--) {
        if (objCard[i].is_used !== true) {
            casper.sendKeys(txtTopUpCardNumber, objCard[i].card_number, {
                reset: true
            });
            casper.sendKeys(txtTopupCardPassword, objCard[i].card_password, {
                reset: true
            });
            casper.click(btnTopUpCard);
            objCard[i].is_used = true;
            fs.write(pathTempTopUpCard, JSON.stringify(objTopUpCard), 'w');
            intCardNumber = i + 1;
            i = -1;
        };
    };
    return intCardNumber;
};

//TC01: get topup card from sql file
casper.test.begin('\n\n TC01_TopUp_Get_TopUp_Card \n', function suite(test) {
    casper.start().then(function() {
        if (!fs.exists(pathTempTopUpCard)) {
            jsonTopUpCard = acGetTopUpCard();
            casper.test.pass('Getting Topup Cards List successfully. Save at ' + pathTempTopUpCard);
        } else {
            casper.test.skip(1, 'The Topup Cards have been got already. Save at ' + pathTempTopUpCard);
        };
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: buy Gtoken by TopUp Card
casper.test.begin('\n\n TC02_TopUp_Buy_Gtoken \n', function suite(test) {
    var intIncreaseGtoken = 10;
    var arrAmoutGtoken = new Array();
    var intCardNumber;

    // Sign out the previous session
    casper.start().then(function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    });

    // Login with account foxymax/123
    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sUsername, sPassword);
    });

    casper.then(function() {
        arrAmoutGtoken = acOpenTabTopUp();
    });

    casper.waitForSelector(txtTopUpCardNumber, function(){
        intCardNumber = acGetTopUpObj(sGtokenTopUpCard);
    }, function(){
        casper.test.fail('Fail to open Top Up Card tab!')
    });

    casper.then(function() {
        casper.waitForText('You topped up 10 Play Token', function() {
            casper.thenClick(btnCloseNotification).reload(function(){
                casper.acVerifyTransactionComplete(arrAmoutGtoken, intIncreaseGtoken, 'Normal', 'plus');
            });
        }, function() {
            if (typeof intCardNumber === "undefined") {
                casper.test.fail('All of the ' + sGtokenTopUpCard + ' have been used. Please reset the cards!');
            } else {
                if (casper.exists(errTopUpCardUsed)) {
                    casper.test.fail('The Card ' + sGtokenTopUpCard + intCardNumber + ' has been used already.');
                } else {
                    casper.test.fail('The Top Up Card Transaction cannot be completed.');
                };
            };
        }, casper.intMaxWaitingTime);
    });

    // Verify Transaction History
    casper.then(function(){
        casper.acCheckTransactionHistory('topup', 'Top-up', 10);
    });

    // // Back to Profile Tab
    // casper.thenClick(tabTransaction).waitForSelector(dateTransaction, function() {
    //     var sdateTransaction = casper.evaluate(function(selector) {
    //         return document.querySelectorAll(selector)[0].getAttribute('src');
    //     }, dateTransaction);
    //     if (sdateTransaction.indexOf('月')) {
    //         test.pass('The Transaction Date has been locailized.')
    //     } else {
    //         test.fail('Fail to localized transaction date!');
    //     };
    // }, function() {
    //     test.fail('Cannot back to Profile Page');
    // });

    casper.run(function() {
        test.done();
    });
});

//TC03: buy Free Gtoken by TopUp Card
casper.test.begin('\n\n TC03_TopUp_Buy_Free_Gtoken \n', function suite(test) {
    var intIncreaseGtoken = 10;
    var arrAmoutFreeGtoken;
    var intCardNumber;

    casper.start().then(function() {
        arrAmoutFreeGtoken = acOpenTabTopUp();
    });

    casper.waitForSelector(txtTopUpCardNumber, function(){
        intCardNumber = acGetTopUpObj(sFreeTopUpCard);
    }, function(){
        casper.test.fail('Fail to open Top Up Card tab!')
    });

    casper.then(function() {
        casper.waitForText('You topped up 10 Free Play Token', function() {
            casper.thenClick(btnCloseNotification).reload(function(){
                casper.acVerifyTransactionComplete(arrAmoutFreeGtoken, intIncreaseGtoken, "Free", 'plus');
            });
        }, function() {
            if (typeof intCardNumber === "undefined") {
                casper.test.fail('All of the ' + sFreeTopUpCard + ' have been used. Please reset the cards');
            } else {
                if (casper.exists(errTopUpCardUsed)) {
                    casper.test.fail('The Card ' + sFreeTopUpCard + intCardNumber + ' has been used already.');
                } else {
                    casper.test.fail('The Top Up Card Transaction cannot be completed.');
                };
            };
        }, casper.intMaxWaitingTime);
    });

    // Verify Transaction History
    casper.then(function(){
        casper.acCheckTransactionHistory('topup', 'Top-up', 10);
    });

    casper.run(function() {
        test.done();
    });
});

//TC04: input the used TopUp Card
casper.test.begin('\n\n TC04_TopUp_Input_Used_Card \n', function suite(test) {
    var intCardNumber;

    casper.start().then(function() {
        acOpenTabTopUp();
    });

    casper.waitForSelector(txtTopUpCardNumber, function(){
        intCardNumber = acGetTopUpObj(sUsedTopUpCard);
    }, function(){
        casper.test.fail('Fail to open Top Up Card tab!')
    });

    casper.then(function() {
        casper.waitForSelector(errTopUpCardUsed, function() {
            casper.test.pass('The User cannot use ' + sUsedTopUpCard + intCardNumber + '.');
        }, function() {
            casper.test.fail('The ' + sUsedTopUpCard + intCardNumber + ' has been used.');
        }, casper.intMaxWaitingTime);
    });

    casper.run(function() {
        test.done();
    });
});

//TC05: input the Expired TopUp Card
casper.test.begin('\n\n TC05_TopUp_Input_Expired_Card \n', function suite(test) {
    var intCardNumber;

    casper.start().then(function() {
        acOpenTabTopUp();
    });

    casper.waitForSelector(txtTopUpCardNumber, function(){
        intCardNumber = acGetTopUpObj(sExpiredTopUpCard);
    }, function(){
        casper.test.fail('Fail to open Top Up Card tab!')
    });

    casper.then(function() {
        casper.waitForSelector(errTopUpCardExpired, function() {
            casper.test.pass('The User cannot use ' + sExpiredTopUpCard + intCardNumber + '.');
        }, function() {
            casper.test.fail('The  ' + sExpiredTopUpCard + intCardNumber + ' has been used.');
        }, casper.intMaxWaitingTime);
    });

    casper.run(function() {
        test.done();
    });
});