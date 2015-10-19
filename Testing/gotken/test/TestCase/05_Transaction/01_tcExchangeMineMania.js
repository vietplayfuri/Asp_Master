/**
 * tcExchange.js - testing Exchange function.
 * casperjs test TestCase/05_Transaction/01_tcExchangeMineMania.js --xunit=Reports/Report_ExchangeMineMania.xml
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

var sGoldCredit = 'Gold';
var intMineManiaGoldRate = 100;

//TC01: check validation of exchange amounts
casper.test.begin('\n\n TC01_ExchangeMineMania_Validate_Amount \n', function suite(test) {
    var lstInvalidAmounts = ['-1', '0.1', 'ten.01', ' ', '@#)&', '6*3'];
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
        test.fail('Cannot back to Profile Page');
    });

    // Choose a game and credit type
    casper.waitForText(sTitleExchangeTab, function() {
        casper.acInputExchangeAmount(sSelectedGame, sGoldCredit);
    });

    // Input invalid value to check validation
    casper.then(function() {
        var urlCurrent = casper.getCurrentUrl();
        casper.each(lstInvalidAmounts, function(self, amount) {
            casper.sendKeys(txtExchangeAmount, amount, {
                reset: true
            });
            casper.thenClick(btnExchangeGToken, function() {
                casper.wait(casper.intWaitingTime * 4, function() {
                    if (!(casper.exists(errPositiveInteger))) {
                        lstAmounts += '"' + amount + '", ';
                        bolAmounts = true;
                    };
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
casper.test.begin('\n\n TC02_ExchangeMineMania_Insufficient_Pocket \n', function suite(test) {
    var arrInsufficientAmounts = new Array();
    var lstInsufficientAmounts = 'List of Insufficient Amount exchanged:';
    var bolInsufficientAmounts = false;
    var sCurrentTotalGToken;

    casper.start().reload();
    // Choose a game and credit type
    casper.waitForText(sTitleExchangeTab, function() {
        casper.acInputExchangeAmount(sSelectedGame, sGoldCredit);
    });

    // Input value '0'
    casper.then(function() {
        casper.waitForSelector(txtExchangeAmount, function() {
            casper.sendKeys(txtExchangeAmount, '0', {
                reset: true
            });
        });
        casper.then(function() {
            this.waitForSelector(errExchangeAmountRequire, function() {}, function() {
                lstInsufficientAmounts += '"' + '0' + '", ';
                bolInsufficientAmounts = true;
            }, casper.intMaxWaitingTime);
        });
    });

    // Input Insufficient Gold value to check validation
    casper.then(function() {
        sCurrentTotalGToken = parseFloat(this.fetchText(lblAmoutTotalGtoken));
        var sInsufficentGold = sCurrentTotalGToken * intMineManiaGoldRate + 1;
        this.sendKeys(txtExchangeAmount, sInsufficentGold.toString(), {
            reset: true
        });
        this.then(function() {
            this.waitForSelector(errInsufficientBalance, function() {}, function() {
                lstInsufficientAmounts += '"' + sInsufficentGold + '", ';
                bolInsufficientAmounts = true;
            }, casper.intMaxWaitingTime);
        });
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
casper.test.begin('\n\n TC03_ExchangeMineMania_Calculate_GToken \n', function suite(test) {
    var sCurrentTotalGToken;

    // Input Insufficient Gold value to check validation
    casper.start(function() {
        sCurrentTotalGToken = parseFloat(this.fetchText(lblAmoutTotalGtoken));
        var sSufficentGold = sCurrentTotalGToken * intMineManiaGoldRate;
        this.sendKeys(txtExchangeAmount, sSufficentGold.toString(), {
            reset: true
        });
        this.then(function() {
            this.waitForSelector(msgExchangeConfirm, function() {
                var txtExchangeConfirm = casper.fetchText(msgExchangeConfirm);
                test.assert(txtExchangeConfirm.indexOf(sCurrentTotalGToken) > -1, 'Calculate amount of GToken correctly.');
            }, function () {
                test.fail('Fail to calculate amount of GToken!');
            }, casper.intMaxWaitingTime);
        });
    });

    casper.run(function() {
        test.done();
    });
});

// //TC04: Exchange amount of gems by Free GToken
// casper.test.begin('\n\n TC04_ExchangeMineMania_Buy_By_Free_GToken \n', function suite(test) {
//     var arrAmoutGtoken = new Array();
//     var intGtokenExchanged = 5;
//     var intCreditAmount = 500;

//      // Set GToken amount and return to Home Page
//     casper.start(casper.urlReturnGtoken(sDefaultGToken, sDefaultFreeGToken)).wait(casper.intWaitingTime * 2);

//     // Navigate to Exchange tab to get Gtoken Amount
//     casper.thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
//         arrAmoutGtoken = casper.acOpenExchangeTab();
//     }, function() {
//         test.fail('Cannot navigate to Profile page');
//     });

//     // Choose a game and input amount of package
//     casper.then(function() {
//         casper.waitForText(sTitleExchangeTab, function() {
//             casper.acInputExchangeAmount(sSelectedGame, sGoldCredit, intCreditAmount);
//             this.thenClick(btnExchangeGToken);
//         }, function(){
//             test.fail('Cannot navigate to Exchange Tab');
//         });
//     });

//     // Verify the Exchange transaction
//     casper.then(function() {
//         casper.waitForText(msgExchangeSuccessfully, function() {
//             casper.acVerifyExchangeComplete(arrAmoutGtoken, intGtokenExchanged, 'Free');
//         });
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/exchange/, /You exchanged 500 Gold in Mine Mania using 5 Free GToken/, /-5*/);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC05: Exchange amount of gems by GToken
// casper.test.begin('\n\n TC05_ExchangeMineMania_Buy_By_GToken \n', function suite(test) {
//     var arrAmoutGtoken = new Array();
//     var intGtokenExchanged = 20;
//     var intCreditAmount = 2000;

