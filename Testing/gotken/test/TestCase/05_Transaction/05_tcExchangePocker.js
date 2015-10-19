/**
 * tcExchange.js - testing Exchange Function.
 * casperjs test TestCase/05_Transaction/05_tcExchangePocker.js --xunit=Reports/Report_ExchangePocker.xml
 */

//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Action/acExchange.js');
phantom.injectJs('./Interface/itfExchangeTab.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

var sSelectedGame = 'Super Poker';
var sDefaultGToken = '19';
var sDefaultFreeGToken = '10';

//TC01: Check that unable to buy package if insufficent amount
casper.test.begin('\n\n TC01_ExchangePocker_Insufficient_Pocket \n', function suite(test) {
    var sSelectedPackage = 'Bag of 5000 diamond';

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

    // Choose a game and credit type (Gold)
    casper.waitForText(sTitleExchangeTab, function() {
        casper.acInputExchangeAmount(sSelectedGame, sSelectedPackage);
    });

    casper.then(function() {
    	casper.wait(3000, function(){
    		test.assertExists(errInsufficientBalance, 'Cannot buy a package with insufficient amount');
    	});  
    });

    casper.run(function() {
        test.done();
    });
});

// //TC02: Check that able to buy a packge by using Free GToken
// casper.test.begin('\n\n TC02_ExchangePocker_Buy_Package_By_Free_GToken \n', function suite(test) {
//     var sSelectedPackage = 'Bag of 100 diamond';
//     var arrAmoutGtoken = new Array();
//     var intGtokenExchanged = 7;

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
//         });
//     });

//     casper.thenClick(btnExchangeGToken);

//     // Verify the Exchange transaction
//     casper.then(function() {
//         casper.waitForText(msgExchangeSuccessfully, function() {
//             casper.acVerifyExchangeComplete(arrAmoutGtoken, intGtokenExchanged, 'Free');
//         }, function(){
//          casper.test.fail('The exchange transaction is unsuccessful!');
//         }, casper.intMaxWaitingTime * 2);
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/exchange/, /Bag of 100 diamond in Super Poker using 7 Free GToken/, /-7*/);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC03: Check that able to buy a packge by using GToken
// casper.test.begin('\n\n TC03_ExchangePocker_Buy_Package_By_GToken \n', function suite(test) {
//     var sSelectedPackage = 'Bag of 300 diamond';
//     var arrAmoutGtoken = new Array();
//     var intGtokenExchanged = 17;

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
//          casper.test.fail('The exchange transaction is unsuccessful.');
//         }, casper.intMaxWaitingTime * 2);
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/exchange/, /Bag of 300 diamond in Super Poker using 17 GToken/, /-17/);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });