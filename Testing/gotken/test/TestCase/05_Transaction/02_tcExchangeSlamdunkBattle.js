/**
 * tcExchange.js - testing Exchange Function.
 * casperjs test TestCase/05_Transaction/02_tcExchangeSlamdunkBattle.js --xunit=Reports/Report_ExchangeSlamdunkBattle.xml
 */

//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Action/acExchange.js');
phantom.injectJs('./Interface/itfExchangeTab.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

var sSelectedGame = 'Slamdunk Battle';

//TC01: Check that unable to buy package if insufficent amount
casper.test.begin('\n\n TC01_ExchangesSlamdunkBattle_Exceed_Pocket \n', function suite(test) {
    var arrAmoutGtoken = new Array();
    var sDefaultGToken = '367';
    var sDefaultFreeGToken = '300';
    var sSelectedPackage = '22,260 Diamonds';

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
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
        casper.acInputExchangeAmount(sSelectedGame, sSelectedPackage);
    });

    casper.then(function() {
    	casper.wait(1000, function(){
    		test.assertExists(errInsufficientBalance, 'Cannot buy pocket with insufficient amount');
    	});  
    });

    casper.run(function() {
        test.done();
    });
});

// //TC02: Check that able to buy a packge by using Free GToken
// casper.test.begin('\n\n TC02_ExchangesSlamdunkBattle_Buy_Package_By_Free_GToken \n', function suite(test) {
//     var arrAmoutGtoken = new Array();
//     var sSelectedPackage = '330 Diamonds';
//     var intGtokenExchanged = 10;

//     // Set GToken amount and return to Home Page
//     casper.start(casper.urlReturnGtoken('40', '20')).wait(casper.intWaitingTime * 2);

//     // Navigate to Profile tab to get Gtoken Amount
//     casper.thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
//         arrAmoutGtoken = casper.acOpenExchangeTab();
//     }, function() {
//         test.fail('Cannot back to Profile Page. Get Amount Gtoken');
//     });

//     // Choose a game and input amount of package
//     casper.then(function() {
//         casper.waitForText(sTitleExchangeTab, function() {
//             casper.acInputExchangeAmount(sSelectedGame, sSelectedPackage);
//             this.thenClick(btnExchangeGToken);
//         });
//     });

//     // Verify the Exchange transaction
//     casper.then(function() {
//         casper.waitForText(msgExchangeSuccessfully, function() {
//             casper.acVerifyExchangeComplete(arrAmoutGtoken, intGtokenExchanged, 'Free');
//         }, function(){
//         	casper.test.fail('The exchange transaction is unsuccessful.');
//         });
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/exchange/, /You exchanged  330 Diamonds in Slamdunk Battle using 10 Free GToken/, /-10*/);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC03: Check that able to buy a packge by using GToken
// casper.test.begin('\n\n TC03_ExchangesSlamdunkBattle_Buy_Package_By_GToken \n', function suite(test) {
//     var arrAmoutGtoken = new Array();
//     var sSelectedPackage = '1,110 Diamonds';
//     var intGtokenExchanged = 34;

//     // Navigate to Profile tab to get Gtoken Amount
//     casper.start().thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
//         arrAmoutGtoken = casper.acOpenExchangeTab();
//     }, function() {
//         test.fail('Cannot back to Profile Page. Get Amount Gtoken');
//     });

//     // Choose a game and input amount of package
//     casper.then(function() {
//         casper.waitForText(sTitleExchangeTab, function() {
//             casper.acInputExchangeAmount(sSelectedGame, sSelectedPackage);
//             this.thenClick(btnExchangeGToken);
//         });
//     });

//     // Verify the Exchange transaction
//     casper.then(function() {
//         casper.waitForText(msgExchangeSuccessfully, function() {
//             casper.acVerifyExchangeComplete(arrAmoutGtoken, intGtokenExchanged, 'Normal');
//         }, function(){
//         	casper.test.fail('The exchange transaction is unsuccessful.');
//         });
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/exchange/, /You exchanged  1,110 Diamonds in Slamdunk Battle using 34 GToken/, /-34/);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });