/**
 * tcJSRewriteHistory.js - testing JS Rewrite History.
 * casperjs test TestCase/03_ProfilePage/tcJSRewriteHistory.js --xunit=Reports/Report_JSRewriteHistory.xml
 */

//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

var postfixTopUp = /transaction\/topup/;
var postfixProfilePage = /account\/profile/;
var postfixExchange = /transaction\/exchange/;
var postfixTransaction = /\/transaction\//;

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

    casper.then(function() {
        casper.waitUntilVisible(btnTopUp, function() {
            casper.thenClick(btnTopUp, function () {
                casper.waitForText(sTitleTopUpTab, function(){
                    test.assertUrlMatch(postfixTopUp, 'Navigate to Top Up Tab by clicking Button');
                }, function(){
                    casper.test.fail('Fail to navigate to Top Up Tab by clicking Button');
                });
            });
            casper.back();
            casper.then(function(){
                casper.waitForText(sTitleProfilePage, function(){
                    test.assertUrlMatch(postfixProfilePage, 'Back to Profile Tab');
                },function(){
                    test.fail('Back to Profile Tab');
                }, casper.intMaxWaitingTime);
            });
            casper.thenClick(btnExchange).waitForText(sTitleExchangeTab, function(){
                test.assertUrlMatch(postfixExchange, 'Navigate to Exchange Tab');
            });
            casper.thenClick('.acc-nav-topup>a').waitForText(sTitleTopUpTab, function(){
                test.assertUrlMatch(postfixTopUp, 'Navigate to Top Up Tab by clicking Total Gtoken Span');
            });
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Verify JSRewriteHistory On Transaction Page
casper.test.begin('\n\n TC02_JSRewriteHistory_On_Transaction_Page \n', function suite(test) {
    casper.start().thenClick(tabTransaction).waitForText(sTitleTransactionTab);
    casper.then(function() {
        casper.thenClick(btnTopUp).waitForText(sTitleTopUpTab, function() {
            test.assertUrlMatch(postfixTopUp, 'Navigate to Top Up Tab by clicking Button');
        }, function() {
            casper.test.fail('Fail to navigate to Top Up Tab by clicking Button')
        });
        casper.back();
        casper.then(function() {
            casper.wait(1000, function() {
                test.assertUrlMatch(postfixTransaction, 'Back to Transaction Tab');
            }, function(){
                casper.test.fail('Cannot back to Transaction Tab')
            });
        });
        casper.thenClick(btnExchange).waitForText(sTitleExchangeTab, function() {
            test.assertUrlMatch(postfixExchange, 'Navigate to Exchange Tab');
        });
    });

    casper.run(function() {
        test.done();
    });
});