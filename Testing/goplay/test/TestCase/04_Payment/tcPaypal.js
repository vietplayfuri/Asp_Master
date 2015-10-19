/**
 * tcPaypal.js - testing signup page.
 * casperjs test TestCase/04_Payment/tcPaypal.js --xunit=Reports/Report_Paypal.xml
 */

//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfPaypalTab.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

// var sAmountPaidRecruit = '9.99 USD';
// var sAmountPaidSkilled = '19.99 USD';
// var sAmountPaidKing = '99.99 USD';
// var sSpecificAmountGToken = '40.00 USD';
// var urlProfilePage;

// var acChoosePackage = function (selectorPackage) {
//     var arrAmoutGtoken = new Array();
//     if (typeof selectorPackage === 'undefined') { selectorPackage = 'default'; }
//     // Select one package on transaction page
//     casper.waitForSelector(tabTransaction, function(){
//         casper.click(tabTransaction);
//         casper.waitForText(sTitleTransactionTab, function(){
//             // Get Amount of each kind of Gtoken
//             arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutGtoken)));
//             arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutFreeGtoken)));
//             arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutTotalGtoken)));

//             // Navigate to Paypal Tab
//             casper.wait(1000, function(){
//                 this.click(btnTopUp); 
//             });
//             casper.wait(1000, function(){
//                 this.click(tabPaypal); 
//             });
//             // this.click(btnTopUp); 
//             // this.click(tabPaypal);
//             if (selectorPackage != 'default') {
//                 casper.then(function(){
//                     casper.wait(1000, function(){
//                         this.click(selectorPackage); 
//                     });
//                 })
//             };
            
//         }, function(){
//             casper.test.fail('Fail to choose Gtoken Packages');
//         },casper.intWaitingTime * 5);
//     });
//     return arrAmoutGtoken;
// };
// var acVerifyTransactionComplete = function (AmountDisplay, AmountGtoken, PlusGtoken){
//     // Check Point on Invoice
//     casper.then(function(){
//         casper.waitForText(sTitleInvoice,function(){
//             var idInvoice = this.fetchText('.transactions>table>tbody>tr:nth-of-type(5)>td:nth-of-type(2)');
//             var packageInvoice = this.fetchText('.transactions>table>tbody>tr:nth-of-type(1)>td:nth-of-type(2)').replace(/ /g,'');
//             var pathSaveID = pathTestDataFolder +'ID-Invoice/' + packageInvoice + '.txt';
//             fs.write(pathSaveID, idInvoice, 'w');
//             this.test.assertTextExists('success','The payment process successfully!');
//             this.test.assertTextExists(AmountDisplay,'The total of invoice is '+ AmountDisplay);
//         },function(){
//             casper.test.fail('Fail to load the invoice.');
//         },casper.intMaxWaitingTime * 3);
//     });

//     // Check Point for Transaction
//     casper.thenOpen(urlProfile,function(){
//         casper.waitForText(sTitleProfilePage,function(){
//             // Navigate to Paypal Tab
//             casper.click(tabTransaction);
//             casper.waitForText('Your Balance', function(){
//                 var intAmoutGtoken = parseFloat(this.fetchText(lblAmoutGtoken));
//                 var intAmoutFreeGtoken = parseFloat(this.fetchText(lblAmoutFreeGtoken));
//                 var intAmoutTotalGtoken = parseFloat(this.fetchText(lblAmoutTotalGtoken));

//                 this.test.assert(intAmoutGtoken == AmountGtoken[0] + PlusGtoken,'The amount of Gtoken has been increased by ' + PlusGtoken);
//                 this.test.assert(intAmoutFreeGtoken == AmountGtoken[1],'The amount of Free Gtoken isnt changed');
//                 this.test.assert(intAmoutTotalGtoken == intAmoutGtoken + intAmoutFreeGtoken,'The Total Gtoken equals to Gtoken plus Free Gtoken');
//             });
//         },function(){
//             casper.test.fail('Fail to load Profile Page. Verify amount of Gtoken');
//         },casper.intMaxWaitingTime);
//     });
// };

