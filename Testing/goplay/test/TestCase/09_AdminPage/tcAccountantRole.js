/**
 * tcAccountantRole.js - testing Accountant Role.
 * casperjs test TestCase/09_AdminPage/tcAccountantRole.js --xunit=Reports/Report_AccountantRole.xml
 */
// Call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfAdminPage.js');
phantom.injectJs('./Interface/itfAdminUserTab.js');
phantom.injectJs('./Interface/itfAdminTransactionTab.js');
phantom.injectJs('./Interface/itfSignUp.js');
phantom.injectJs('./Interface/itfSignIn.js');

var sExchangeOrderID = 'c4cfd30c-7658-49b5-be9f-49dad181305f';

//TC01: stucture
casper.test.begin('\n\n TC01_AccountantRole_Stucture \n', function suite(test) {

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
            casper.acSignIn(sAccountantUsername, sAccountantPassword);
        });
    });

    casper.thenOpen(urlAdminPage, function(){
    	casper.waitForText(sTitleAdminPage, function(){
    		test.assertDoesntExist(itfAdminPage.lnkGamesMenu,'The Games Menu is hiden in accountant user.');
    		test.assertDoesntExist(itfAdminPage.lnkStudiosMenu,'The Studio Menu is hiden in accountant user.');
    		test.assertDoesntExist(itfAdminPage.lnkExchangeOptionsMenu,'The Exchange Option Menu is hiden in accountant user.');
            test.assertDoesntExist(itfAdminPage.lnkAdminReport,'The Daily Report Link is hiden in accountant user.');
    		test.assertExist(itfAdminPage.lnkTransactionsMenu,'The Transaction Menu is displayed in accountant user.');
    		test.assertExist(itfAdminPage.lnkTopUpCardsMenu,'The TopUpCard Menu is displayed in accountant user.');
    		test.assertExist(itfAdminPage.lnkUsersMenu,'The Users Menu is displayed in accountant user.');
    	}, function() {
        casper.test.fail('Cannot open Admin Page!');
        });
    });

    casper.thenClick(itfAdminPage.lnkTransactionsMenu, function(){
        casper.waitForSelector(tabGcoinIncome, function(){
            test.assertExist(tabGcoinIncome,'The GCoin Income Tab is displayed in accountant user.');
            test.assertExist(tabGcoinOutcome,'The GCoin Outcome Tab is displayed in accountant user.');   
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Cannot navigate top menu by url
casper.test.begin('\n\n TC02_AccountantRole_Navigate_By_URL \n', function suite(test) {

    casper.start(urlAdminGamePage).waitForText('Forbidden', function() {
        test.pass('Cannot navigate to Games Menu by url');
    });

    casper.run(function() {
        test.done();
    });
});

//TC03: 
casper.test.begin('\n\n TC03_AccountantRole_Not_Reveal_Password \n', function suite(test) {

    casper.start(urlAdminUserPage).waitForText(sTitleAdminUserTab, function() {
        casper.sendKeys(txtSearchUsername, sGameAdminUsername);
    });

    casper.thenClick(btnQuery, function(){
        casper.waitForSelector(lnkUserID(idGameAdmin), function(){
            casper.click(lnkUserID(idGameAdmin));
        });
    });

    casper.waitForText('Please contact developer for password revelation', function(){
        test.pass('The accountant role cannot reveal password.');
    });

    casper.run(function() {
        test.done();
    });
});

//TC04: 
casper.test.begin('\n\n TC04_AccountantRole_Query_Exchange_Transaction \n', function suite(test) {

    casper.start(urlAdminTransactionPage).waitForText(sTitleAdminTransactionTab, function() {
        casper.sendKeys(txtSearchUsername, sExchangeUsername);
    });

    casper.thenClick(btnQuery, function(){
        casper.waitUntilVisible(lstTransaction, function(){
            test.pass('Query the transaction of exchange option successfully.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC05: 
casper.test.begin('\n\n TC05_AccountantRole_Query_By_TimeZone \n', function suite(test) {
    var sTransactionDate;
    var sTransactionTime;
    var sAfTransactionTime;

    var sAfTimeZone = 'Asia/Tashkent';
    var sBfTimeZone = 'Asia/Singapore';
    var sGMT = 3;

    casper.start(function() {
        var tblValues = {};
        tblValues[cbxTimeZone] = sBfTimeZone;
        tblValues[txtSearchOrderID('exchange')] = sExchangeOrderID;
        tblValues[txtSearchUsername] = '';

        this.fillSelectors(frmSearchExchange, tblValues, false);
    });

    casper.thenClick(btnQuery, function(){
        casper.waitUntilVisible(lstTransaction, function(){
            test.pass('Query the transaction of exchange option successfully.');
            sTransactionDate = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[7].textContent;
            }, lst1stTransactionCell);
            sTransactionTime = sTransactionDate.split(" ")[1].split(":")[0];
        });
    });

    casper.then(function(){
        var tblValues = {};
        tblValues[cbxTimeZone] = sAfTimeZone;

        this.fillSelectors(frmSearchExchange, tblValues, false);
    });

    casper.thenClick(btnQuery, function(){
        casper.wait(3000, function(){
            sTransactionDate = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[7].textContent;
            }, lst1stTransactionCell);
            sAfTransactionTime = sTransactionDate.split(" ")[1].split(":")[0];
        });
    });

    casper.then(function(){
        test.assert(sAfTransactionTime == sTransactionTime - sGMT, 'the Date has been changed by time zone.');
    });

    casper.run(function() {
        test.done();
    });
});

//TC06: 
casper.test.begin('\n\n TC06_AccountantRole_Query_Topup_Transaction \n', function suite(test) {

    casper.start().thenClick(tabTopUp);

    casper.waitUntilVisible(frmSearchTopUp, function() {
        var tblValues = {};
        tblValues[txtSearchUsernameTopup] = sUsername;
        this.fillSelectors(frmSearchTopUp, tblValues, false);
    });

    casper.thenClick(btnQueryTransaction('topup'), function() {
        casper.waitUntilVisible(lstTransaction, function() {
            test.pass('Query the transaction of topup successfully.');
        }, function() {
            test.fail('Fail to query TopUp Transaction!');
        }, casper.intMaxWaitingTime * 3);
    });

    casper.run(function() {
        test.done();
    });
});

//TC07: 
casper.test.begin('\n\n TC07_AccountantRole_Query_GCoin_Income_Transaction \n', function suite(test) {
    var sGCoinIncomeID = fs.read(path_gcoin_income);
    var game = 'Endgods'
    var amount = '5.000'
    var status = 'success'

    casper.start(urlHomepage);
    casper.thenOpen(urlAdminTransactionIncomePage);

    casper.waitUntilVisible(frmSearchGCoinIncome, function() {
        var tblValues = {};
        tblValues[txtSearchOrderID('gcoin-income')] = sGCoinIncomeID;
        this.fillSelectors(frmSearchGCoinIncome, tblValues, false);
    }, function(){
        test.fail('Fail to open GCoin Income Tab!');
    }, casper.intMaxWaitingTime * 2);

    casper.thenClick(btnQueryTransaction('gcoin-income'), function() {
        casper.waitUntilVisible(lstTransaction, function() {
            sTransactionID = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[0].textContent;
            }, lst1stTransactionCell);
            sTransactionAccount = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[1].textContent;
            }, lst1stTransactionCell);
            sTransactionAmount = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[2].textContent;
            }, lst1stTransactionCell);
            sTransactionGame = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[4].textContent;
            }, lst1stTransactionCell);
            sTransactionStatus = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[7].textContent;
            }, lst1stTransactionCell);
        }, function() {
            test.fail('Fail to query GCoin Income Transaction!');
        }, casper.intMaxWaitingTime * 3);
    });

    casper.then(function(){
        test.assert(sTransactionID.indexOf(sGCoinIncomeID) != -1, 'The transaction ID is right.')
        test.assert(sTransactionAccount.indexOf(sExchangeUsername) != -1, 'The transaction Account is '+sTransactionAccount +'.')
        test.assert(sTransactionGame.indexOf(game) != -1, 'The transaction game is '+game+'.')
        test.assert(sTransactionAmount.indexOf(amount) != -1, 'The transaction amount is '+amount+'.')
        test.assert(sTransactionStatus.indexOf(status) != -1, 'The transaction status is '+status+'.')
    });
    casper.run(function() {
        test.done();
    });
});

