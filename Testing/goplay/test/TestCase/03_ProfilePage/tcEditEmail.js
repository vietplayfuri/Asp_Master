/**
 * tcEditEmail.js - testing Edit Email Function.
 * casperjs test TestCase/03_ProfilePage/tcEditEmail.js --xunit=Reports/Report_EditEmail.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfSignUp.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');
phantom.injectJs('./Interface/itfChangeEmailPage.js');

var sNewEmail = 'phuong.gtoken+editemail' + sUniqueValue + '@gmail.com';

//TC01: Test stucture
casper.test.begin('\n\n TC01_EditEmail_Stucture \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function() {
        casper.test.fail('Cannot open Home Page.');
    });


    // Sign in the account if it exists
    casper.then(function() {
        casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
            casper.acSignIn(sEditEmailUsername, sEditEmailPassword);
        });
    });

    casper.then(function() {
        casper.waitForSelector(btnEditEmail, function() {
            casper.click(btnEditEmail);
        });
    });

    casper.then(function() {
        casper.waitForText(sTitleChangeEmailPage, function() {
            test.assertExists(txtNewEmail, 'New Email textbox is found.');
            test.assertExists(txtRepeatedNewEmail, 'Repeated Email textbox is found.');
            test.assertExists(txtConfirmedPassword, 'Confirmed Password textbox is found.');
            test.assertExists(btnChangeEmail, 'Change Email button is found.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Check All Empty
casper.test.begin('\n\n TC02_EditEmail_All_Empty \n', function suite(test) {

    casper.start().then(function() {
        this.click(txtNewEmail);
        this.click(txtRepeatedNewEmail);
        this.click(txtConfirmedPassword);
    });
   
    casper.thenClick(txtNewEmail, function() {
        casper.waitForSelector(errEmptyField, function() {
            test.assertElementCount(errEmptyField, 3, 'All of required textbox cannot be empty.');
        }, function() {
            casper.test.fail('No empty error appear.');
        }, casper.intMaxWaitingTime * 2);
    });

    casper.run(function() {
        test.done();
    });
});

//TC03: Check Wrong Password
casper.test.begin('\n\n TC03_EditEmail_Wrong_Confirmed_Password \n', function suite(test) {

    casper.start().then(function() {
        casper.waitForSelector(txtNewEmail, function() {
            casper.sendKeys(txtNewEmail, sNewEmail);
        }, function() {
            casper.test.fail('Fail to reload the Change Email Page!');
        });
    });

    casper.then(function() {
        casper.sendKeys(txtRepeatedNewEmail, sNewEmail);
        casper.sendKeys(txtConfirmedPassword, 'WrongPassword');
    });

    casper.then(function() {
        casper.waitWhileSelector(errEmptyField, function() {
            casper.click(btnChangeEmail);
        }, function() {
        }, casper.intMaxWaitingTime * 2);
    });

    casper.then(function() {
        casper.waitForText('Incorrect password. Please try again', function() {
            test.pass('Need using correct password to change email.');
        }, function() {
            test.fail('Fail to validate password!');
        }, casper.intMaxWaitingTime);
    });

    casper.run(function() {
        test.done();
    });
});

//TC04: Check Wrong Repeated Email
casper.test.begin('\n\n TC04_EditEmail_Wrong_Repeated_Email \n', function suite(test) {

    casper.start().then(function() {
        this.sendKeys(txtNewEmail, sEmailUnique, {
            reset: true
        });
        this.sendKeys(txtRepeatedNewEmail, 'DifferentEmail', {
            reset: true
        });
        this.sendKeys(txtConfirmedPassword, sEditEmailPassword, {
            reset: true
        });
    });

    casper.then(function() {
        casper.waitForSelector(errRepeatEmailNotMatch, function() {
            casper.test.pass('Not Match Repeat Email error appears.');
        }, function() {
            casper.test.fail('Not Match Repeat Email error appears!');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC05: Verify Email Format on Change Email Page
casper.test.begin('\n\n TC05_EditEmail_Validate_Email_Format \n', function suite(test) {
    var lstInvalidEmails = ['fx.@server1.gtoken.com', 'f..x@gtoken.com', 'Fx*@gtoken.com',
        'fx@gtoken..com', 'F@x@gtoken.com', 'Fx@gtoken.', 'email@111.222.333.44444',
        'foxymaxaddress', '#@%^%#$@#$@#.com', '@gtoken.com', 'foxymaxaddress.com', 'F^x@gtoken.com'
    ];
    var lstReportWrongEmails = 'List Of Invalid Format Email: ';
    var bolWrongEmail = false;

    casper.each(lstInvalidEmails, function(self, email) {
        this.reload(function() {
            casper.wait(1000).waitForSelector(frmChangeEmail, function() {
                casper.sendKeys(txtNewEmail, email, {
                    reset: true
                });
                casper.thenClick(txtRepeatedNewEmail);
                casper.then(function() {
                    casper.waitForSelector(errInvalidEmail, function() {}, function() {
                        lstReportWrongEmails += email + ', ';
                        bolWrongEmail = true;
                    });
                });
            }, function() {
                casper.test.comment('Fail to reload the page at turn: ' + email);
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

// //TC06: the email has to be unique
// casper.test.begin('\n\n TC06_EditEmail_Email_Used_Already \n', function suite(test) {

//     casper.start().then(function() {
//         this.sendKeys(txtNewEmail, sEmail, {reset: true});
//         this.sendKeys(txtRepeatedNewEmail, sEmail, {reset: true});
//         this.sendKeys(txtConfirmedPassword, sEditEmailPassword, {reset: true});
//     });

//     casper.then(function() {
//         casper.waitForSelector(errEmailTaken, function() {
//             test.pass('The email cannot be already in use.');
//         });
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

//TC07: Edit Email Successfully
casper.test.begin('\n\n TC07_EditEmail_Edit_Email_Successfully \n', function suite(test) {

    casper.start().then(function() {
        casper.reload(function() {
            casper.waitWhileSelector(errEmailTaken, function() {
                this.sendKeys(txtConfirmedPassword, sEditEmailPassword, {
                    reset: true
                });
            }, function() {
                this.test.fail('Reload the page!');
            }, casper.intMaxWaitingTime * 2);
        })
    });

    casper.then(function() {
        this.sendKeys(txtNewEmail, sNewEmail, {
            reset: true
        });
        this.sendKeys(txtRepeatedNewEmail, sNewEmail, {
            reset: true
        });
    });

    casper.then(function() {
        casper.waitForSelector('input[class*="field-valid"]', function() {
            casper.click(btnChangeEmail);
        }, function() {
        }, casper.intMaxWaitingTime * 2);
    });

    casper.then(function() {
        casper.waitForText(msgChangeEmail, function() {
            test.assertSelectorHasText(lblUserEmail, sNewEmail, 'The new email address is displayed on profile page');
        }, function() {
            test.fail('fail to edit email!');
        }, casper.intMaxWaitingTime * 2);
    });

    casper.run(function() {
        test.done();
    });
});