/**
 * tcCustomerSupportRole.js - testing CustomerSupport Role.
 * casperjs test TestCase/09_AdminPage/tcCustomerSupportRole.js --xunit=Reports/Report_CustomerSupport.xml
 */
// Call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfAdminPage.js');
phantom.injectJs('./Interface/itfAdminUserTab.js');
phantom.injectJs('./Interface/itfSignUp.js');
phantom.injectJs('./Interface/itfSignIn.js');

//TC01: stucture
casper.test.begin('\n\n TC01_CustomerSupportRole_Stucture \n', function suite(test) {

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
            casper.acSignIn(sCustomerSupportUsername, sCustomerSupportPassword);
        });
    });

    casper.thenOpen(urlAdminPage, function(){
    	casper.waitForText(sTitleAdminPage, function(){
    		test.assertDoesntExist(itfAdminPage.lnkGamesMenu,'The Games Menu is hiden in CustomerSupport user.');
    		test.assertDoesntExist(itfAdminPage.lnkStudiosMenu,'The Studio Menu is hiden in CustomerSupport user.');
    		test.assertDoesntExist(itfAdminPage.lnkExchangeOptionsMenu,'The Exchange Option Menu is hiden in CustomerSupport user.');
            test.assertDoesntExist(itfAdminPage.lnkPaypalMenu,'The Paypal Menu is hiden in CustomerSupport user.');
    		test.assertExist(itfAdminPage.lnkTransactionsMenu,'The Transaction Menu is displayed in CustomerSupport user.');
    		test.assertExist(itfAdminPage.lnkTopUpCardsMenu,'The TopUpCard Menu is displayed in CustomerSupport user.');
    		test.assertExist(itfAdminPage.lnkUsersMenu,'The Users Menu is displayed in CustomerSupport user.');
    	}, function() {
            casper.test.fail('Cannot open Admin Page!');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Cannot navigate top menu by url
casper.test.begin('\n\n TC02_CustomerSupportRole_Navigate_By_URL \n', function suite(test) {

    casper.start(urlAdminGamePage).waitForText('Forbidden', function() {
        test.pass('Cannot navigate to Games Menu by url');
    });

    casper.run(function() {
        test.done();
    });
});

//TC03: 
casper.test.begin('\n\n TC03_CustomerSupportRole_Not_Reveal_Password \n', function suite(test) {

    casper.start(urlAdminUserPage).waitForText(sTitleAdminUserTab, function() {
        casper.sendKeys(txtSearchUsername, sGameAdminUsername);
    }, function(){
        test.fail('Cannot access to Admin User Page!');
    });

    casper.thenClick(btnQuery, function(){
        casper.waitForSelector(lnkUserID(idGameAdmin), function(){
            casper.click(lnkUserID(idGameAdmin));
        }, function(){
        test.fail('Fail to query!');
        });
    });

    casper.waitForSelector(lblRevealedPassword, function(){
        var sRevealedPassword = this.fetchText(lblRevealedPassword);
        test.assert(sRevealedPassword.indexOf(sGameAdminPassword) > -1, 'The password has been revealed.')
    }, function(){
        test.fail('The Revealed Password lable is not available!');
    });

    casper.run(function() {
        test.done();
    });
});

// //TC04: stucture
// casper.test.begin('\n\n TC04_CustomerSupportRole_DAU/MAU_Report \n', function suite(test) {
//     var year = TimeNow.getFullYear().toString();
//     var month = (TimeNow.getMonth() + 1).toString();
//     var date = TimeNow.getDate().toString();
//     var bfDate = date
//     if (TimeNow.getDate() > 8) {
//         bfDate = (TimeNow.getDate() - 7).toString();
//     };

//     var intGame = 1;
//     var sCurrentTime = year + '-' + month +'-' + date + ' 23:59';
//     var sBeforeTime = year + '-' + month +'-' + bfDate + ' 00:00';

//     // Sign out the previous session
//     casper.start(urlAdminPage).waitForSelector(itfAdminPage.lnkAdminReport, function() {
//         casper.click(itfAdminPage.lnkAdminReport);
//     }, function(){
//         casper.test.fail('Cannot open Admin Page!');
//     });

//     casper.waitForSelector(itfAdminPage.frmDailyReport, function(){
//         var tblValues = {};
//         tblValues[itfAdminPage.chkDaily] = true;
//         casper.sendKeys(itfAdminPage.txtFromTime, sBeforeTime);
//         casper.sendKeys(itfAdminPage.txtToTime, sCurrentTime);
//         casper.evaluate(function(selector, gameID){
//             document.querySelector(selector).value = gameID;
//         }, itfAdminPage.cbxGame, intGame);

//         this.fillSelectors(itfAdminPage.frmDailyReport, tblValues, false);
//     });

//     casper.thenClick(itfAdminPage.btnQuery, function(){
//         if (TimeNow.getDate() < 8) {
//             test.assertElementCount(itfAdminPage.lblResultRow, 1,'The Daily Report is corrent.');
//         } else {
//             test.assertElementCount(itfAdminPage.lblResultRow, 8,'The Daily Report is corrent.');
//         };
//     });

//     casper.waitForSelector(itfAdminPage.frmDailyReport, function(){
//         var tblValues = {};
//         tblValues[itfAdminPage.chkDaily] = false;

//         this.fillSelectors(itfAdminPage.frmDailyReport, tblValues, false);
//     });

//     casper.thenClick(itfAdminPage.btnQuery, function(){
//         test.assertElementCount(itfAdminPage.lblResultRow, 1,'The Monthly Report is corrent.');
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

//TC04: stucture
casper.test.begin('\n\n TC04_CustomerSupportRole_DAU/MAU_Report \n', function suite(test) {
    var year = TimeNow.getFullYear().toString();
    var month = (TimeNow.getMonth() + 1).toString();
    var date = TimeNow.getDate().toString();
    var bfDate = date
    if (TimeNow.getDate() > 8) {
        bfDate = (TimeNow.getDate() - 7).toString();
    };

    var intGame = 1;
    var sCurrentTime = year + '-' + month +'-' + date + ' 23:59';
    var sBeforeTime = year + '-' + month +'-' + bfDate + ' 00:00';

    // Sign out the previous session
    casper.start(urlAdminPage).waitForSelector(itfAdminPage.lnkAdminReport, function() {
        casper.click(itfAdminPage.lnkAdminReport);
    }, function(){
        casper.test.fail('Cannot open Admin Page!');
    });

    casper.waitForSelector(itfAdminPage.frmDailyReport, function(){
        // var tblValues = {};
        // tblValues[itfAdminPage.chkDaily] = true;
        // this.fillSelectors(itfAdminPage.frmDailyReport, tblValues, false);

        casper.sendKeys(itfAdminPage.txtFromTime, sBeforeTime);
        casper.sendKeys(itfAdminPage.txtToTime, sCurrentTime);
        // casper.evaluate(function(selector, gameID){
        //     document.querySelector(selector).value = gameID;
        // }, itfAdminPage.cbxGame, intGame);
    });

    casper.thenClick(itfAdminPage.btnQuery, function(){
        // if (TimeNow.getDate() < 8) {
        //     test.assertElementCount(itfAdminPage.lblResultRow, 1,'The Daily Report is corrent.');
        // } else {
        //     test.assertElementCount(itfAdminPage.lblResultRow, 8,'The Daily Report is corrent.');
        // };
        test.assertExist(itfAdminPage.chartActiveUser, 'The Daily Report is displayed.');
    });

    // casper.waitForSelector(itfAdminPage.frmDailyReport, function(){
    //     var tblValues = {};
    //     tblValues[itfAdminPage.chkDaily] = false;

    //     this.fillSelectors(itfAdminPage.frmDailyReport, tblValues, false);
    // });

    // casper.thenClick(itfAdminPage.btnQuery, function(){
    //     test.assertElementCount(itfAdminPage.lblResultRow, 1,'The Monthly Report is corrent.');
    // });

    casper.run(function() {
        test.done();
    });
});