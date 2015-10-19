// /**
//  * 01_tcUPointBalance.js - testing buying gtoken by UPoint.
//  * casperjs test TestCase/04_Payment/01_tcUPointBalance.js --xunit=Reports/Report_UPointBalance.xml
//  */
// //call references
// phantom.injectJs('./Global.js');
// phantom.injectJs('./Action/acUPoint.js');
// phantom.injectJs('./Interface/itfSignIn.js');
// phantom.injectJs('./Interface/itfProfilePage.js');
// phantom.injectJs('./Interface/itfUPointTab.js');

// var sUPointAmount = '4';

// //TC01: 
// casper.test.begin('\n\n TC01_UPointBalance_Input_Invalid_Phone_Number \n', function suite(test) {
//     // Sign out the previous session
//     casper.start(urlHomepage, function() {
//         if (casper.exists(btnSignOut)) {
//             casper.acSignOut();
//         };
//     });

//     // Login with account gtokentester/123456
//     casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
//         casper.acSignIn(sExchangeUsername, sExchangePassword);
//     });

//     casper.then(function(){
//     	acOpenUPoint(btnBalanceMethod);
//     });

//     casper.then(function(){
//         casper.waitForSelector(txtBalancePhone, function(){
//             casper.sendKeys(txtBalancePhone, '08522209343a');
//         }, function(){
//             test.fail('Fail to select Balance Deduction method!');
//         });
//     });

//     casper.thenClick(btnBalanceSubmit, function(){
//     	casper.wait(2000, function(){
//     		test.assertExists(errInvalidPhone, 'Have to input valid phone number.');
//     	});	
//     });	

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC02: 
// casper.test.begin('\n\n TC02_UPointBalance_Get_SMS \n', function suite(test) {

//     casper.start(urlProfile).waitForText(sTitleProfilePage, function(){
//     	acOpenUPoint(btnBalanceMethod);
//     });

//     casper.then(function(){
//         casper.sendKeys(txtBalancePhone, sValidUPointPhone);
//     });

//     casper.thenClick(btnBalanceSubmit, function(){
//     	casper.wait(5000, function(){
//     		test.assertExist(lblBalanceSMS, 'Get the sms from UPoint.');
//     	});	
//     });	

//     casper.run(function() {
//         test.done();
//     });
// });
