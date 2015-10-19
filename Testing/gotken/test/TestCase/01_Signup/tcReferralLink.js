/**
 * tcReferralLink.js - testing Referral Link.
 * casperjs test TestCase/01_Signup/tcReferralLink.js --xunit=Reports/Report_ReferralLink.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfSignUp.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');
phantom.injectJs('./Interface/itfFooterHeader.js');

var sReferralLink;
var sReferralEmail = sReferralUsername + '@gmail.com';

//TC01:
casper.test.begin('\n\n TC01_ReferralLink_Have_Referral_Lable \n', function suite(test) {

    casper.start(urlRegister).waitForSelector(frmSignUp, function() {
        casper.acSignUp(sReferralUsername, sReferralPassword, sReferralEmail, sReferralEmail, '', bAcceptTOS);
    }, function () {
        casper.test.fail('Fail to open Sign Up page!');
    });

    casper.then(function() {
        if (casper.exists(btnSignOut) == false) {
            casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
                casper.acSignIn(sReferralUsername, sReferralPassword);
            }, function(){
                casper.test.fail('Cannot open Login Page!');
            });
        };
    });

    casper.thenClick(btnShowReferralLink, function() {
        casper.waitUntilVisible(lblReferralLink, function() {
            sReferralLink = casper.evaluate(function(selector) {
                return document.querySelector(selector).value;
            }, lblReferralLink);
            var bolSelectedText = casper.evaluate(function(element) {
                var input = document.querySelector(element);
                if (typeof input.selectionStart == "number") {
                    return input.selectionStart == 0 && input.selectionEnd == input.value.length;
                } else if (typeof document.selection != "undefined") {
                    input.focus();
                    return document.selection.createRange().text == input.value;
                }
            }, lblReferralLink);

            test.pass('The referral lable has been displayed.');
            test.assert(bolSelectedText, 'The referral link is selected.');
        }, function() {
            casper.test.fail('Fail to open referral lable!');
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02:
casper.test.begin('\n\n TC02_ReferralLink_Convert_Link_To_Name \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function() {
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(sReferralLink).waitForSelector(frmSignUp, function() {
        casper.wait(1000, function() {
            var sConvertedReferral = casper.evaluate(function(selector) {
                return document.querySelector(selector).value;
            }, txtReferralID);
            test.assertEquals(sReferralUsername, sConvertedReferral, 'The Referral Link has been converted to the name');
        });
    });

    casper.run(function() {
        test.done();
    });
});