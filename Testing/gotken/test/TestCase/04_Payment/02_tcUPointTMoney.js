/**
 * 02_tcUPointTMoney.js - testing buying gtoken by UPoint.
 * casperjs test TestCase/04_Payment/02_tcUPointTMoney.js --xunit=Reports/Report_UPointTMoney.xml
 */
 
// //call references
// phantom.injectJs('./Global.js');
// phantom.injectJs('./Action/acUPoint.js');
// phantom.injectJs('./Interface/itfSignIn.js');
// phantom.injectJs('./Interface/itfProfilePage.js');
// phantom.injectJs('./Interface/itfTMoneyPage.js');
// phantom.injectJs('./Interface/itfUPointTab.js');

// var sTMoneyCharge = 'Rp 600';

casper.test.begin('\n\n TC01_UPointTMoney_No_Running \n', function suite(test) {

    // Sign out the previous session
    casper.start().then(function() {
        casper.test.skip(1, 'NO RUNNING ON UPOINT TMONEY TESTING');
    });

    casper.run(function() {
        test.done();
    });
});

// //TC01: 
// casper.test.begin('\n\n TC01_UPointTMoney_Check_Info \n', function suite(test) {
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
//     	acOpenUPoint(btnTMoneyMethod);
//     });

//     casper.thenClick(btnTMoneySubmit, function(){
//     	casper.waitForText('Konfirmasi Pembayaran', function(){
//             var textTMoneyContainer = casper.fetchText(frmTMoney);
//     		test.assert(textTMoneyContainer.indexOf(sTMoneyCharge) > -1, 'The TMoney charges ' + sTMoneyCharge + '.');
//     	}, function(){
//             casper.echo(casper.getCurrentUrl());
//             test.fail('Fail to redirect to TMoney!')
//         }, casper.intMaxWaitingTime);	
//     });	

//     casper.run(function() {
//         test.done();
//     });
// });
