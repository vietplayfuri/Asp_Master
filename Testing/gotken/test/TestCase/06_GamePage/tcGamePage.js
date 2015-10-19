/**
 * tcGamePage.js - testing Game Page.
 * casperjs test TestCase/06_GamePage/tcGamePage.js --xunit=Reports/Report_GamePage.xml
 */
// Call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfGamePage.js');

// Variables
var Links;
var lnkiOSDownload;
var lnkAndroidDownload;

// TC01: Test stucture
casper.test.begin('\n\n TC01_GamePage_Stucture \n', function suite(test) {

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function() {
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlGamePage).waitForText(sTitleGamePage, function() {
        Links = casper.getElementsAttribute(lblGameTitle, 'href');
        lnkiOSDownload = casper.getElementsAttribute(icoiOSDowload, 'href');
        lnkAndroidDownload = casper.getElementsAttribute(icoAndroidDowload, 'href');

        test.assertElementCount(divGameBrief, Links.length, 'The page shows ' + Links.length + ' Game Briefs.');
        test.assertElementCount(imgGameThump, Links.length, 'The page shows ' + Links.length + ' Game Thumps.');
        test.assertElementCount(lblGameGerne, Links.length, 'The page shows ' + Links.length + ' Game Gernes.');
    });

    casper.run(function() {
        test.done();
    });
});

// TC02: Test Each Game Detail
casper.test.begin('\n\n TC02_GamePage_Check_Game_Detail\ \n', function suite(test) {
    var iCount = 0;
    casper.start(urlGamePage).waitForText(sTitleGamePage, function() {
        casper.each(Links, function(self, link) {
            var urlCheck = urlHomepage + link;
            casper.thenOpen(urlCheck).waitForSelector('.statistic>h5', function() {
                // Get Position of Intro Header and Statistic Header
                var JSONStatisticBounds = this.getElementBounds('.statistic>h5');
                var JSONIntroBounds = this.getElementBounds('.introduction>h5');
                var sTitleGameDetail = this.fetchText(lblGameDetailTitle);

                delete JSONStatisticBounds.left;
                delete JSONStatisticBounds.width;
                delete JSONIntroBounds.left;
                delete JSONIntroBounds.width;

                JSONStatisticBounds = JSON.stringify(JSONStatisticBounds);
                JSONIntroBounds = JSON.stringify(JSONIntroBounds);
                // Make sure that both of them are parallel
                test.assertEquals(JSONStatisticBounds, JSONIntroBounds, 'The Outline has been checked on ' + sTitleGameDetail);
            }, function() {
                casper.test.fail('Cannot open Game Detail Page');
            });

            // // Check title Game and Image 
            // casper.then(function(){

            // });

            // // Check All download links match the ones on game page
            // casper.then(function(){
            //     var lnkGetIOS = casper.getElementAttribute(icoiOSDowload, 'href');
            //     var lnkGetAndroid= casper.getElementAttribute(icoAndroidDowload, 'href');
            //     //Check link download
            //     if (lnkiOSDownload[iCount] == lnkGetIOS && lnkAndroidDownload[iCount] == lnkGetAndroid) {
            //         test.pass('All of the download links match together');
            //     }else {
            //         test.fail('All of the download links don\'t match together');
            //     };
            // });
        });
    });

    casper.run(function() {
        test.done();
    });
});

// TC03: Test download iOS verion links
casper.test.begin('\n\n TC03_GamePage_Download_iOS_Version \n', function suite(test) {
    casper.start(urlGamePage).waitForText(sTitleGamePage, function() {
        if (utils.isArray(lnkiOSDownload)) {
            casper.each(lnkiOSDownload, function(self, link) {
                this.thenOpen(link, function() {
                    var currentHTTPStatus = casper.currentHTTPStatus;
                    if (currentHTTPStatus === 200) {
                        test.pass(link + ' is eixst.');
                    } else {
                        test.fail(link + ' isnt eixst.');
                    };
                });
            });
        } else {
            test.fail('Cannot get Download iOS Link.');
        };
    });

    casper.run(function() {
        test.done();
    });
});

// TC04: Test download Android verion links
casper.test.begin('\n\n TC04_GamePage_Download_Android_Version \n', function suite(test) {
    casper.start(urlGamePage).waitForText(sTitleGamePage, function() {
        if (utils.isArray(lnkAndroidDownload)) {
            casper.each(lnkAndroidDownload, function(self, link) {
                this.thenOpen(link, function() {
                    var currentHTTPStatus = casper.currentHTTPStatus;
                    if (currentHTTPStatus === 200) {
                        test.pass(link + ' is eixst.');
                    } else {
                        test.fail(link + ' isnt eixst.');
                    };
                });
            });
        } else {
            test.fail('Cannot get Download Android Link.');
        };
    });

    casper.run(function() {
        test.done();
    });
});