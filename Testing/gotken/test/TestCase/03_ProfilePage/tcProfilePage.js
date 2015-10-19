/**
 * tcProfilePage.js - testing Profile page.
 * casperjs test TestCase/03_ProfilePage/tcProfilePage.js --xunit=Reports/Report_ProfilePage.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Action/acExchange.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');
phantom.injectJs('./Interface/itfExchangeTab.js');
phantom.injectJs('./Interface/itfPaypalTab.js');

var sSelectedGame = 'Mine Mania';
var sGoldCredit = 'Gold';
var intAmountExchange = 200;

//TC01: Verify Profile Page after Login successfully
casper.test.begin('\n\n TC01_ProfilePage_Verify \n', function suite(test) {

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

    casper.then(function() {
        casper.waitForSelector(btnSignOut, function() {
            test.assertExists(btnEditProfile, 'Edit Profile Button is found.');
            test.assertExists(btnExchange, 'Exchange Button is found.');
            test.assertExists(btnTopUp, 'Top Up Button is found.');
            // test.assertExists(btnEditEmail, 'Edit Email Button is found.');
            test.assertExists(btnChangePassword, 'Change Password Button is found.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Refill Credit amount on Exchange Tab when navigating by URL
casper.test.begin('\n\n TC02_ProfilePage_Refill_Credit_Amount_Exchange_Tab_By_URL \n', function suite(test) {
    var sCurrentURL;

    // Navigate to Exchange Tab
    casper.start().thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
        casper.acOpenExchangeTab();
    }, function() {
        test.fail('Cannot navigate to Profile Page');
    });

    // Choose a game and credit type
    casper.waitForText(sTitleExchangeTab, function() {
        casper.acInputExchangeAmount(sSelectedGame, sGoldCredit, intAmountExchange);
    }, function(){
        test.fail('Cannot open Exchange Tab!');
    });

    // Get the current url
    casper.then(function() {
        sCurrentURL = this.getCurrentUrl();
    });

    // Back to Profile Tab
    casper.thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
        casper.open(sCurrentURL);
    }, function() {
        test.fail('Cannot back to Profile Page');
    });

    casper.waitForSelector(txtExchangeAmount, function() {
        casper.wait(2000, function() {
            var valueExchangeAmount = casper.evaluate(function(selector) {
                return document.querySelector(selector).value;
            }, txtExchangeAmount);
            test.assert(intAmountExchange == (+valueExchangeAmount), 'Refilled Exchange Amount textbox with text ' + valueExchangeAmount);
            test.assertTextExists(sSelectedGame, 'The Game has been selected');
            test.assertTextExists('100 Gold / GToken', 'The Game Item has been selected');
        });
    }, function(){
        test.fail('Cannot open Exchange Tab!');
    });

    casper.run(function() {
        test.done();
    });
});
    
//TC03: Refill Package on Exchange Tab when navigating by URL
casper.test.begin('\n\n TC03_ProfilePage_Refill_Package_Exchange_Tab_By_URL \n', function suite(test) {
    var sCurrentURL_TC03;
    var sSelectedGame_TC03 = 'Slamdunk Battle';
    var sSelectedPackge = '55,920 Diamonds';

     // Navigate to Exchange Tab
    casper.start().thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
        casper.acOpenExchangeTab();
    }, function() {
        test.fail('Cannot navigate to Profile Page');
    });

    // Choose a game and credit type
     casper.waitForText(sTitleExchangeTab, function() {
        casper.acInputExchangeAmount(sSelectedGame_TC03, sSelectedPackge);
    }, function(){
        test.fail('Cannot open Exchange Tab!');
    });

    // Get the current url
    casper.then(function() {
        sCurrentURL_TC03 = this.getCurrentUrl();
    });

    // Back to Profile Tab
    casper.thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
        casper.open(sCurrentURL_TC03);
    }, function() {
        test.fail('Cannot back to Profile Page');
    });

    casper.waitForSelector(cbxExchangeSelectGame, function() {
        casper.wait(2000, function() {
            test.assertTextExists(sSelectedGame_TC03, 'The Game has been selected');
            test.assertTextExists(sSelectedPackge, 'The Game Item has been selected');
        });
    }, function(){
        test.fail('Cannot open Exchange Tab!');
    });

    casper.run(function() {
        test.done();
    });
});

//TC04: Refill the Transaction Tab when navigating by URL
casper.test.begin('\n\n TC04_ProfilePage_Refill_Transaction_Tab_By_URL \n', function suite(test) {
    var sCurrentURL;
    var intGTokenAmount = 100;

    // Navigate to Paypal Tab
    casper.start().thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
        casper.thenClick(tabTransaction, function() {
            casper.waitForText(sTitleTransactionTab, function() {
                this.click(btnTopUp);
                this.click(tabPaypal);
            }, function() {
                casper.test.fail('Fail to open PayPal Tab');
            });
        });
    }, function() {
        test.fail('Cannot navigate to Profile Page');
    });

    // Choose a game and credit type
    casper.then(function() {
        this.sendKeys(txtGTokenPaypal, intGTokenAmount.toString());
    });

    // Get the current url
    casper.then(function() {
        casper.wait(1000, function() {
            sCurrentURL = this.getCurrentUrl();
        });
    });

    // Back to Profile Tab
    casper.thenClick(tabProfile).waitForText(sTitleProfilePage, function() {
        casper.open(sCurrentURL);
    }, function() {
        test.fail('Cannot back to Profile Page');
    });

    casper.waitForSelector(txtGTokenPaypal, function() {
        casper.wait(2000, function() {
            var valuePayPalAmount = casper.evaluate(function(selector) {
                return document.querySelector(selector).value;
            }, txtGTokenPaypal);
            test.assert(intGTokenAmount == (+valuePayPalAmount), 'Refilled PayPal Amount textbox with text ' + valuePayPalAmount);
        });
    }, function () {
        casper.capture('./Screenshot/abc.png')
        test.fail('Fail to refill the paypal amount textbox!');
    });

    casper.run(function() {
        test.done();
    });
});