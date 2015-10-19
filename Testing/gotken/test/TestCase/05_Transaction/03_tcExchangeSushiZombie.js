/**
 * tcExchange.js - testing Exchange Function.
 * casperjs test TestCase/05_Transaction/03_tcExchangeSushiZombie.js --xunit=Reports/Report_ExchangesSushiZombie.xml
 */

//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Action/acExchange.js');
phantom.injectJs('./Interface/itfExchangeTab.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

var sSelectedGame = 'Sushi Zombie';
var sSelectedPackage = 'Gold';
var sDefaultGToken = '19';
var sDefaultFreeGToken = '10';

//TC01: Check that unable to buy package if insufficent amount
casper.test.begin('\n\n TC01_ExchangeSushiZombie_Insufficient_Pocket \n', function suite(test) {
    var arrAmoutGtoken = new Array();
    var sSelectedAmount = '29001';

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
        casper.acInputExchangeAmount(sSelectedGame, sSelectedPackage, sSelectedAmount);
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

// //TC02: Exchange amount of gems by GToken and Free GToken
// casper.test.begin('\n\n TC02_ExchangeSushiZombie_Use_Two_Kind_GToken \n', function suite(test) {
//     var arrAmoutGtoken = new Array();
//     var intGtokenExchanged = 5.555;
//     var sSelectedAmount = 15555;

//     // Navigate to Exchange tab to get Gtoken Amount
//     casper.start().thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
//         arrAmoutGtoken = casper.acOpenExchangeTab();
//     }, function() {
//         test.fail('Cannot navigate to Profile page');
//     });

//     // Choose a game and input amount of package
//     casper.then(function() {
//         casper.waitForText(sTitleExchangeTab, function() {
//             casper.acInputExchangeAmount(sSelectedGame, sSelectedPackage, sSelectedAmount);
//             this.thenClick(btnExchangeGToken);
//         }, function(){
//             test.fail('Cannot navigate to Exchange Tab');
//         });
//     });

//     // Verify the Exchange transaction
//     casper.then(function() {
//         casper.waitForText(msgExchangeSuccessfully, function() {
//             casper.acVerifyExchangeComplete(arrAmoutGtoken, intGtokenExchanged, 'Both');
//         }, function(){
//             casper.test.fail('Fail to exchange');
//         });
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/exchange/, /You exchanged 5555 Gold in Sushi Zombie using 5.555 GToken/, /-5.555/);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC03: Check that able to buy a packge by using Free GToken
// casper.test.begin('\n\n TC03_ExchangeSushiZombie_Buy_Package_By_Free_GToken \n', function suite(test) {
//     var arrAmoutGtoken = new Array();
//     var intGtokenExchanged = 8.999;
//     var intSelectedAmount = 8999;

//     // Set GToken amount and return to Home Page
//     casper.start(casper.urlReturnGtoken(sDefaultGToken, sDefaultFreeGToken)).wait(casper.intWaitingTime * 2);

//     // Navigate to Profile tab to get Gtoken Amount
//     casper.thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
//         arrAmoutGtoken = casper.acOpenExchangeTab();
//     }, function() {
//         test.fail('Cannot back to Profile Page. Get Amount Gtoken');
//     });

//     // Choose a game and input amount of package
//     casper.then(function() {
//         casper.waitForText(sTitleExchangeTab, function() {
//             casper.acInputExchangeAmount(sSelectedGame, sSelectedPackage, intSelectedAmount);
//         });
//     });

//     casper.thenClick(btnExchangeGToken);

//     // Verify the Exchange transaction
//     casper.then(function() {
//         casper.waitForText(msgExchangeSuccessfully, function() {
//             casper.acVerifyExchangeComplete(arrAmoutGtoken, intGtokenExchanged, 'Free');
//         }, function(){
//         	casper.test.fail('The exchange transaction is unsuccessful!');
//         }, casper.intMaxWaitingTime);
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/exchange/, /You exchanged 8999 Gold in Sushi Zombie using 8.999 Free GToken/, /-8.999*/);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC04: Check that able to buy a packge by using GToken
// casper.test.begin('\n\n TC04_ExchangeSushiZombie_Buy_Package_By_GToken \n', function suite(test) {
//     var arrAmoutGtoken = new Array();
//     var intSelectedAmount = 17777;
//     var intGtokenExchanged = 17.777;

//      // Set GToken amount and return to Home Page
//     casper.start(casper.urlReturnGtoken(sDefaultGToken, 0)).wait(casper.intWaitingTime * 2);

//     // Navigate to Profile tab to get Gtoken Amount
//     casper.thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
//         arrAmoutGtoken = casper.acOpenExchangeTab();
//     }, function() {
//         test.fail('Cannot back to Profile Page. Get Amount Gtoken');
//     });

//     // Choose a game and input amount of package
//     casper.then(function() {
//         casper.waitForText(sTitleExchangeTab, function() {
//             casper.acInputExchangeAmount(sSelectedGame, sSelectedPackage, intSelectedAmount);
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
//         casper.acCheckTransactionHistory(/exchange/, /You exchanged 17777 Gold in Sushi Zombie using 17.777 GToken/, /-17.777/);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });