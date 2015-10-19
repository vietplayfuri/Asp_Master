/**
 * tcExchange.js - testing Exchange function.
 * casperjs test TestCase/05_Transaction/02_tcExchangePackage.js --xunit=Reports/Report_ExchangePackage.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Action/acExchange.js');
phantom.injectJs('./Interface/itfExchangeTab.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

var sDefaultGToken = '1';
var sDefaultFreeGToken = '1';
var sSelectedGame = 'Pocket Heroes';
var sGamePackage = '3000 coins';

//TC01: check validation of exchange amounts
casper.test.begin('\n\n TC01_ExchangePackage_Validate_Amount \n', function suite(test) {
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
    casper.waitForSelector(txtSearchGame, function() {
        casper.acSelectGameItem(sSelectedGame, sGamePackage);
    }, function(){
        test.fail('Cannot open Exchange Tab!');
    });

    casper.waitUntilVisible(errExchange, function() {
        test.assertSelectorHasText(errExchange, "Insufficient Balance", 'Validate correctly.')
    }, function(){
        casper.test.fail('Fail to validate purchase!');
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Send the amount be exceeded the pocket
casper.test.begin('\n\n TC02_ExchangePackage_Purchase_Item \n', function suite(test) {
    var sDefaultGToken_2 = 3;
    var intGtokenExchanged = 2.99;
    var arrAmoutGtoken = new Array();

    // Set GToken amount and return to Home Page
    casper.start(casper.urlReturnGtoken(sDefaultGToken_2, sDefaultFreeGToken)).wait(casper.intWaitingTime * 2);

    // Navigate to Exchange Tab
    casper.waitForSelector(tabProfile, function(){
        casper.thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
            arrAmoutGtoken = casper.acOpenExchangeTab();
        }, function() {
            test.fail('Cannot open to Profile Page!');
        });
    }, function(){
        casper.test.fail('Cannot reload after getting goplay token!')
    });
    

    // Choose a game and credit type
    casper.waitForSelector(txtSearchGame, function() {
        casper.acSelectGameItem(sSelectedGame, sGamePackage);
    }, function(){
        test.fail('Cannot open Exchange Tab!');
    });

    casper.waitUntilVisible(btnConfirmExchange, function(){
        casper.thenClick(btnConfirmExchange, function(){
            // Verify the Exchange transaction
            casper.waitUntilVisible(lblExchangeConfirm, function() {
                casper.waitWhileVisible(lblExchangeConfirm, function(){
                    casper.acVerifyExchangeComplete(arrAmoutGtoken, intGtokenExchanged, 'Normal');
                });
            }, function(){
                casper.test.fail('Cannot complete the exchange transaction!');
            });

            // Verify Transaction History
            casper.then(function(){
                casper.acCheckTransactionHistory('exchange', 'Exchange for 3000 coins in Pocket Heroes', '-2.99');
            });
        });
    }, function(){
        casper.test.fail('Cannot see the confirm exchange button!');
    });

    casper.run(function() {
        test.done();
    });
});
