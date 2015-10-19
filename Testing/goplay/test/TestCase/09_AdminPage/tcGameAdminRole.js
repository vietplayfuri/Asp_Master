/**
 * tcGameAdminRole.js - testing Game Admin Role.
 * casperjs test TestCase/09_AdminPage/tcGameAdminRole.js --xunit=Reports/Report_GameAdminRole.xml
 */
// Call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Action/acExchange.js');
phantom.injectJs('./Interface/itfExchangeTab.js');
phantom.injectJs('./Interface/itfProfilePage.js');
phantom.injectJs('./Interface/itfAdminPage.js');
phantom.injectJs('./Interface/itfAdminStudioTab.js');
phantom.injectJs('./Interface/itfAdminGameTab.js');
phantom.injectJs('./Interface/itfSignUp.js');
phantom.injectJs('./Interface/itfSignIn.js');

var idStudio = 1;
var nameGTokenStudio = 'Sparkjumpers Pte Ltd';
var sTitleViewStudioPage = 'Studio: ' + nameGTokenStudio;
var sTitleEditStudioPage = 'Edit Studio: ' + nameGTokenStudio;
var sExchangeOptionID = 0;
var sSelectedGame = 'Endgods';
var sNameCreditType = 'FxMx';

//TC01: stucture
casper.test.begin('\n\n TC01_GameAdminRole_Stucture \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sGameAdminUsername, sGameAdminPassword);
    });

    casper.thenOpen(urlAdminPage, function() {
        casper.waitForText(sTitleAdminPage, function() {
            test.assertExist(itfAdminPage.lnkGamesMenu, 'The Games Menu is displayed in game admin user.');
            test.assertExist(itfAdminPage.lnkStudiosMenu, 'The Studio Menu is displayed in game admin user.');
            test.assertExist(itfAdminPage.lnkExchangeOptionsMenu, 'The Exchange Option Menu is displayed in game admin user.');
            test.assertDoesntExist(itfAdminPage.lnkTransactionsMenu, 'The Transaction Menu is hiden in game admin user.');
            test.assertDoesntExist(itfAdminPage.lnkTopUpCardsMenu, 'The TopUpCard Menu is hiden in game admin user.');
            test.assertDoesntExist(itfAdminPage.lnkUsersMenu, 'The Users Menu is hiden in game admin user.');
        }, function() {
        casper.test.fail('Cannot open Admin Page!');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Cannot navigate top menu by url
casper.test.begin('\n\n TC02_GameAdminRole_Navigate_By_URL \n', function suite(test) {

    casper.start(urlAdminTransactionPage).waitForText('Forbidden', function() {
        test.pass('Cannot navigate to Transaction Menu by url');
    });

    casper.run(function() {
        test.done();
    });
});

//TC03: 
casper.test.begin('\n\n TC03_GameAdminRole_Assign_GameAdmin \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function() {
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sAdminUsername, sAdminPassword);
    });

    casper.thenOpen(urlAdminStudioPage).waitForText(sTitleAdminStudioTab, function() {
        casper.click(btnViewStudio(idStudio));
    });

    casper.then(function() {
        casper.waitForText(sTitleViewStudioPage, function() {
            if (casper.exists(btnUnassignGameAdmin(idGameAdmin)) == true) {
                test.skip(1, 'The GameAdmin "' + sGameAdminUsername + '" has been assigned.');
            } else {
                casper.thenClick(btnAssignAdmin(idStudio), function() {
                    casper.thenClick(btnAssignGameAdmin(idGameAdmin), function() {
                        casper.waitForText(msgAssignGameAdmin, function() {
                            test.pass('Assgin game addmin successfully.');
                        });
                    });
                });
            };
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC04: 
casper.test.begin('\n\n TC04_GameAdminRole_See_Only_Perrmitted_Studio \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function() {
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sGameAdminUsername, sGameAdminPassword);
    });

    casper.thenOpen(urlAdminStudioPage).waitForSelector(lstStudio, function() {
        test.assertSelectorHasText(lblStudioName(idStudio), nameGTokenStudio, 'The ' + sGameAdminUsername + ' belongs to studio ' + nameGTokenStudio);
        // test.assertElementCount(lstStudio, 1, 'See only studio which user belongs to.');
    });

    casper.run(function() {
        test.done();
    });
});

//TC05: 
casper.test.begin('\n\n TC05_GameAdminRole_Edit_Studio \n', function suite(test) {

    casper.start().thenClick(btnEditStudio(idStudio), function() {
        casper.waitForText(sTitleEditStudioPage, function() {
            test.pass('GameAdmin can edit the permitted studio');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC06: 
casper.test.begin('\n\n TC06_GameAdminRole_See_All_Game_From_Studio \n', function suite(test) {
    var bolGameFromOtherStudio = false;
    var lstNoPermiitedGames = '';

    casper.start().thenClick(itfAdminPage.lnkGamesMenu, function() {
        casper.waitForText(sTitleAdminGameTab, function() {
            var arrGameStudioName = casper.evaluate(function(selector){
                var arrSelectorText = new Array();
                var lstSelector = document.querySelectorAll(selector);
                for (var i = lstSelector.length - 1; i >= 0; i--) {
                    arrSelectorText.push(document.querySelectorAll(selector)[i].textContent);
                };
                return arrSelectorText; 
            }, lstGameStudioName);

            for (var i = arrGameStudioName.length - 1; i >= 0; i--) {
                if (arrGameStudioName[i].indexOf(nameGTokenStudio) == -1) {
                    bolGameFromOtherStudio = true;
                    lstNoPermiitedGames += arrGameStudioName[i] + ' ,';
                };
            };
        });
    });

    casper.then(function() {
        if (bolGameFromOtherStudio) {
            test.fail('List of Games doesn\'t belong to Game Admin: ' + lstNoPermiitedGames);
        } else {
            test.pass('See only the games belong to Game Admin');
        };
    });

    casper.thenClick(btn1stEditGame, function() {
        casper.waitForSelector(txtGameName, function(){
            test.assertDoesntExist(chkGameActive,'Game Admin cannot set Game Status.');
        });   
    });

    casper.run(function() {
        test.done();
    });
});

//TC07: 
casper.test.begin('\n\n TC07_GameAdminRole_Add_Exchange_Option \n', function suite(test) {
    var bolGameFromOtherStudio = false;
    var lstNoPermiitedGames = '';

    casper.start(urlAdminExchangePage).waitForSelector(itfAdminExchangeTab.btnAddCredit, function() {
        var tblValues = {};
        tblValues[itfAdminExchangeTab.txtStringIdentifier] = 'StringCreditTest_' + sUniqueValue;
        tblValues[itfAdminExchangeTab.txtName] = sNameCreditType;
        tblValues[itfAdminExchangeTab.txtExchangeRate] = 5;
        tblValues[itfAdminExchangeTab.txtFreeExchangeRate] = 5;
        tblValues[itfAdminExchangeTab.chkActive] = true;

        casper.thenClick(itfAdminExchangeTab.btnAddCredit, function () {
            this.fillSelectors(itfAdminExchangeTab.frmAdminExchange, tblValues, false);
            this.page.uploadFile(itfAdminExchangeTab.btnUploadIcon, pathIconImage); 
            this.click(itfAdminExchangeTab.btnSubmit);
        });
    });

    casper.waitForText(msgCreateCredit, function() {
        sExchangeOptionID = casper.evaluate(function(selector){
            return document.querySelector(selector).textContent;
        }, itfAdminExchangeTab.lblExchangeOptionID);
        test.pass('The "'+sNameCreditType+'" credit type has been created successfully.')
    }, function () {
        test.fail('Fail to create the credit type "'+sNameCreditType+'" !')
    });

    // Sign out the previous session
    casper.thenOpen(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function() {
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sExchangeUsername, sExchangePassword);
    });

    // Navigate to Exchange Tab
    casper.thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
        casper.acOpenExchangeTab();
    }, function() {
        test.fail('Cannot back to Profile Page');
    });

    // Choose a game and credit type
    casper.waitForText(sTitleExchangeTab, function() {
        casper.acSelectGameItem(sSelectedGame, sNameCreditType, '2');
    });

    casper.then(function() {
       casper.waitForText('5 '+sNameCreditType+' / GToken', function() {
            test.pass('The Exchange Option is available for user to select.')
       });
    });

    casper.run(function() {
        test.done();
    });
});

//TC08: 
casper.test.begin('\n\n TC08_GameAdminRole_Archive_Game \n', function suite(test) {
    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function() {
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sGameAdminUsername, sGameAdminPassword);
    });

    casper.thenOpen(urlAdminExchangePage).waitForSelector(itfAdminExchangeTab.btnEditCredit(sExchangeOptionID), function() {
        casper.thenClick(itfAdminExchangeTab.btnEditCredit(sExchangeOptionID), function () {
            var tblValues = {};
            tblValues[itfAdminExchangeTab.chkArchive] = true;

            this.fillSelectors(itfAdminExchangeTab.frmAdminExchange, tblValues, false);
            this.click(itfAdminExchangeTab.btnSubmit);
        });
    }, function () {
        test.fail('Fail to open Admin Exchange Page');
    });

    casper.then(function() {
        casper.test.assertDoesntExist(itfAdminExchangeTab.btnEditCredit(sExchangeOptionID),'The Exchange Option has been archived.');
    });

    casper.run(function() {
        test.done();
    });
});

//TC09: 
casper.test.begin('\n\n TC09_GameAdminRole_Unassign_GameAdmin \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function() {
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sAdminUsername, sAdminPassword);
    });

    casper.thenOpen(urlAdminStudioPage).waitForText(sTitleAdminStudioTab, function() {
        casper.click(btnViewStudio(idStudio));
    });

    casper.thenClick(btnUnassignGameAdmin(idGameAdmin), function() {
        casper.waitForText(msgUnassignGameAdmin, function() {
            test.pass('Unassign the game admin successfully.');
        });
    });

    casper.run(function() {
        test.done();
    });
});
