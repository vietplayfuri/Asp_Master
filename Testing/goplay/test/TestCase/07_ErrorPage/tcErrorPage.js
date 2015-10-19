/*
 * tcErrorPage.js - testing Error page.
 * casperjs test TestCase/07_ErrorPage/tcErrorPage.js --xunit=Reports/Report_ErrorPage.xml
 */
//call references
phantom.injectJs('./Global.js');

//TC01: Test 404 page
casper.test.begin('\n\n TC01_ErrorPage_404_Page \n', function suite(test) {

    casper.start(urlHomepage + sUniqueValue).waitForText('Error #404', function() {   
        test.assertTextExists('Well, the page that you’re looking for can’t be found.','The 404 page has been loaded.');
        test.assertVisible('img[src="/static/images/error-404.png"]','The 404 Picture has been displayed')
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Test 500 page
casper.test.begin('\n\n TC02_ErrorPage_500_Page \n', function suite(test) {

    casper.start(url500Page).waitForText('Error #500', function() {   
        test.assertTextExists('Oops! We\'re sorry that the system encountered an internal error','The 500 page has been loaded.');
        test.assertVisible('img[src="/static/images/error-500.png"]','The 500 Picture has been displayed')
    });

    casper.run(function() {
        test.done();
    });
});