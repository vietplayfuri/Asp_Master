/**
 * tcTransfer.js - testing Transfering Gtoken.
 * casperjs test TestCase/05_Transaction/tcTransfer.js --xunit=Reports/Report_Transfer.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfProfilePage.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfSignUp.js');

// Actions
var acVerifyTransactionComplete = function(AmountGtoken, Gtoken, status) {
    // Check Point for Transaction
    casper.then(function() {
        // Navigate to Paypal Tab
        casper.click(tabTransaction);
        casper.waitForText(sTitleTransactionTab, function() {
            var intAmoutGtoken = this.fetchText(lblAmoutGtoken);
            var intAmoutFreeGtoken = this.fetchText(lblAmoutFreeGtoken);
            var intAmoutTotalGtoken = this.fetchText(lblAmoutTotalGtoken).replace(/ /g, '');
            
            var intExpectedMinusGtoken = ((+AmountGtoken[0])*1000 - Gtoken*1000)/1000;
            var intExpectedPlusGtoken = ((+AmountGtoken[0])*1000 + Gtoken*1000)/1000;
            var intRealTotal = ((+intAmoutGtoken)*1000 + (+intAmoutFreeGtoken)*1000)/1000;

            this.wait(casper.intWaitingTime, function() {
                if (status == 'minus') {
                    this.test.assert(intAmoutGtoken == intExpectedMinusGtoken.toString(), 'Transfer by ' + Gtoken + ' (' + AmountGtoken[0] + '=>' + intAmoutGtoken + ')');
                } else if (status == 'plus') {
                    this.test.assert(intAmoutGtoken == intExpectedPlusGtoken.toString(), 'Transfer by ' + Gtoken + ' (' + AmountGtoken[0] + '=>' + intAmoutGtoken + ')');
                };
                this.test.assert(intAmoutFreeGtoken == AmountGtoken[1].toString(), 'The amount of Free Gtoken ('+ intAmoutFreeGtoken +') isnt changed');
                this.test.assert(intAmoutTotalGtoken == intRealTotal.toString(), 'The Total Gtoken equals to Gtoken plus Free Gtoken ('+ intAmoutTotalGtoken + ')');
            });
        });
    });
};
var acOpenTransactionTab = function() {
    var arrAmount = new Array();
    // Navigate to Transfer Tab
    casper.waitForSelector('a[data-active-page="account_profile"]', function() {
        casper.click(tabTransaction);
        casper.waitForText(sTitleTransactionTab, function() {
            // Get Amount of each kind of Gtoken
            arrAmount.push(this.fetchText(lblAmoutGtoken));
            arrAmount.push(this.fetchText(lblAmoutFreeGtoken));
            arrAmount.push(this.fetchText(lblAmoutTotalGtoken));

            this.thenClick(tabFriends);
        }, function() {
            casper.test.fail('Fail to open Transfer tab');
        });
    }, function() {
        casper.test.fail('Cannot back to profile page')
    });
    return arrAmount;
};
var acInputTransfer = function(Username, GtokenAmount) {
    casper.waitUntilVisible(txtReceiverSearch, function() {
        casper.sendKeys(txtReceiverSearch, Username, {
            keepFocus: true
        });
        casper.waitForSelector(itmSearchReceiver, function() {
            this.click(itmSearchReceiver);
        }, function() {
            casper.test.fail('The Auto Complete Function of Receiver doesnt worked');
        });
        casper.then(function() {
            this.sendKeys(txtGtokenTransfer, GtokenAmount.toString(), {reset : true});
            this.click(btnTransferGtoken);
        });
    });
};

var arrAmoutGtoken1 = new Array();
var arrAmoutGtoken2 = new Array();
var intAmountTransfer = 1.1;

//TC01: Validate the amount transfered
casper.test.begin('\n\n TC01_Transfer_Validate_Amount \n', function suite(test) {
    var lstInvalidAmounts = ['-1', '-01', '0', 'ten', '-0.01', '1.001', 'ten.01', ' '];
    var lstAmounts = 'List of Invalid Amount transfered: ';
    var bolAmounts = '';

   // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    // Login with acc FoxyMax/123
    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sUsername, sPassword);
    });

    // Navigate to Transfer Tab
    casper.then(function() {
        acOpenTransactionTab();
    });

    // Validate the amount input
    casper.waitUntilVisible(txtReceiverSearch, function() {
        casper.sendKeys(txtReceiverSearch, sReceiverUsername, {
            keepFocus: true
        });
        casper.wait(casper.intWaitingTime * 2, function() {
            this.click(itmSearchReceiver);
        }, function() {
            casper.test.fail('The Auto Complete Function of Receiver doesnt worked');
        });
        casper.then(function() {
            casper.each(lstInvalidAmounts, function(self, amount) {
                this.sendKeys(txtGtokenTransfer, amount, {
                    reset: true
                });
                this.wait(casper.intWaitingTime);
                this.then(function(){
                    var amountText = this.fetchText('.gtoken_number.ng-binding');
                    var amountInput = amountText.split(" ");
                    if ((+amountInput[0]) > 0) {
                        lstAmounts += amount + ', ';
                        bolAmounts = true;
                    };
                });

            });
        });
    });

    casper.then(function() {
        if (bolAmounts) {
            casper.test.fail(lstAmounts);
        } else {
            casper.test.pass('The Gtoken Amount are validated');
        };
    });
    
    casper.run(function() {
        test.done();
    });
});

//TC02: Verify Sending Gtoken successfully
casper.test.begin('\n\n TC02_Transfer_Sending_Gtoken \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    // Pre-condition (including new accounts and get current Gtoken of the account)
    // get current Gtoken of the 2nd account
    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        this.acSignIn(sReceiverUsername, sReceiverPassword);
        arrAmoutGtoken2 = acOpenTransactionTab();
        this.acSignOut();
    });

    // Login with acc FoxyMax/123
    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sUsername, sPassword);
    });

    // Navigate to Transfer Tab
    casper.then(function() {
        arrAmoutGtoken1 = acOpenTransactionTab();
    });

    // Search receiver (using auto-complete username) and input amount of Gtoken to transferd
    casper.then(function() {
        acInputTransfer(sReceiverUsername, intAmountTransfer);
    });

    // Verify the transaction successfully or not
    casper.then(function() {
        casper.waitForText('Transaction is successful.', function() {
            acVerifyTransactionComplete(arrAmoutGtoken1, intAmountTransfer, 'minus');
        }, function() {
            casper.capture('./Screenshot/transfered.png')
            casper.test.fail('Fail to transfer Gtoken. Verify amount of Gtoken');
        });
    });

    // Verify Transaction History
    casper.then(function(){
        casper.acCheckTransactionHistory('transfer', 'You transferred 1.1 GToken to ' + sReceiverUsername, '1.1');
    });

    casper.run(function() {
        test.done();
    });
});

//TC03: Verify Receiving Gtoken successfully
casper.test.begin('\n\n TC03_Transfer_Receving_Gtoken \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    // Login with acc GtokenReceiver/123
    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sReceiverUsername, sReceiverPassword);
    });

    casper.then(function() {
        acOpenTransactionTab();
    });

    // Verify the transaction successfully or not
    casper.then(function() {
        casper.waitUntilVisible(txtReceiverSearch, function() {
            acVerifyTransactionComplete(arrAmoutGtoken2, intAmountTransfer, 'plus');
        }, function() {
            casper.test.fail('Fail to transfer Gtoken. Verify amount of Gtoken');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC04: Search by Email to transfer
casper.test.begin('\n\n TC04_Transfer_Search_Email \n', function suite(test) {

    // Navigate to Profile tab to get Gtoken Amount
    casper.start().thenClick(tabProfile, function() {
        acOpenTransactionTab();
    });

    // Search receiver (using auto-complete username) and input amount of Gtoken to transferd
    casper.then(function() {
        acInputTransfer(sEmail, '');
    });

    // Verify the transaction successfully or not
    casper.then(function() {
        casper.test.assert(this.fetchText(lblReceiverName) == sUsername, 'The sender can search receiver by email.');
    });

    casper.run(function() {
        test.done();
    });
});

//TC05: Send the amount be exceeded the pocket
casper.test.begin('\n\n TC05_Transfer_Exceed_Pocket \n', function suite(test) {
    var arrAmountGtoken_TC05 = new Array();
    var sExceedAmount;

     // Navigate to Profile tab to get Gtoken Amount
    casper.start().thenClick(tabProfile, function() {
        arrAmountGtoken_TC05 = acOpenTransactionTab();
    });

    // Search receiver (using auto-complete username) and input amount of Gtoken to transferd
    casper.then(function() {
        sExceedAmount = (+arrAmountGtoken_TC05[0]) + 1;
        acInputTransfer(sUsername, sExceedAmount);
    });

    // Verify the transaction successfully or not
    casper.then(function() {
        casper.waitForSelector(errInsufficientAmount, function(){
            // var attributeTransferBtn = this.getElementAttribute(btnTransferGtoken, 'class');
            // this.test.assert(attributeTransferBtn == 'disabled', 'Cannot transfer exceed amount ' + sExceedAmount);
            this.test.pass('Cannot transfer exceed amount ' + sExceedAmount);
        });  
    });

    casper.run(function() {
        test.done();
    });
});

