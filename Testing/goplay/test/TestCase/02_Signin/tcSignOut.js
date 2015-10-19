/*
 * tcSignOut.js - testing Sign Out Function.
 * casperjs test TestCase/02_Signin/tcSignOut.js --xunit=Reports/Report_SignOut.xml
 */

//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfSignIn.js');

// Variables
var sTitleSignInPage = 'Login';

 //TC01: Log Out successfully
casper.test.begin('\n\n TC01_Logout_Complete \n', function suite(test) {
    
    casper.start(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sUsername, sPassword);
    });

    casper.then(function() {
        casper.acSignOut();
    });

    casper.run(function() {
        test.done();
    });
});