/**
 * tcClubPage.js - testing Club Page.
 * casperjs test TestCase/08_HomePage/tcClubPage.js --xunit=Reports/Report_ClubPage.xml
 */
//call references
phantom.injectJs('./Global.js');

// TC01: 
casper.test.begin('\n\n TC01_ClubPage_200 \n', function suite(test) {
    casper.start(urlClubPage, function() {
        var currentHTTPStatus = casper.currentHTTPStatus;
        if (currentHTTPStatus === 200) {
            test.pass(urlClubPage + ' is eixst.');
        } else {
            test.fail(urlClubPage + ' isnt eixst.');
        };
    });

    casper.run(function() {
        test.done();
    });
});