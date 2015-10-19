casper.test.comment("Loading Common Actions...");

var section_top_user = '.top-user-section'

// Sign in on Gtoken Page
casper.acSignIn = function(username, password) {
    // Fill all TextBox
    var tblValues = {};
    tblValues[txtUsername] = username;
    tblValues[txtPassword] = password;

    casper.waitForSelector(frmSignIn, function() {
        this.fillSelectors(frmSignIn, tblValues, false);
        this.click(btnSignIn);
    }, function() {
        casper.test.fail('Cannot not load Log In Page!');
    }, casper.intMaxWaitingTime);

    casper.waitForSelector(section_top_user, function() {
        casper.test.pass('Sign In with "' + username + '" successfully.');
        casper.then(function(){
            if (!(this.fetchText(lblCurrentLanguage) == 'English')) {
                casper.click(lnkEnglishLanguage);
            };
        });
    }, function() {
        casper.test.fail('Cannot not login to GToken with account "' + username + '"');
    });
};

casper.acSignOut = function() {
    casper.wait(1000, function() {
        casper.click(btnSignOut);
    });
    casper.waitForUrl(urlHomepage, function() {
        casper.test.pass('Log Out process is completed.');
    }, function() {
        casper.test.fail('Log Out unsuccessfully.');
    });
};

// Sign Up on Gtoken Page
casper.acSignUp = function(username, password, email, repeated_email, referralID, status) {
    // Fill all TextBox
    var sDefaultLocation = 'Viet Nam';
    var tblValues = {};
    tblValues[txtUsername] = username;
    tblValues[txtEmail] = email;
    tblValues[txtRepeatedEmail] = repeated_email;
    tblValues[txtPassword] = password;
    tblValues[txtReferralID] = referralID;
    tblValues[rdnAcceptTOS] = status;

    casper.waitForSelector(frmSignUp, function(){
        casper.sendKeys(txtLocation, sDefaultLocation, {reset : true, keepFocus: true});
        casper.waitForSelector(itemLocation, function() {
            casper.thenClick(itemLocation);
        }, function () {
            casper.test.fail('The Auto Complete Function of User Location doesnt worked'); 
        });
    }, function(){
        casper.test.fail('Cannot not load Register page!');
    });

    // casper.then(function () {
    //     casper.wait(2000, function () {
    //         if (casper.fetchText(txtLocation) != 'Viet Nam') {
    //             casper.capture('./Screenshot/abc.png');
    //             casper.echo(casper.fetchText(txtLocation));
    //         };
    //     });
    // });


    casper.waitForSelector(frmSignUp, function() {
        this.fillSelectors(frmSignUp, tblValues, false);
        this.wait(casper.intWaitingTime * 2, function() {
            this.click(btnSignUp);
        });
    }, function() {
        casper.test.fail('Cannot not load Register Page!');
    }, casper.intMaxWaitingTime);

    casper.waitForSelector(section_top_user, function() {
        casper.test.pass('Sign Up with "' + username + '" successfully.');
        casper.test.assertSelectorHasText(lblUserLocation, sDefaultLocation, 'The User location is "'+ sDefaultLocation +'".' + this.fetchText(lblUserLocation));
        // casper.then(function() {
        //     if (referralID == '') {
        //        var intTotalGToken = parseInt(casper.fetchText('#user-total-gtoken'));
        //         casper.wait(1000, function() {
        //             casper.test.assertEquals(intTotalGToken, 1, 'Receive ' + intTotalGToken + ' Free GToken.');
        //         }); 
        //     };  
        // });

        casper.then(function(){
            if (!(this.fetchText(lblCurrentLanguage) == 'English')) {
                casper.click(lnkEnglishLanguage);
            };
        });
    }, function() {
        if (this.exists(errUsernameTaken)) {
            casper.test.skip(1, 'The account "' + username + '" has been used already.');
        } else {
            casper.test.fail('Create a new account "' + username + '" unsuccessfully');
        };
    }, casper.intMaxWaitingTime*2);
};

// Sign in on Paypal Page
casper.acSignInPaypal = function(username, password) {
    // Login to paypal by using Khang account
    casper.then(function() {
        casper.waitForText('Pay with my PayPal account', function() {
            if (casper.exists(lnkPayWithAccount)) {
                casper.click(lnkPayWithAccount);
            };
            casper.waitUntilVisible(txtUsernamePaypal, function() {
                this.sendKeys(txtUsernamePaypal, username);
                this.sendKeys(txtPasswordPaypal, password);
                this.click(btnLoginPaypal);
            }, function() {
                casper.test.fail('Fail to open Sign In tab on Paypal!');
            }, casper.intMaxWaitingTime * 2);
        }, function() {
            casper.test.fail('Fail to navigate to Paypal sandbox!');
        }, casper.intMaxWaitingTime * 2);
    });

    casper.then(function(){
        casper.waitForText('Review your information', function(){
            casper.test.pass('Sign in on Paypal by acc "' + username + ' successfully."');
        }, function(){
            casper.test.fail('Cannot connect to Paypal website by acc "' + username + '"');
        }, casper.intMaxWaitingTime * 2);
    });
};