// //TC01: buy a Recruit package by Paypal account
// casper.test.begin('\n\n TC01_Paypal_Buy_Recruit_Package \n', function suite(test) {
//     var sAmountDisplay = sAmountPaidRecruit;
//     var intIncreaseGtoken = 10;
//     var arrAmoutGtoken = new Array();

//     // Sign out the previous session
//     casper.start(urlHomepage, function() {
//         if (casper.exists(btnSignOut)) {
//             casper.acSignOut();
//         };
//     });

//     // Login with account FoxyMax/123
//     casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
//         casper.acSignIn(sUsername, sPassword); 
//     });

//     // choose the package on Gtoken website
//     casper.then(function(){
//         urlProfilePage = this.getCurrentUrl();
//         arrAmoutGtoken = acChoosePackage(btnRecruitPackage);
//     });

//     // Login to paypal by using Khang account
//     casper.acSignInPaypal(sUsernamePaypal, sPasswordPaypal);

//     // Confirm the payment 
//     casper.then(function(){
//         casper.waitForText('Review your information',function(){
//             this.test.assertTextExists('Total $' + sAmountDisplay,'The amount of payment ($'+ sAmountDisplay +') as expected on Paypal');
//             this.click(btnContinuePaypal);
//         },function(){
//             casper.test.fail('Transaction fails at Confirm step.');
//         },casper.intMaxWaitingTime * 5);
//     });

//     // Verify Point
//     casper.then(function(){
//         acVerifyTransactionComplete(sAmountDisplay, arrAmoutGtoken, intIncreaseGtoken);
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/topup/, /You topped up 10 GToken/, /10/, true);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC02: cancel Paypal Transaction
// casper.test.begin('\n\n TC02_Paypal_Cancel_Transaction \n', function suite(test) {
//     var sAmountDisplay = sAmountPaidSkilled;

//     // Choose the package on Gtoken website
//     casper.start(urlProfilePage, function() {
//         acChoosePackage(btnSkilledPackage);
//     });  

//     // Cancel Paypal Transaction
//     casper.then(function(){
//         casper.waitForText('Pay with my PayPal account', function(){
//             casper.test.assertTextExists('Total $' + sAmountDisplay,'The amount of payment ($'+ sAmountDisplay +') as expected');
//             casper.click(btnCancelPaypal);
//         },function(){
//             casper.test.fail('Navigate to Paypal Page unsuccessfully.');
//         },casper.intWaitingTime * 5);
//     });

//     // Check Point
//     casper.then(function(){
//         casper.waitForText(sTitleTransactionTab,function(){
//             test.assertTextExists(msgCancelPaypal,'The payment process cancels successfully!');
//         },function(){
//             casper.test.fail('The cancel process failed.');
//         },casper.intWaitingTime * 5);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC03: buy a Skilled package by Paypal account
// casper.test.begin('\n\n TC03_Paypal_Buy_Skilled_Package \n', function suite(test) {
//     var sAmountDisplay = sAmountPaidSkilled;
//     var intIncreaseGtoken = 20;
//     var arrAmoutGtoken = new Array();

//     // Choose the package on Gtoken website
//     casper.start(urlProfilePage, function() {
//         arrAmoutGtoken = acChoosePackage(btnSkilledPackage);
//     });     

//     // Login to paypal by using Khang account
//     casper.acSignInPaypal(sUsernamePaypal, sPasswordPaypal);

//     // Confirm the payment 
//     casper.then(function(){
//         casper.waitForText('Review your information',function(){
//             casper.test.assertTextExists('Total $' + sAmountDisplay,'The amount of payment ($'+ sAmountDisplay +') as expected on Paypal');
//             casper.click(btnContinuePaypal);
//         },function(){
//             casper.test.fail('Transaction fails at Confirm step.');
//         },casper.intMaxWaitingTime * 5);
//     });

//     // Check Point
//     casper.then(function(){
//         acVerifyTransactionComplete(sAmountDisplay, arrAmoutGtoken, intIncreaseGtoken);
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/topup/, /You topped up 20 GToken/, /20/, true);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC04: buy a King package by Paypal account
// casper.test.begin('\n\n TC04_Paypal_Buy_King_Package \n', function suite(test) {
//     var sAmountDisplay = sAmountPaidKing;
//     var intIncreaseGtoken = 100;
//     var arrAmoutGtoken = new Array();