//TC08: 
casper.test.begin('\n\n TC08_AccountantRole_Query_GCoin_Outcome_Transaction \n', function suite(test) {
    var sGCoinOutcomeID = fs.read(path_gcoin_outcome);
    var game = 'Endgods'
    var amount = '1.000'
    var status = 'success'

    casper.start(urlHomepage);
    casper.thenOpen(urlAdminTransactionOutcomePage);

    casper.waitUntilVisible(frmSearchGCoinOutcome, function() {
        var tblValues = {};
        tblValues[txtSearchOrderID('gcoin-outcome')] = sGCoinOutcomeID;
        this.fillSelectors(frmSearchGCoinOutcome, tblValues, false);
    }, function(){
        test.fail('Fail to open GCoin Outcome Tab!');
    }, casper.intMaxWaitingTime * 2);

    casper.thenClick(btnQueryTransaction('gcoin-outcome'), function() {
        casper.waitUntilVisible(lstTransaction, function() {
            sTransactionID = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[0].textContent;
            }, lst1stTransactionCell);
            sTransactionAccount = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[1].textContent;
            }, lst1stTransactionCell);
            sTransactionAmount = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[2].textContent;
            }, lst1stTransactionCell);
            sTransactionStatus = casper.evaluate(function(selector) {
                return document.querySelectorAll(selector)[6].textContent;
            }, lst1stTransactionCell);
        }, function() {
            test.fail('Fail to query GCoin Outcome Transaction!');
        }, casper.intMaxWaitingTime * 3);
    });

    casper.then(function(){
        test.assert(sTransactionID.indexOf(sGCoinOutcomeID) != -1, 'The transaction ID is right.')
        test.assert(sTransactionAccount.indexOf(sExchangeUsername) != -1, 'The transaction Account is '+sTransactionAccount+'.')
        test.assert(sTransactionAmount.indexOf(amount) != -1, 'The transaction amount is '+amount+'.')
        test.assert(sTransactionStatus.indexOf(status) != -1, 'The transaction status is '+status+'.')
    });
    casper.run(function() {
        test.done();
    });
});

//TC09: 
casper.test.begin('\n\n TC09_AccountantRole_Search_Paypal_Payment \n', function suite(test) {
    var sTransactionID = '9WX058551U622251W'

    casper.start(urlAdminPage).waitForSelector(itfAdminPage.lnkPaypalMenu, function () {
        casper.click(itfAdminPage.lnkPaypalMenu);
    });

    casper.waitForSelector(itfAdminPaypalTab.btnSearchPayment, function () {
        casper.thenClick(itfAdminPaypalTab.btnSearchPayment, function () {
            casper.waitForSelector(itfAdminPaypalTab.txtTransactionID, function () {
                casper.sendKeys(itfAdminPaypalTab.txtTransactionID, sTransactionID);
            });
        });
    });

    casper.thenClick(itfAdminPaypalTab.btnSubmit, function () {
        casper.waitForSelector(itfAdminPaypalTab.tblPaymentDetail, function () {
            test.assertExist(itfAdminPaypalTab.tblPaymentDetail, 'Search paypal payment successfully.');
        }, function () {
            test.fail('Fail to search payment!');
        });
    });

    casper.run(function() {
        test.done();
    });
});
