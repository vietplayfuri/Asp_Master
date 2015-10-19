/**
 * tcGameAccountantRole.js - testing Game Admin Role.
 * casperjs test TestCase/09_AdminPage/tcGameAccountantRole.js --xunit=Reports/Report_GameAccountantRole.xml
 */
// Call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfAdminPage.js');
phantom.injectJs('./Interface/itfAdminTransactionTab.js');
phantom.injectJs('./Interface/itfSignUp.js');
phantom.injectJs('./Interface/itfSignIn.js');

//TC01: stucture
casper.test.begin('\n\n TC01_GameAccountantRole_Stucture \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    casper.then(function() {
        casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
            casper.acSignIn(sGameAccountantUsername, sGameAccountantPassword);
        });
    });

    casper.thenOpen(urlAdminPage, function() {
        casper.waitForText(sTitleAdminPage, function() {
            test.assertExist(itfAdminPage.lnkGamesMenu, 'The Games Menu is displayed in game accountant user.');
            test.assertExist(itfAdminPage.lnkStudiosMenu, 'The Studio Menu is displayed in game accountant user.');
            test.assertExist(itfAdminPage.lnkExchangeOptionsMenu, 'The Exchange Option Menu is displayed in game accountant user.');
            test.assertExist(itfAdminPage.lnkTransactionsMenu, 'The Transaction Menu is displayed in game accountant user.');
            test.assertDoesntExist(itfAdminPage.lnkTopUpCardsMenu, 'The TopUpCard Menu is hiden in game accountant user.');
            test.assertDoesntExist(itfAdminPage.lnkUsersMenu, 'The Users Menu is hiden in game accountant user.');
            test.assertDoesntExist(itfAdminPage.lnkPaypalMenu, 'The Paypal Menu is hiden in game accountant user.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Cannot navigate top menu by url
casper.test.begin('\n\n TC02_GameAccountantRole_Navigate_By_URL \n', function suite(test) {

    casper.start(urlAdminUserPage).waitForText('Forbidden', function() {
        test.pass('Cannot navigate to Transaction Menu by url');
    });

    casper.run(function() {
        test.done();
    });
});

//TC03: The TopUp tab in Transaction menu is hidden.
casper.test.begin('\n\n TC03_GameAccountantRole_TopUp_Tab_Hidden \n', function suite(test) {

    casper.start(urlAdminTransactionPage).waitForText(sTitleAdminTransactionTab, function() {
        test.assertDoesntExist(tabTopUp, 'The TopUp Tab is hiden in Transaction menu.');
    });

    casper.run(function() {
        test.done();
    });
});