//     // Choose the package on Gtoken website
//     casper.start(urlProfilePage, function() {
//         arrAmoutGtoken = acChoosePackage(btnKingPackage);
//     });   

//     // Login to paypal by using Khang account
//     casper.acSignInPaypal(sUsernamePaypal, sPasswordPaypal);

//     // Confirm the payment 
//     casper.then(function(){
//         casper.waitForText('Review your information',function(){
//             casper.test.assertTextExists('Total $' + sAmountDisplay,'The amount of payment ($'+ sAmountDisplay +') as expected on Paypal');
//             casper.click(btnContinuePaypal);
//         },function(){
//             casper.test.fail('Transaction fails at Confirm step.');
//         },casper.intMaxWaitingTime * 5);
//     });

//     // Verify Point
//     casper.then(function(){
//         acVerifyTransactionComplete(sAmountDisplay, arrAmoutGtoken, intIncreaseGtoken);
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/topup/, /You topped up 100 GToken/, /100/, true);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC05: buy specific GToken amount by Paypal Transaction
// casper.test.begin('\n\n TC05_Paypal_Buy_Specific_GToken_Amount \n', function suite(test) {
//     var sAmountDisplay = sSpecificAmountGToken;
//     var intIncreaseGtoken = 40;
//     var arrAmoutGtoken = new Array();

//     // Choose the package on Gtoken website
//     casper.start(urlProfilePage, function() {
//        arrAmoutGtoken =  acChoosePackage();
//     });  

//     // Input GToken Amount
//     casper.then(function(){
//         casper.waitForText(sTitleTopUpTab, function(){
//             this.sendKeys(txtGTokenPaypal, intIncreaseGtoken.toString());
//             this.click(btnTopUpAmountGToken);
//         });
//     });

//     // Login to paypal by using Khang account
//     casper.acSignInPaypal(sUsernamePaypal, sPasswordPaypal);

//     // Confirm the payment 
//     casper.then(function(){
//         casper.waitForText('Review your information',function(){
//             casper.test.assertTextExists('Total $' + sAmountDisplay,'The amount of payment ($'+ sAmountDisplay +') as expected on Paypal');
//             casper.click(btnContinuePaypal);
//         },function(){
//             casper.test.fail('Transaction fails at Confirm step.');
//         },casper.intMaxWaitingTime * 5);
//     });

//     // Check Point
//     casper.then(function(){
//         acVerifyTransactionComplete(sAmountDisplay, arrAmoutGtoken, intIncreaseGtoken);
//     });

//     // Verify Transaction History
//     casper.then(function(){
//         casper.acCheckTransactionHistory(/topup/, /You topped up 40 GToken/, /40/, true);
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

// //TC06: Validate the Gtoken Amount
// casper.test.begin('\n\n TC06_Paypal_Validate_GToken_Amount \n', function suite(test) {
//     var arrOutOfRange = [11, 4001, -1, 0, 1, 9, -10, 0.01];
//     var lstInvalidAmount = 'List Of Invalid Amount: ';
//     var bolInValid = false;

//     // Choose the package on Gtoken website
//     casper.start(urlProfilePage, function() {
//        acChoosePackage();
//     });  

//     // Input GToken Amount
//     casper.then(function(){
//         casper.waitForText(sTitleTopUpTab, function(){
//             casper.each(arrOutOfRange, function(self, invalid) {
//                 this.sendKeys(txtGTokenPaypal, invalid.toString(), {reset : true});
//                 this.wait(1000, function(){
//                     if (!(this.exists(err10To4000)) && !(this.exists(errDenominationsOf10)) ) {
//                         lstInvalidAmount += invalid + ', ';
//                         bolInValid = true;
//                     };
//                 });
//             });    
//         });
//     });

//     casper.then(function() {
//         if (bolInValid) {
//             casper.test.fail(lstInvalidAmount);
//         } else {
//             casper.test.pass('The Buying GToken Amount by Paypal are valid');
//         };
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

casper.test.begin('\n\n TC01_Paypal_No_Running \n', function suite(test) {

    // Sign out the previous session
    casper.start().then(function() {
        casper.test.skip(1, 'NO RUNNING ON PAYPAL TESTING');
    });

    casper.run(function() {
        test.done();
    });
});