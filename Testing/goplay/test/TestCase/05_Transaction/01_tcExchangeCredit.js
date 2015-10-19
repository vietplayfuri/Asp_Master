/**
 * tcExchange.js - testing Exchange function.
 * casperjs test TestCase/05_Transaction/01_tcExchangeCredit.js --xunit=Reports/Report_ExchangeCredit.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Action/acExchange.js');
phantom.injectJs('./Interface/itfExchangeTab.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

var sDefaultGToken = '20';
var sDefaultFreeGToken = '10';
var sSelectedGame = 'Mine Mania';
var sGoldCredit = '100 Gold = 1 Play Token';

var intMineManiaGoldRate = 100;

//TC01: check validation of exchange amounts
casper.test.begin('\n\n TC01_ExchangeCredit_Validate_Amount \n', function suite(test) {
    var lstInvalidAmounts = ['-1', '@#)&', 'ten.01', '1.0', '2 2', '6*3'];
    var lstAmounts = 'List of Invalid Amount exchanged:';
    var bolAmounts = false;
    var arrAmoutGtoken = new Array();

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function() {
        casper.test.fail('Cannot open Home Page.');
    });

    // Sign in by Exchange Account
    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sExchangeUsername, sExchangePassword);
    });

    // Set GToken amount and return to Home Page
    casper.thenOpen(casper.urlReturnGtoken(sDefaultGToken, sDefaultFreeGToken)).wait(casper.intWaitingTime * 2);

    // Navigate to Exchange Tab
    casper.thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
        casper.acOpenExchangeTab();
    }, function() {
        test.fail('Cannot back to Profile Page!');
    });

    // Choose a game and credit type
    casper.waitForText(sTitleExchangeTab, function() {
        casper.then(function() {
            casper.sendKeys(txtSearchGame, sSelectedGame);
        });

        casper.then(function(){
            casper.waitUntilVisible(lnkGame(sSelectedGame),function(){
                casper.click(lnkGame(sSelectedGame));
            });
        });
    }, function(){
        test.fail('Cannot open Exchange Tab!');
    });

    // Input invalid value to check validation
    casper.then(function() {
        var urlCurrent = casper.getCurrentUrl();
        casper.each(lstInvalidAmounts, function(self, amount) {
            casper.waitForSelector(txtGameCredit(sGoldCredit),function(){
                casper.then(function(){
                    if (this.exists(errExchange)) {
                        casper.sendKeys(txtGameCredit(sGoldCredit), '2', {
                            reset: true
                        });
                        casper.waitWhileVisible(errExchange, function(){
                        });
                    };
                });

                casper.sendKeys(txtGameCredit(sGoldCredit), amount, {
                    reset: true
                });
                casper.then(function() {
                    casper.waitUntilVisible(errExchange, function() {
                        sErrorExchange = casper.evaluate(function(selector) {
                            return document.querySelector(selector).textContent;
                        }, errExchange);
                        if (!(sErrorExchange == "Exchange amount needs to be a positive integer")) {
                            lstAmounts += '"' + amount + '", ';
                            bolAmounts = true;
                        };
                    }, function(){
                        casper.test.fail('Fail to check at turn: ' + amount);
                    });
                });
            });
        });    
    });

    casper.then(function() {
        if (bolAmounts) {
            casper.test.fail(lstAmounts);
        } else {
            casper.test.pass('The Gtoken Amount exchanged are validated');
        };
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Send the amount be exceeded the pocket
casper.test.begin('\n\n TC02_ExchangeCredit_Insufficient_Pocket \n', function suite(test) {
    var arrInsufficientAmounts = new Array();
    var lstInsufficientAmounts = 'List of Insufficient Amount exchanged:';
    var bolInsufficientAmounts = false;
    var intPlayToken;

    casper.start().reload();

    // Input value '0'
    casper.then(function() {
        casper.waitForSelector(txtGameCredit(sGoldCredit), function() {
            casper.sendKeys(txtGameCredit(sGoldCredit), '0', {
                reset: true
            });
        });
    });

    casper.waitUntilVisible(errExchange, function() {
        sErrorExchange = casper.evaluate(function(selector) {
            return document.querySelector(selector).textContent;
        }, errExchange);
        if (!(sErrorExchange == "Exchange amount is required")) {
            lstInsufficientAmounts += '"' + '0' + '", ';
            bolInsufficientAmounts = true;
        };
    }, function(){
        casper.test.fail('Fail to check at turn: ' + '0');
    });

    //Reload Page
    casper.reload();

    // Input Insufficient Gold value to check validation
    casper.then(function() {
        casper.wait(3000, function() {
            intPlayToken = parseFloat(this.fetchText(lblAmoutGtoken)) * intMineManiaGoldRate + 1
            casper.sendKeys(txtGameCredit(sGoldCredit), intPlayToken.toString(), {
                reset: true
            });
        });
    });

    casper.waitUntilVisible(errExchange, function() {
        sErrorExchange = casper.evaluate(function(selector) {
            return document.querySelector(selector).textContent;
        }, errExchange);
        if (!(sErrorExchange == "Insufficient Balance")) {
            lstInsufficientAmounts += '"' + intPlayToken + '", ';
            bolInsufficientAmounts = true;
        };
    }, function(){
        casper.test.fail('Fail to check at turn: ' + intPlayToken);
    });

    //Verify Point 
    casper.then(function() {
        if (bolInsufficientAmounts) {
            casper.test.fail(lstInsufficientAmounts);
        } else {
            casper.test.pass('Cannot exchange by insufficient amount');
        };
    });

    casper.run(function() {
        test.done();
    });
});

//TC03: Send the amount be exceeded the pocket
casper.test.begin('\n\n TC03_ExchangeCredit_Calculate_GToken \n', function suite(test) {
    var intPlayToken;
    var intCreditAmount;

    casper.start().reload();

    // Input value
    casper.then(function() {
        casper.wait(3000, function() {
            intPlayToken = parseFloat(this.fetchText(lblAmoutGtoken))
            intCreditAmount = intPlayToken  * intMineManiaGoldRate
            casper.sendKeys(txtGameCredit(sGoldCredit), intCreditAmount.toString(), {
                reset: true
            });
        });
    });

    casper.then(function() {
        test.assertSelectorHasText(btnExchangeGToken, intPlayToken, 'Calculate successfully.')
    });

    casper.run(function() {
        test.done();
    });
});