// Verify Transaction history
casper.acCheckTransactionHistory = function(typeOfTransaction, description, amount, bolInvoice) {
    casper.waitForSelector(pnlTransactionHistory, function() {
        if (typeof invoice === 'undefined') {
            invoice = 'default';
        }

        transactionImage = casper.evaluate(function(selector) {
            return document.querySelectorAll(selector)[0].getAttribute('src');
        }, imgTransaction);
        transactionDesc = casper.evaluate(function(selector) {
            return document.querySelectorAll(selector)[0].textContent;
        }, descTransaction);
        transactionAmount = casper.evaluate(function(selector) {
            return document.querySelectorAll(selector)[0].textContent;
        }, amountTransaction);
        transactionInvoice = casper.evaluate(function(selector) {
            return document.querySelectorAll(selector)[0].getAttribute('href');
        }, invoiceTransaction);

        // casper.test.assertMatch(transactionImage, typeOfTransaction, 'The transaction displays ' + typeOfTransaction + ' icon.');
        // casper.test.assertMatch(transactionDesc, description, 'The transaction displays correct description.');
        // casper.test.assertMatch(transactionAmount, amount, 'The amount of transaction is ' + amount);

        casper.test.assert(transactionImage.indexOf(typeOfTransaction) > -1, 'The transaction displays ' + typeOfTransaction + ' icon.');
        casper.test.assert(transactionDesc.indexOf(description) > -1, 'The transaction displays correct description.');
        casper.test.assert(transactionAmount.indexOf(amount) > -1, 'The amount of transaction is ' + amount);

        if (bolInvoice == true) {
            casper.thenOpen(urlHomepage + transactionInvoice).waitForText(sTitleInvoice, function() {
                var idInvoice = transactionInvoice.split('id=')[1];
                var amountInvoice = amount.toString().replace(/\//g, '') + ' GToken';
                casper.test.assertTextExists(idInvoice, 'The transaction links to correct invoice.');
                casper.test.assertTextExists(amountInvoice, 'The invoice shows correct amount');
            }, function() {
                casper.test.fail('Cannot open the invoice link.');
            });
        };
    });
};

casper.acVerifyTransactionComplete = function(arrAmountGtoken, Gtoken, status, typeOfTransaction) {
    // Check Point for Transaction
    casper.then(function() {
        if (typeof typeOfTransaction === "undefined") {
            typeOfTransaction = 'minus';
        };

        // Navigate to Paypal Tab
        casper.waitForSelector(lblAmoutGtoken, function() {
            var intAmoutGtoken = this.fetchText(lblAmoutGtoken);
            var intAmoutFreeGtoken = this.fetchText(lblAmoutFreeGtoken);
            var intExpectedGtoken;
            var intAmountGToken_bf;

            if (status == 'Normal' || status == 'Both') {
                intAmountGToken_bf = arrAmountGtoken[0];
            } else if (status == 'Free') {
                intAmountGToken_bf = arrAmountGtoken[1];
            };

            if (typeOfTransaction == 'minus') {
                intExpectedGtoken = (intAmountGToken_bf*1000 - Gtoken*1000)/1000;
            } else if(typeOfTransaction == 'plus'){
                intExpectedGtoken = (intAmountGToken_bf*1000 + Gtoken*1000)/1000;
            } ;

            if (status == 'Normal') {
                this.test.assert(intAmoutGtoken == intExpectedGtoken, 'Exchange/Topup ' + Gtoken + ' Play Token (' + intExpectedGtoken + '=>' + intAmoutGtoken + ')');
                this.test.assert(intAmoutFreeGtoken == arrAmountGtoken[1], 'The Free Play Token not changed');
            } else if (status == 'Free') {
                this.test.assert(intAmoutFreeGtoken == intExpectedGtoken, 'Exchange/Topup ' + Gtoken + ' Free Play Token (' + intExpectedGtoken + '=>' + intAmoutFreeGtoken + ')' + typeOfTransaction);
                this.test.assert(intAmoutGtoken == arrAmountGtoken[0], 'The Play Token not changed');
            } else if(status == 'Both'){
                this.test.assert(intAmoutGtoken == intExpectedGtoken, 'Exchange all free gtoken and ' + Gtoken + ' Play Token to credit type (' + intExpectedGtoken + '=>' + intAmoutGtoken + ')');
                this.test.assert(intAmoutFreeGtoken == 0, 'The Free Gtoken has been run out');
            };
            // this.test.assert(intAmoutTotalGtoken == (+intAmoutGtoken) + (+intAmoutFreeGtoken), 'The Total Gtoken equals to Gtoken plus Free Gtoken');
        }, function(){
            casper.fail('Fail at acVerifyTransactionComplete');
        });
    });
};