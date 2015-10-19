/**
 * tcCustomerSupport.js - testing Customer Support.
 * casperjs test TestCase/08_HomePage/tcCustomerSupport.js --xunit=Reports/Report_CustomerSupport.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfFooterHeader.js');
phantom.injectJs('./Interface/itfSupportPage.js');
phantom.injectJs('./Interface/itfSignIn.js');

var subjectEmail = 'No reply (testing from GToken)';

// TC01: Send a support message for Website after signin
casper.test.begin('\n\n TC01_CustomerSupport_Send_Website_Message_By_Account \n', function suite(test) {;
    var tblValues = {};
    tblValues[cbxPlatform] = 'website';
    tblValues[txtSubject] = subjectEmail;
    tblValues[txtMessage] = 'It is a test for Website support';

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
        casper.thenClick(lnkSupport).waitForText(sTitleSupportPage);
    }, function(){
        test.fail('Cannot navigate to Sign In Page');
    });

    // Check name and Email textfields will be auto-filled
    casper.then(function(){
    	casper.wait(1000, function(){
    		var valueUserName = casper.evaluate(function(selector){
    			return document.querySelector(selector).value;
			}, txtCustomerName);
			var valueEmail = casper.evaluate(function(selector){
    			return document.querySelector(selector).value;
			}, txtCustomerEmail);    		
    		test.assert(sUsername == valueUserName, 'Auto-filled Name textbox with text ' + valueUserName);
    		test.assert(sEmail ==  valueEmail, 'Auto-filled Email textbox with text ' + valueEmail);
    	});
    });

    //Input required text field
    casper.then(function(){
    	casper.fillSelectors(frmSupport, tblValues, false);
    	casper.thenClick(btnMessageSubmit);
    });

    casper.then(function(){
    	casper.waitForText(msgSendSupport, function(){
    		test.pass('Send a message to website support successfully.');
    	}, function(){
    		test.fail('fail to send a message!');
    	});
    });
    
    casper.run(function() {
        test.done();
    });
});

// TC02: Leave all fields blank
casper.test.begin('\n\n TC02_CustomerSupport_Send_Message_With_All_Blank \n', function suite(test) {;
    var tblValues = {};
    tblValues[cbxPlatform] = 'game';
    tblValues[cbxGame] = '2';

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    // Open Support Page and click Message submit
    casper.thenClick(lnkSupport).waitForText(sTitleSupportPage,function(){
    	casper.thenClick(btnMessageSubmit);
    }, function(){
        casper.test.fail('Cannot open Support Page.');
    });

    //Input required text field
    casper.then(function(){
        casper.wait(2000, function(){
            test.assertElementCount(errEmptyField, 5, 'All of required textbox cannot be empty.');
        });  	  
    });

    casper.then(function() {
        casper.fillSelectors(frmSupport, tblValues, false);
    });

    casper.waitWhileSelector(errEmptyField, function(){
            //Input required text field
        casper.thenClick(btnMessageSubmit, function(){
            casper.wait(3000, function(){
                test.assertElementCount(errEmptyField, 5, 'All of required textbox cannot be empty.');
            });  
        });
    });



    casper.run(function() {
        test.done();
    });
});

// TC03: Send a support message for Website without signin
casper.test.begin('\n\n TC03_CustomerSupport_Send_Game_Support_Without_Login \n', function suite(test) {;
    var tblValues = {};
    tblValues[txtCustomerName] = 'No Customer Name';
    tblValues[txtCustomerEmail] = sSendingSupportGameEmail;
    tblValues[cbxPlatform] = 'game';
    tblValues[txtSubject] = subjectEmail;
    tblValues[txtMessage] = 'It is a test for Game support';
    tblValues[cbxGame] = '2';
    tblValues[cbxGameOS] = 'ios';
    tblValues[txtGameVersion] = '2.08 v13';
    tblValues[txtGameDevice] = 'Iphone 6+';
    tblValues[txtGameOSVersion] = '8.01';

    //Input required text field
    casper.start(urlSupportPage).waitForSelector(frmSupport, function() {
    	casper.fillSelectors(frmSupport, tblValues, false);
    }, function(){
        casper.test.fail('Cannot open Support Page.');
    });

    casper.thenClick(btnMessageSubmit, function(){
    	casper.waitForText(msgSendSupport, function(){
    		test.pass('Send a message to game support successfully.');
    	}, function(){
            casper.capture('./Screenshot/abc.png');
    		test.fail('fail to send a message!');
    	});
    });
    
    casper.run(function() {
        test.done();
    });
});



