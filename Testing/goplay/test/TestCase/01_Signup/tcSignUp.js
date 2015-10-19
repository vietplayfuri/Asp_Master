/**
 * tcSignup.js - testing signup page.
 * casperjs test TestCase/01_Signup/tcSignUp.js --xunit=Reports/Report_SignUp.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfSignUp.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

// Actions
acFillSignUpForm = function(username, password, email, repeated_email, referralID, status) {
    // Fill all TextBox
    var tblValues = {};
    tblValues[txtUsername] = username;
    tblValues[txtEmail] = email;
    tblValues[txtRepeatedEmail] = repeated_email;
    tblValues[txtPassword] = password;
    tblValues[txtReferralID] = referralID;
    tblValues[rdnAcceptTOS] = status;
    casper.fillSelectors(frmSignUp, tblValues, false);
    casper.wait(1000, function() {
        this.click(btnSignUp);
    });
};

var intGTokenFromReferralID = 5;
var sTC11Uername = sUsernameUnique + '_tc11';
var sTC11Email = 'phuong.gtoken+tc11' + sUniqueValue + '@gmail.com';

var sTC12Uername = sUsernameUnique + '_tc12';
var sTC12Email = 'phuong.gtoken+tc12' + sUniqueValue + '@gmail.com';

//TC01: Test stucture
casper.test.begin('\n\n TC01_Signup_Stucture \n', 7, function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlRegister).waitForSelector(frmSignUp, function() {
        test.assertExists(txtUsername, 'Username textbox is found.');
        test.assertExists(txtPassword, 'Password textbox is found.');
        test.assertExists(txtEmail, 'Email textbox is found.');
        test.assertExists(txtRepeatedEmail, 'Repeated Email textbox is found.');
        test.assertExists(txtReferralID, 'ReferralID textbox is found.');
        test.assertExists(btnSignUp, 'Signup button is found.');
        test.assertExists(rdnAcceptTOS, 'Accept TOS radiobutton is found.');
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: User can sign up successfully
casper.test.begin('\n\n TC02_Signup_Complete \n', function suite(test) {

    casper.start(urlRegister, function() {
        casper.acSignUp(sUsername, sPassword, sEmail, sEmail, '', bAcceptTOS);

        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sChatUsername, sChatPassword, sChatEmail, sChatEmail, '', bAcceptTOS);

        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sTC11Uername, sPassword, sTC11Email, sTC11Email, '', bAcceptTOS);

        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sTC12Uername, sChatPassword, sTC12Email, sTC12Email, '', bAcceptTOS);

        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    // Create a New Account to transfer
    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sReferralUsername, sReferralPassword, sEmailUnique, sEmailUnique, '', bAcceptTOS);

        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    // Sign up a new account to change password
    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sChangePasswordUsername, sNewPassword1, sEmailUnique, sEmailUnique, '', bAcceptTOS);

        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    // Sign up a new account to edit email
    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sEditEmailUsername, sEditEmailPassword, sEditEmailEmail, sEditEmailEmail, '', bAcceptTOS);

        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sAdminUsername, sAdminPassword, sAdminEmail, sAdminEmail, '', bAcceptTOS);

        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sAccountantUsername, sAccountantPassword, sAccountantEmail, sAccountantEmail, '', bAcceptTOS);

        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sCustomerSupportUsername, sCustomerSupportPassword, sCustomerSupportEmail, sCustomerSupportEmail, '', bAcceptTOS);
    
        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sGameAccountantUsername, sGameAccountantPassword, sGameAccountantEmail, sGameAccountantEmail, '', bAcceptTOS);
    
        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sGameAdminUsername, sGameAdminPassword, sGameAdminEmail, sGameAdminEmail, '', bAcceptTOS);
    
        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });

    // Create a New Account to transfer
    casper.thenOpen(urlRegister, function() {
        casper.acSignUp(sReceiverUsername, sReceiverPassword, sEmailUnique, sEmailUnique, '', bAcceptTOS);
    
        casper.then(function() {
            if (casper.exists(btnSignOut)) {
                casper.acSignOut();
            };
        });
    });
    
    casper.run(function() {
        test.done();
    });
});

//TC03: User cannot sign up with email and username exist
casper.test.begin('\n\n TC03_Signup_Exist_Username_Email \n', function suite(test) {

    casper.start(urlRegister, function() {
        acFillSignUpForm(sUsername, sPassword, sEmail, sEmail, sReferralID, bAcceptTOS);
    });

    casper.then(function() {
        casper.waitForSelector(errUsernameTaken, function() {
            test.assertExists(errUsernameTaken, 'Exist Username error appears');
            // test.assertExists(errEmailTaken, 'Exist Email error appears');
        },function(){
            casper.test.fail('Exist Error not appear.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC04: Check all field empty 
casper.test.begin('\n\n TC04_Signup_Empty_Info \n', function suite(test) {

    casper.start(urlRegister).waitForSelector(frmSignUp, function() {
        this.click(txtUsername);
        this.click(txtPassword);
        this.click(txtEmail);
        this.click(txtRepeatedEmail);
        this.click(txtReferralID);
    });

    casper.then(function() {
        casper.waitForSelector(errEmptyField, function() {
            test.assertElementCount(errEmptyField, 4, 'All of required textbox cannot be empty.');
        }, function() {
            casper.test.fail('No empty error appear.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC05: Check Repeated Email empty 
casper.test.begin('\n\n TC05_Signup_Empty_Repeated_Email \n', function suite(test) {

    casper.start(urlRegister).waitForSelector(frmSignUp, function() {
        acFillSignUpForm(sUsernameUnique, sPassword, sEmailUnique, '', sReferralID, false);
        this.click(txtRepeatedEmail);
    });

    casper.then(function() {
        casper.waitForSelector(errEmptyField, function() {
            test.assertExists(errEmptyField, 'The Confirm Email needs to be filled.');
        },function(){
            casper.test.fail('Repeat Email Error not appear.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC06: Check Invalid Format Email  
casper.test.begin('\n\n TC06_Signup_Invalid_Format_Email \n', function suite(test) {

    var lstInvalidEmails = ['fx.@server1.gtoken.com', 'f..x@gtoken.com', 'Fx*@gtoken.com',
        'fx@gtoken..com', 'F@x@gtoken.com', 'Fx@gtoken.', 'email@111.222.333.44444',
        'foxymaxaddress', '#@%^%#$@#$@#.com', '@gtoken.com', 'foxymaxaddress.com', 'F^x@gtoken.com'
    ];
    var lstReportWrongEmails = 'List Of Invalid Format Email: ';
    var bolWrongEmail = false;

    casper.start(urlRegister).waitForSelector(frmSignUp, function() {
        casper.each(lstInvalidEmails, function(self, email) {
            this.reload(function() {
                // casper.sendKeys(txtEmail, '', {reset : true});
                // casper.sendKeys(txtRepeatedEmail, '', {reset : true});
                casper.waitForSelector(frmSignUp, function() {
                    casper.wait(2000, function() {
                        acFillSignUpForm(sUsernameUnique, sPassword, email, email, sReferralID, false);
                    });
                    casper.waitForSelector(errInvalidEmail, function() {}, function() {
                        casper.wait(2000, function() {
                            lstReportWrongEmails += email + ', ';
                            bolWrongEmail = true;
                        });
                    }, casper.intMaxWaitingtime*2);
                }, function() {
                    casper.test.comment('Fail to reload the page at turn: ' + email);
                });
            });
        });
    });

    casper.then(function() {
        if (bolWrongEmail) {
            casper.test.fail(lstReportWrongEmails);
        } else {
            casper.test.pass('The Emails are valid');
        };
    });

    casper.run(function() {
        test.done();
    });
});

//TC07: Check the Email can sign up with special char or not. 
casper.test.begin('\n\n TC07_Signup_Invalid_Email_With_Special_Char \n', function suite(test) {

    var lstSpecialChars = ['\"', '\\', '/', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')',
        '{', '}', '\'', ':', ';', '[', ']', '|', '<', '>', '=', '?', ' ', ','
    ];
    var lstReportSpecialChars = 'List Of special Chars accepted: ';
    var bolWrongEmail = false;
    var sEmailWithSpecialChar = '';

    casper.start(urlRegister).waitForSelector(frmSignUp, function() {
        casper.each(lstSpecialChars, function(self, char) {
            this.reload(function() {
                casper.waitForSelector(frmSignUp, function() {
                    casper.wait(2000, function() {
                        sEmailWithSpecialChar = 'phuong+' + sUniqueValue + '_' + char + '@gtoken.com';
                        acFillSignUpForm(sUsernameUnique, sPassword, sEmailWithSpecialChar, sEmailWithSpecialChar, sReferralID, false);
                    });
                    casper.waitForSelector(errInvalidEmail, function() {}, function() {
                        casper.wait(2000, function() {
                            lstReportSpecialChars += char + ' ';
                            bolWrongEmail = true;
                        });
                    }, casper.intMaxWaitingtime);
                }, function() {
                    casper.test.comment('Fail to reload the page at turn: ' + char);
                });
            });
        });
    });

    casper.then(function() {
        if (bolWrongEmail) {
            casper.test.fail(lstReportSpecialChars);
        } else {
            casper.test.pass('The Emails are valid');
        };
    });

    casper.run(function() {
        test.done();
    });
});

//TC08: Check the Username can sign up with special char or not. 
casper.test.begin('\n\n TC08_Signup_Invalid_Username_With_Special_Char \n', function suite(test) {

    var lstSpecialChars = ['\"', '\\', '/', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')',
        '{', '}', '\'', ':', ';', '[', ']', '|', '<', '>', '=', '+', '?', ',', ' 2'
    ];
    var lstReportSpecialChars = 'List Of special Chars accepted: ';
    var bolWrongEmail = false;
    var sUsernameWithSpecialChar = '';

    casper.start(urlRegister).waitForSelector(frmSignUp, function() {
        casper.each(lstSpecialChars, function(self, char) {
            this.reload(function() {
                casper.waitForSelector(frmSignUp, function() {
                    sUsernameWithSpecialChar = 'Fx_' + sUniqueValue + '_' + char;
                    acFillSignUpForm(sUsernameWithSpecialChar, sPassword, sEmailUnique, sEmailUnique, sReferralID, false);
                    casper.waitForSelector(errInvalidUsername, function() {}, function() {
                        lstReportSpecialChars += char + ' ';
                        bolWrongEmail = true;
                    });
                }, function() {
                    casper.test.comment('Fail to reload the page at turn: ' + char);
                });
            });
        });
    });

    casper.then(function() {
        if (bolWrongEmail) {
            casper.test.fail(lstReportSpecialChars);
        } else {
            casper.test.pass('The Username are valid');
        };
    });

    casper.run(function() {
        test.done();
    });
});

//TC09: Check Repeat Email not math
casper.test.begin('\n\n TC09_Signup_Wrong_Repeted_Email \n', function suite(test) {

    casper.start(urlRegister).waitForSelector(frmSignUp, function() {
        acFillSignUpForm(sUsernameUnique, sPassword, sEmailUnique, 'abc@gmail.com', sReferralID, false);
    });

    casper.then(function() {
        casper.waitForSelector(errRepeatEmailNotMatch, function() {
            casper.test.pass('Not Match Repeat Email error appears.');
        }, function() {
            casper.test.fail('check the repeated email improperly');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC10: User can sign up with Referral ID successfully
casper.test.begin('\n\n TC10_Signup_With_ReferralID \n', function suite(test) {

    casper.start(urlRegister).waitForSelector(frmSignUp, function() {
        casper.acSignUp(sUsernameUnique, sPassword, sEmailUnique, sEmailUnique, sReferralID, bAcceptTOS);
    }, function () {
        casper.test.fail('Cannot open Sign Up page!')
    });

    // casper.then(function() {
    //     casper.waitUntilVisible(lblAmoutTotalGtoken, function() {
    //         var intTotalGToken = parseInt(casper.fetchText(lblAmoutTotalGtoken));
    //         casper.test.assertEquals(intTotalGToken, intGTokenFromReferralID, 'Receive ' + intTotalGToken + ' Free GToken.');
    //     }, function () {
    //         casper.test.fail('Cannot get Total Gtoken!');
    //     });
    // });

    casper.waitForSelector(tabFriends).thenClick(tabFriends, function() {
        casper.waitForText(sReferralUsername, function() {
            casper.test.pass('the username ' + sReferralUsername + ' has been added friend.');
        }, function () {
            casper.test.fail('Cannot find the referral name on friend list!')
        });
    });

    // Verify Transaction History
    casper.thenClick(tabTransaction, function() {
        casper.acCheckTransactionHistory('small-gtoken', 'Receive Free Play Token', 1);
    });

    casper.run(function() {
        test.done();
    });
});

//TC11: User can add Referral ID later
casper.test.begin('\n\n TC11_Signup_Add_ReferralID \n', function suite(test) {

    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    });

    // Sign up a new account without Referral ID
    casper.thenOpen(urlSignIn, function() {
        casper.acSignIn(sTC11Uername, sPassword);
    });

    // Add Referral ID
    casper.thenClick(btnEditProfile, function() {
        casper.waitUntilVisible(txtProfileReferralID, function() {
            casper.sendKeys(txtProfileReferralID, sTC12Uername);
        }, function(){
            casper.test.fail('Fail to get referral ID!');
        });
    });
    
    casper.thenClick(btnSaveProfile, function() {
        casper.waitForText(msgEditProfile, function() {
            test.assertSelectorHasText(lblReferee, sTC12Uername, 'The Referral ID has been added.');
        }, function(){
            test.fail('Fail to add Refferal ID');
        });
    });

    casper.thenClick(tabFriends, function() {
        casper.waitForText(sTC12Uername, function() {
            casper.test.pass('the username ' + sTC12Uername + ' has been added friend.');
        });
    });

    casper.thenClick(tabTransaction).waitUntilVisible(btnExchange);

    // Verify Transaction History
    casper.then(function() {
        casper.acCheckTransactionHistory('small-gtoken', 'Receive Free Play Token ', 1);
    });

    // Check Referral ID textbox not exist
    casper.thenClick(tabProfile).waitUntilVisible(btnEditProfile, function() {
        casper.thenClick(btnEditProfile, function() {
            casper.waitUntilVisible(btnSaveProfile, function() {
                test.assertNotVisible(txtProfileReferralID, 'The Referral ID textbox disappreares after adding referral');
            }, function(){
                test.fail('Fail to add referral!');
            });
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC12: User can use two-way adding
casper.test.begin('\n\n TC12_Signup_Two_Ways_Adding \n', function suite(test) {

    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        }
    });

    casper.thenOpen(urlSignIn, function() {
        casper.acSignIn(sTC12Uername, sPassword);
    });

    // Add Referral ID
    casper.thenClick(btnEditProfile, function() {
        casper.waitUntilVisible(txtProfileReferralID, function() {
            casper.sendKeys(txtProfileReferralID, sTC11Uername);
        }, function(){
            test.fail('Fail to open Edit Profile panel!');
        });
    });

    casper.then(function(){
        casper.wait(2000, function(){
            casper.thenClick(btnSaveProfile, function(){
                casper.waitForText('Uh oh, you’ve referred this one already', function(){
                    test.pass('Cannot add referral ID circularly.');
                });
            }); 
        })
    });

    casper.run(function() {
        test.done();
    });
});