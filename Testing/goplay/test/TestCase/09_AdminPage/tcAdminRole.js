/**
 * tcAdminRole.js - testing Admin Role.
 * casperjs test TestCase/09_AdminPage/tcAdminRole.js --xunit=Reports/Report_AccountantRole.xml
 */
// Call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfAdminPage.js');
phantom.injectJs('./Interface/itfAdminTransactionTab.js');
phantom.injectJs('./Interface/itfSignUp.js');
phantom.injectJs('./Interface/itfSignIn.js');

//TC01: stucture
casper.test.begin('\n\n TC01_AdminRole_Stucture \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page!');
    });
    
    casper.then(function() {
        casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
            casper.acSignIn(sAdminUsername, sAdminPassword);
        });
    });

    casper.thenOpen(urlAdminPage, function(){
    	casper.waitForText(sTitleAdminPage, function(){
    		test.assertExist(itfAdminPage.lnkGamesMenu,'The Games Menu is displayed in admin user.');
    		test.assertExist(itfAdminPage.lnkStudiosMenu,'The Studio Menu is displayed in admin user.');
    		test.assertExist(itfAdminPage.lnkExchangeOptionsMenu,'The Exchange Option Menu is displayed in admin user.');
    		test.assertExist(itfAdminPage.lnkTransactionsMenu,'The Transaction Menu is displayed in admin user.');
    		test.assertExist(itfAdminPage.lnkTopUpCardsMenu,'The TopUpCard Menu is displayed in admin user.');
    		test.assertExist(itfAdminPage.lnkUsersMenu,'The Users Menu is displayed in admin user.');
    	}, function() {
        casper.test.fail('Cannot open Admin Page!');
        });
    });

    casper.thenClick(itfAdminPage.lnkTransactionsMenu, function(){
        casper.waitForSelector(itfAdminTransactionTab.tabGcoinIncome, function(){
            test.assertExist(itfAdminTransactionTab.tabGcoinIncome,'The GCoin Income Tab is displayed in admin user.');
            test.assertExist(itfAdminTransactionTab.tabGcoinOutcome,'The GCoin Outcome Tab is displayed in admin user.');   
        });
    });

    casper.run(function() {
        test.done();
    });
});