//     // Navigate to Exchange tab to get Gtoken Amount
//     casper.start().thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
//         arrAmoutGtoken = casper.acOpenExchangeTab();
//     }, function() {
//         test.fail('Cannot navigate to Profile page');
//     });

//     // Choose a game and input amount of package
//     casper.then(function() {
//         casper.waitForText(sTitleExchangeTab, function() {
//             casper.acInputExchangeAmount(sSelectedGame, sGoldCredit, intCreditAmount);
//             this.thenClick(btnExchangeGToken);
//         }, function(){
//             test.fail('Cannot navigate to Exchange Tab');
//         });
//     });

//     // Verify the Exchange transaction
//     casper.then(function() {
//         casper.waitForText(msgExchangeSuccessfully, function() {
//             casper.acVerifyExchangeComplete(arrAmoutGtoken, intGtokenExchanged, 'Normal');
//         });
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/exchange/, /You exchanged 2000 Gold in Mine Mania using 20 GToken/, /-20/);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// // //TC03: Exchange amount of gems by GToken and Free GToken
// // casper.test.begin('\n\n TC03_ExchangeMineMania_Use_Two_Kind_GToken \n', function suite(test) {
// //     var arrAmoutGtoken = new Array();
// //     var intGtokenExchanged = 5;
// //     var intCreditAmount = 1500;

// //     // Navigate to Exchange tab to get Gtoken Amount
// //     casper.start().thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
// //         arrAmoutGtoken = casper.acOpenExchangeTab();
// //     }, function() {
// //         test.fail('Cannot navigate to Profile page');
// //     });

// //     // Choose a game and input amount of package
// //     casper.then(function() {
// //         casper.waitForText(sTitleExchangeTab, function() {
// //             this.acInputExchangeAmount(sSelectedGame, sGoldCredit, intCreditAmount);
// //             this.thenClick(btnExchangeGToken);
// //         }, function(){
// //             test.fail('Cannot navigate to Exchange Tab');
// //         });
// //     });

// //     // Verify the Exchange transaction
// //     casper.then(function() {
// //         casper.waitForText(msgExchangeSuccessfully, function() {
// //             casper.acVerifyExchangeComplete(arrAmoutGtoken, intGtokenExchanged, 'Both');
// //         }, function(){
// //             casper.test.fail('Fail to exchange');
// //         });
// //     });

// //     // Verify Transaction History
// //     casper.then(function(){
// //         casper.acCheckTransactionHistory(/exchange/, /You exchanged 500 Gold in Mine Mania using 5 GToken/, /-5/);
// //     });

// //     casper.run(function() {
// //         test.done();
// //     });
// // });