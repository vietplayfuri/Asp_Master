/**
 * tcEditProfile.js - testing Edit Profile Function.
 * casperjs test TestCase/03_ProfilePage/tcEditProfile.js --xunit=Reports/Report_EditProfile.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfSignUp.js');
phantom.injectJs('./Interface/itfProfilePage.js');
phantom.injectJs('./Interface/itfUPointTab.js');

var arrLocation = ["Singapore", "Indonesia", "Malaysia"];
var sEditedLocation = arrLocation[Math.floor(Math.random() * arrLocation.length)];;
var sEditedBioBox = 'I\'m sexy and I know it! ' + sUniqueValue;
var sEditedNickName = 'FoxyHandsome+' + sUniqueValue;

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

    casper.then(function() {
        casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
            casper.acSignIn(sAdminUsername, sAdminPassword);
        });
    });

    // Open the edit tab
    casper.then(function() {
        casper.thenClick(btnEditProfile);
    });

    // Edit infos on the tab
    casper.then(function() {
        casper.sendKeys(txtUserBioBox, sEditedBioBox, {
            reset: true
        });
        casper.sendKeys(txtUserNickName, sEditedNickName, {
            reset: true
        });
        casper.sendKeys(txtUserLocation, sEditedLocation, {
            reset: true,
            keepFocus: true
        });
        casper.waitForSelector(itemUserLocation, function() {
            this.click(itemUserLocation);
        }, function() {
            casper.test.fail('The Auto Complete Function of User Location doesnt worked');
        });
        casper.thenClick(btnSaveProfile);
    });

    // Verify Point
    casper.then(function() {
        casper.waitWhileVisible(btnSaveProfile, function() {
            casper.test.assertSelectorHasText(lblUserLocation, sEditedLocation, 'The User location "' + sEditedLocation + '" has been changed');
            casper.test.assertSelectorHasText(lblUserBioBox, sEditedBioBox, 'The User bio has been changed');
            casper.test.assertSelectorHasText(lblNickName, sEditedNickName, 'The User nickName "' + sEditedNickName + '" has been changed');
        }, function() {
            casper.test.fail('Fail to edit profile');
        });
    });

    // casper.thenOpen(urlTopUp).waitForText(sTitleTopUpTab, function(){
    //     if (sEditedLocation == 'Indonesia') {
    //         test.assertVisible(tabUPoint, 'The UPoint Payment is available for ' + sEditedLocation); 
    //     } else {
    //         test.assertNotVisible(tabUPoint, 'The UPoint Payment isnt available for ' + sEditedLocation);
    //     };  
    // });

    casper.run(function() {
        test.done();
    });
});