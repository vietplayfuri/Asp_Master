casper.test.comment("Loading Global Configuration...");

//Set Default Viewport for Webpage
casper.options.viewportSize = {
    width: 1600,
    height: 928
};

// casper.options.pageSettings = {
//         loadImages:  false,        // do not load images
//         loadPlugins: false
// };

/**
 * Capture interface when TC fail
 */
casper.on('capture.saved', function(targetFile) {
    this.echo('Screenshot save at ' + targetFile);
});

// casper.on('starting', function() {
//     var d = new Date();
//     this.echo("Browser ready and starting at " + d.toTimeString());
// });

// casper.on('load.failed', function() {
//     console.log('Could not load webpage.');
//     // this.capture('./Screenshot/LoadPageFail.png');
//     // this.exit();
// });

casper.on('error', function(msg, backtrace) {
    console.log('Error: ' + msg);
    // this.exit();
});

// casper.on('page.error', function(msg, backtrace) {
//     console.log('There was an error loading the webpage.');
//     // this.capture('./Screenshot/LoadPageError.png');
//     // this.exit();
// });

// casper.test.on('fail', function() {
//    casper.capture('./Screenshot/fail.png');
// });

/**
* Capture interface when timeout issue
*/
casper.options.onWaitTimeout = function() {
    // this.capture('./Screenshot/timeout.png');
    casper.test.fail('Fail due to timeout.');
    this.exit();
};

// Before & After test
casper.test.setUp(function() {
    casper.echo('\n #################### Report of the TestCase #################### \n');
});
casper.test.tearDown(function() {
    casper.echo('\n                      End Of TestCase  \n');
});