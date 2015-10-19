/**
 * tcChangePassword.js - testing Changing Password.
 * casperjs test TestCase/02_Signin/tcChangePassword.js --xunit=Reports/Report_ChangePassword.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfSignUp.js');
phantom.injectJs('./Interface/itfChangePassword.js');
phantom.injectJs('./Interface/itfProfilePage.js');

//Variables
var sNewPassword;
var sOldPassword;

//TC01: Verify user can change password
casper.test.begin('\n\n TC01_ChangePassword_Complete \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function() {
        casper.test.fail('Cannot open Home Page.');
    });

    // casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
    //     casper.acSignIn(sChangePasswordUsername, sNewPassword1);
    // });

    // // Sign up a new account to change password
    // casper.thenOpen(urlRegister).waitForText(sTitleSignUpPage, function() {
    //     casper.acSignUp(sChangePasswordUsername, sNewPassword1, sEmailUnique, sEmailUnique, '', bAcceptTOS);
    // });

    // Sign in the account if it exists
    casper.then(function() {
        // if (casper.exists(errUsernameTaken)) {
            casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
                // casper.acSignIn(sChangePasswordUsername, sNewPassword1);
                var tblValues = {};
                tblValues[txtUsername] = sChangePasswordUsername;
                tblValues[txtPassword] = sNewPassword1;

                casper.waitForSelector(frmSignIn, function() {
                    this.fillSelectors(frmSignIn, tblValues, false);
                    this.click(btnSignIn);
                }, function() {
                    casper.test.fail('Cannot not load Log In Page');
                }, casper.intMaxWaitingTime);
            });
        // };
    });

    casper.waitForText(msgLogInSuccessfully, function() {
        sOldPassword = sNewPassword1;
        sNewPassword = sNewPassword2;
        casper.test.pass('Sign In with old password "' + sOldPassword + '" successfully.');
    }, function() {
        sOldPassword = sNewPassword2;
        sNewPassword = sNewPassword1;
        casper.acSignIn(sChangePasswordUsername, sNewPassword2);
    });

    //Change password
    casper.then(function() {
        casper.click(btnChangePassword);
        casper.waitForText(sTitleChangePasswordPage, function() {
            casper.sendKeys(txtOldPassword, sOldPassword);
            casper.sendKeys(txtNewPassword, sNewPassword);
            casper.sendKeys(txtConfirmPassword, sNewPassword);
            casper.click(btnSubmit);
        });
    });

    //Sign Out
    casper.then(function() {
        casper.waitForText(msgChangePassword, function() {
            casper.test.pass('The password has been changed successfully.');
            casper.acSignOut();
        });
    });

    //Sign in with new password
    casper.then(function() {
        casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
            casper.acSignIn(sChangePasswordUsername, sNewPassword);
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Verify validation
casper.test.begin('\n\n TC02_ChangePassword_Validate_Password_Page \n', function suite(test) {
    var arrPassword = new Array(
        ["", "", "", "Old password is incorrect", "The old password cannot be blank"], ["oldPassword", "pass", "pass", "Old password is incorrect", "The old password must be correct"], [sNewPassword, "", "", "This field is required", "The New password cannot be blank"], [sNewPassword, "ChangedPassword", "", "This field is required", "The Confirm password cannot be blank"], [sNewPassword, "ChangedPassword", "ConfirmPassword", "Confirm password does not match", "The Confirm password must match"]
    );

    //validat change password page
    casper.start().then(function() {
        casper.click(btnChangePassword);
        casper.waitForText(sTitleChangePasswordPage, function() {
            casper.each(arrPassword, function(self, password) {
                casper.then(function() {
                    this.sendKeys(txtOldPassword, password[0], {
                        reset: true
                    });
                    this.sendKeys(txtNewPassword, password[1], {
                        reset: true
                    });
                    this.sendKeys(txtConfirmPassword, password[2], {
                        reset: true
                    });
                    this.click(btnSubmit);
                });

                casper.waitForText(password[3], function() {
                    test.pass(password[4]);
                }, function() {
                    test.fail(password[4]);
                });
            });
        });
    });

    casper.run(function() {
        test.done();
    });
});