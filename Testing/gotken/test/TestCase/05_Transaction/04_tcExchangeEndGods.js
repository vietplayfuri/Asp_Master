/**
 * tcExchange.js - testing Exchange Function.
 * casperjs test TestCase/05_Transaction/04_tcExchangeEndGods.js --xunit=Reports/Report_ExchangeEndGods.xml
 */

//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Action/acExchange.js');
phantom.injectJs('./Interface/itfExchangeTab.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

var sSelectedGame = 'Endgods';
var sSelectedPackage = 'Gems';
var sDefaultGToken = '19';
var sDefaultFreeGToken = '10';
var intEndGodGemRate = 6;

//TC01: Check that unable to buy package if insufficent amount
casper.test.begin('\n\n TC01_ExchangeEndgods_Insufficient_Pocket \n', function suite(test) {
    var arrAmoutGtoken = new Array();
    var sSelectedAmount;

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
        var sCurrentTotalGToken = parseFloat(this.fetchText(lblAmoutTotalGtoken));
        sSelectedAmount = sCurrentTotalGToken * intEndGodGemRate + 1;
        casper.acOpenExchangeTab();
    }, function() {
        test.fail('Cannot back to Profile Page');
    });

    // Choose a game and credit type (Gold)
    casper.waitForText(sTitleExchangeTab, function() {
        casper.acInputExchangeAmount(sSelectedGame, sSelectedPackage, sSelectedAmount);
    });

    casper.then(function() {
    	casper.wait(3000, function(){
    		test.assertExists(errInsufficientBalance, 'Cannot buy pocket with insufficient amount');
    	});  
    });

    casper.run(function() {
        test.done();
    });
});

// //TC02: Check that able to buy gems by using Free GToken
// casper.test.begin('\n\n TC02_ExchangeEndGods_Buy_Gems_By_Free_GToken \n', function suite(test) {
//     var arrAmoutGtoken = new Array();
//     var intGtokenExchanged = 8.833;
//     var intSelectedAmount = 53;

//     // Set GToken amount and return to Home Page
//     casper.start(casper.urlReturnGtoken(sDefaultGToken, sDefaultFreeGToken)).wait(casper.intWaitingTime * 2);

//     // Navigate to Profile tab to get Gtoken Amount
//     casper.waitForText(sTitleProfilePage, function() {
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
//         casper.acCheckTransactionHistory('exchange', 'You exchanged 53 Gems in '+ sSelectedGame +' using 8.833 Free GToken', '-8.833*');
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC03: Check that able to buy gems by using Free GToken
// casper.test.begin('\n\n TC03_ExchangeEndGods_Buy_Gold_By_GToken \n', function suite(test) {
//     var arrAmoutGtoken = new Array();
//     var intGtokenExchanged = 8.978;
//     var intSelectedAmount = 9876;
//     var timeDate = new Date();
//     var sCurrentMonth = timeDate.getMonth().toString();
//     sSelectedPackage = 'Gold';

//     // Navigate to Profile tab to get Gtoken Amount
//     casper.start().thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
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
//             casper.acVerifyExchangeComplete(arrAmoutGtoken, intGtokenExchanged, 'Normal');
//         }, function(){
//             casper.test.fail('The exchange transaction is unsuccessful!');
//         }, casper.intMaxWaitingTime);
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory('exchange', 'You exchanged 9876 Gold in '+ sSelectedGame+ ' using 8.978 GToken', '-8.978');
//     });

//     // Back to Profile Tab
//     casper.then(function() {
//         var sdateTransaction = casper.evaluate(function(selector) {
//             return document.querySelectorAll(selector)[0].getAttribute('src');
//         }, dateTransaction);
//         if (sdateTransaction.indexOf(sCurrentMonth + ' 月')) {
//             test.pass('The Transaction Date has been locailized.')
//         } else {
//             test.fail('Fail to localized transaction date!');
//         };
//     }, function() {
//         test.fail('Cannot back to Profile Page');
//     });

//     casper.run(function() {
//         test.done();
//     });
// });