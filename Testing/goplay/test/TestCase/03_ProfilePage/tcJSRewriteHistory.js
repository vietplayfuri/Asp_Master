/**
 * tcJSRewriteHistory.js - testing JS Rewrite History.
 * casperjs test TestCase/03_ProfilePage/tcJSRewriteHistory.js --xunit=Reports/Report_JSRewriteHistory.xml
 */

//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');
phantom.injectJs('./Interface/itfExchangeTab.js');

var postfixTopUp = /transaction\/topup/;
var postfixExchange = /transaction\/exchange/;
var postfixTransaction = /\/transaction\//;
var sURLSelectedGame = "/transaction/exchange/?game-id="
var sSelectedGame = 'Mine Mania';
var sGoldCredit = '100 Gold = 1 Play Token';

//TC01: Verify JSRewriteHistory On Profile Page
casper.test.begin('\n\n TC01_JSRewriteHistory_On_Profile_Page \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sAdminUsername, sAdminPassword);
    });

    casper.thenClick(tabTransaction ,function() {
        casper.waitUntilVisible(btnTopUp, function() {
            casper.thenClick(btnTopUp, function () {
                casper.waitForText(sTitleTopUpTab, function(){
                    test.assertUrlMatch(postfixTopUp, 'Navigate to Top Up Tab by clicking Button');
                }, function(){
                    casper.test.fail('Fail to navigate to Top Up Tab by clicking Button');
                });
            });
            casper.back(function(){
                casper.waitForText(sTitleProfilePage, function(){
                    test.assertUrlMatch(postfixTransaction, 'Back to Transaction Tab');
                },function(){
                    test.fail('Back to Transaction Tab');
                }, casper.intMaxWaitingTime);
            });
            casper.thenClick(btnExchange).waitForText(sTitleExchangeTab, function(){
                test.assertUrlMatch(postfixExchange, 'Navigate to Exchange Tab');
            });
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Verify JSRewriteHistory On Transaction Page
casper.test.begin('\n\n TC02_JSRewriteHistory_On_Transaction_Page \n', function suite(test) {

        // Choose a game and credit type
    casper.start().waitForText(sTitleExchangeTab, function() {
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

    casper.then(function() {
        casper.wait(1000, function() {
            var currentURL = this.getCurrentUrl();
            test.assert(currentURL.indexOf(sURLSelectedGame) > -1, 'the URL is correctly.');
        });
    });

    casper.run(function() {
        test.done();
    });
});