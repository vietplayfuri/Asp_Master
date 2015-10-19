/*
 * tcSignin.js - testing Sign In page.
 * casperjs test TestCase/02_Signin/tcSignIn.js --xunit=Reports/Report_SignIn.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfFooterHeader.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

var utils = require('utils');
var acFillSignInForm = function(username, password) {
    // Fill all TextBox
    var tblValues = {};
    tblValues[txtUsername] = username;
    tblValues[txtPassword] = password;
    casper.fillSelectors(frmSignIn, tblValues, false);
    casper.wait(1000, function(){
        this.click(btnSignIn);
    });   
};

//TC01: Test stucture
casper.test.begin('\n\n TC01_Signin_Stucture \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        test.assertExists(txtUsername, 'Username textbox is found.');
        test.assertExists(txtPassword, 'Password textbox is found.');
        test.assertExists(lnkForgotPassword, 'Forgot Password Link is found.');
        test.assertExists(btnSignIn, 'SignIn button is found.');
        test.assertExists(chkRemenberAcc, 'Remember Account CheckBox is found.');
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: User can sign in successfully
casper.test.begin('\n\n TC02_Signin_Complete \n', function suite(test) {
    
    casper.start(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sUsername + ' ', sPassword);
    });

    // casper.then(function(){
    //     // Return to default language
    //     this.click(lnkEnglishLanguage);
    // });

    casper.then(function(){
        casper.acSignOut();
    });

    casper.run(function() {
        test.done();
    });
});

//TC03: Login unsuccessfully with empty Username
casper.test.begin('\n\n TC03_Signin_Empty_Username \n', function suite(test) {

    casper.start(urlSignIn).waitForText(sTitleSignInPage, function() {
        acFillSignInForm('', sPassword);
    });

    casper.then(function() {
        casper.waitForSelector(errBothRequired, function() {
            casper.test.pass('The Sign In process with blank username has noticed error.');
        },function(){
            casper.test.fail('No Empty Username Error appear.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC04: Login unsuccessfully with empty Password
casper.test.begin('\n\n TC04_Signin_Empty_Password \n', function suite(test) {

    casper.start(urlSignIn).waitForText(sTitleSignInPage, function() {
        acFillSignInForm(sUsername, '');
    });

    casper.then(function() {
        casper.waitForSelector(errBothRequired, function() {
            casper.test.pass('The Sign In process with blank password has noticed error.');
        }, function(){
            casper.test.fail('No Empty Password Error appear.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC05: Login unsuccessfully with wrong Username
casper.test.begin('\n\n TC05_Signin_Wrong_Username \n', function suite(test) {

    casper.start(urlSignIn).waitForText(sTitleSignInPage, function() {
        acFillSignInForm(sUsernameUnique, sPassword);
    });

    casper.then(function() {
        casper.waitForSelector(errNotCorrect, function() {
            casper.test.pass('The Sign In process with wrong Username has noticed error.');
        }, function(){
            casper.test.fail('No Wrong Username Error appear.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC06: Login unsuccessfully with wrong Password
casper.test.begin('\n\n TC06_Signin_Wrong_Password \n', function suite(test) {

    casper.start(urlSignIn).waitForText(sTitleSignInPage, function() {
        acFillSignInForm(sUsername, sPasswordUnique);
    });

    casper.then(function() {
        casper.waitForSelector(errNotCorrect, function() {
            casper.test.pass('The Sign In process with wrong Password has noticed error.');
        },function(){
            casper.test.fail('No Wrong Password Error appear.');
        });
    });

    casper.run(function() {
        test.done();
    });
});

// //TC07: User can sign in successfully by Email
// casper.test.begin('\n\n TC07_Signin_Email_Complete \n', function suite(test) {
    
//     casper.start(urlSignIn).waitForText(sTitleSignInPage, function() {
//         casper.acSignIn(sEmail, sPassword);
//     });

//     casper.then(function(){
//         // Return to default language
//         casper.acSignOut();
//     });

//     casper.run(function() {
//         test.done();
//     });
// });
