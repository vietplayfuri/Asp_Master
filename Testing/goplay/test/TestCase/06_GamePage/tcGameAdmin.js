/**
 * tcGameAdmin.js - testing Game Admin.
 * casperjs test TestCase/06_GamePage/tcGameAdmin.js --xunit=Reports/Report_GameAdmin.xml
 */
// Call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfGamePage.js');
phantom.injectJs('./Interface/itfAdminGameTab.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfFooterHeader.js');

// Variables
var sName = 'GTokenGameFake ';
var sShortDescription = 'Short Description ';
var sDescription = 'Game Description ';
var sGenre = 'Strategy ';
var siOSLink = 'https://itunes.apple.com/';
var sAndroidLink = 'https://play.google.com/';
var sContentRating = 'Content rating +9 ';
var sChangeLog = 'Change Log ';

var sIndoLanguage = 'Indo ';
var sIndoShortDescription = sShortDescription + sIndoLanguage;
var sIndoDescription = sDescription + sIndoLanguage;
var sIndoGenre = sGenre + sIndoLanguage;
var sIndoChangeLog = sChangeLog + sIndoLanguage;
var sIndoContentRating = sContentRating + sIndoLanguage ;

var sEditName = sName + sUniqueValue;
var sEditShortDescription = sShortDescription + sUniqueValue;
var sEditDescription = sDescription + sUniqueValue;
var sEditGenre = sGenre + sUniqueValue;
var sEditiOSLink = siOSLink + sUniqueValue;
var sEditAndroidLink = sAndroidLink + sUniqueValue;

var intGameID;
var urlGameDetail;

var acFillGameForm = function(name, shortDescription, description, genre, ioslink, andlink, icon, thumb, cover, active){
    casper.then(function(){
        var tblValues = {};
            tblValues[cbxStudio] = '5';
            tblValues[txtGameName] = name;
            tblValues[txtGameShortDescription] = shortDescription;
            tblValues[txtGameDescription] = description;
            tblValues[txtGameContentRating] = sContentRating;
            tblValues[txtGameChangeLog] = sChangeLog;
            tblValues[txtGameGenre] = genre;
            tblValues[txtiOSDownload] = ioslink; 
            tblValues[txtAndroidDownload] = andlink;
            
        this.page.uploadFile(btnUploadIcon, icon); 
        this.page.uploadFile(btnUploadThumbnail, thumb);
        this.page.uploadFile(btnUploadBanner, cover);
        var status = this.evaluate(function() {
            return document.getElementById('is_active').checked;
        });
        if (status !== active) {
            this.click(chkGameActive);
        };
        if (casper.exists(frmAddGame)) {
            this.fillSelectors(frmAddGame, tblValues, false);
        } else {
            this.fillSelectors(frmEditGame, tblValues, false);
        };
        
    }); 
};

var acFillLocallizedForm = function(UniqueValue){
    casper.then(function(){
        if (typeof UniqueValue === 'undefined') { UniqueValue = ''; }
        var sInputValue = UniqueValue;
        var tblValues = {};
            tblValues[txtIndoGameShortDescription] = sIndoShortDescription + sInputValue;
            tblValues[txtIndoGameDescription] = sIndoDescription + sInputValue;
            tblValues[txtIndoGameGenre] = sIndoGenre + sInputValue;
            tblValues[txtIndoGameChangeLog] = sIndoChangeLog + sInputValue;
            tblValues[txtIndoGameContentRating] = sIndoContentRating + sInputValue;

        casper.click(btnShowLocalizedShortDescription);
        casper.click(btnShowLocalizedDescription);
        casper.click(btnShowLocalizedGenre);
        casper.click(btnShowLocalizeChangeLog);
        casper.click(btnShowLocalizeContentRating);


        if (casper.exists(frmAddGame)) {
            this.fillSelectors(frmAddGame, tblValues, false);
        } else {
            this.fillSelectors(frmEditGame, tblValues, false);
        };
        
    }); 
};

// TC01: Add a new game on Admin Panel
casper.test.begin('\n\n TC01_GameAdmin_Add_Game \n', function suite(test) {
    var sCreateGame = sName;

    // Sign out the previous session
    casper.start(urlHomepage).waitForText(sTitleHomePage, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    // Login with acc FoxyAdmin/123
    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sAdminUsername, sAdminPassword);
    });

    casper.thenOpen(urlAdminPage).waitForText(sTitleAdminPage, function() {
        casper.click(topMenuGames);
        casper.waitUntilVisible(btnAddGame, function(){
            this.click(btnAddGame);
        }, function(){
            casper.test.fail('Cannot open Games Tab');
        });
    });

    // Fill details for a game
    casper.then(function(){
        casper.waitForSelector(btnUploadBanner, function(){
            acFillGameForm(sName, sShortDescription, sDescription, sGenre, siOSLink, sAndroidLink, pathIconImage, pathThumbImage, pathCoverImage, true);
            acFillLocallizedForm();
        });
    });

    // Check Point
    casper.thenClick(btnSubmitGame, function(){
        casper.waitForText('Successfully added game ' + sCreateGame, function(){
            intGameID = this.fetchText(lblGameID);
            test.pass('The game '+ sCreateGame +' has been created.');
        }, function(){
            test.fail('Fail to create a game '+ sCreateGame +'!');
        }, casper.intMaxWaitingTime);
    });

    casper.run(function() {
        test.done();
    });
});

// TC02: View a current game on Admin Panel
casper.test.begin('\n\n TC02_GameAdmin_View_Game \n', function suite(test) {
    var sViewGame = sName;

    casper.start(urlAdminPage).waitForText(sTitleAdminPage, function() {
        casper.click(topMenuGames);        
    });

    // Fill details for a game
    casper.then(function(){
        casper.waitForSelector(btnAddGame, function(){
            casper.click(btnViewGame(intGameID));
        });
    });

    casper.then(function(){
        casper.waitForText('Game: ' + sViewGame, function(){
            test.assertTextExists(sName, 'The Game Name displayed');
            test.assertTextExists(sShortDescription,'The Short Description displayed');
            test.assertTextExists(sDescription,'The Description displayed');
            test.assertTextExists(sGenre,'The Genre displayed');
            test.assertTextExists(sContentRating, 'The Content Rating displayed');
            test.assertTextExists(sChangeLog, 'The Change Log displayed');
            test.assertExists(lblActiveStatus, 'The status of game is active');
            test.assertExists('a[href="'+ siOSLink + '"]', 'iOS Download Link added');
            test.assertExists('a[href="'+ sAndroidLink + '"]', 'Android Download Link added');

        }, function(){
            test.fail('Fail to view a game '+ sViewGame +'!');
        }, casper.intMaxWaitingTime);
    });

    casper.run(function() {
        test.done();
    });
});

// TC03: The just-added game displays on Games List
casper.test.begin('\n\n TC03_GameAdmin_Added_Game_Available_On_GamePage \n', function suite(test) {
    var sViewGame = sName;

    casper.start(urlGamePage).waitForText(sTitleGamePage, function() {
        test.assertExists(divGame(intGameID), 'The Game with ID ' + intGameID +' displayed on Games List');    
    });

    casper.run(function() {
        test.done();
    });
});

// TC04: 
casper.test.begin('\n\n TC04_GameAdmin_Check_Game_Localization_On_ProfilePage \n', function suite(test) {
    var sViewGame = sName;
    var intReloadTimes = 10;

    // Sign out the previous session
    casper.start().then(function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    });

    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sUsername, sPassword);
    });

    casper.then(function() {
        casper.repeat(intReloadTimes, function() {
            if (intReloadTimes != -1) {
                casper.thenOpen(urlProfile, function() {
                    casper.waitForText('Recommended Games', function() {
                        if (casper.visible(divGame(intGameID))) {
                            intReloadTimes = -1;
                        };
                    });
                });
            };
        });
    });

    // casper.thenClick(lnkIndoLanguage, function() {
    //     casper.waitForText('Indonesian', function() {
    //         test.assertTextExists(sIndoGenre, 'The Indonesian Genre displayed on ProfilePage.');
    //     });
    // });

    casper.run(function() {
        test.done();
    });
});

// TC05: Edit a current game on Admin Panel
casper.test.begin('\n\n TC05_GameAdmin_Edit_Game \n', function suite(test) {
    var sEditGame = sEditName;

    casper.start().thenClick(lnkEnglishLanguage, function(){
        casper.waitForText('English', function(){
            test.pass('Back to English version successfully.');
        })
    });

    // Sign out the previous session
    casper.thenOpen(urlHomepage).waitForSelector(btnSignOut, function() {
        if (casper.exists(btnSignOut)) {
            casper.acSignOut();
        };
    }, function(){
        casper.test.fail('Cannot open Home Page.');
    });

    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sAdminUsername, sAdminPassword);
    });

    casper.thenOpen(urlAdminPage).waitForText(sTitleAdminPage, function() {
        casper.click(topMenuGames);        
    });

    // Select a game to edit based on ID game
    casper.then(function(){
        casper.waitForSelector(btnAddGame, function(){
            casper.click(btnEditGame(intGameID));
        });
    });

    // Edit detail of game
    casper.then(function(){
        casper.waitForText('Edit Game: ', function(){
            acFillGameForm(sEditName, sEditShortDescription, sEditDescription, sEditGenre, sEditiOSLink, sEditAndroidLink, pathIconImage, pathThumbImage, pathCoverImage, false);
            acFillLocallizedForm(sUniqueValue);
            this.thenClick(btnSubmitGame);
        });
    });

    casper.then(function(){
        casper.waitForText('Successfully updated game ' + sEditGame, function(){
            test.pass('The game '+ sEditGame +' has been edited.');
            test.assertTextExists(sEditGame, 'Edit game name successfully.');
            test.assertTextExists(sEditShortDescription,'Edit short game description successfully.');
            test.assertTextExists(sEditDescription,'Edit game description successfully.');
            test.assertTextExists(sEditGenre,'Edit game genre successfully.');
            test.assertDoesntExist(lblActiveStatus, 'The status of game is inactive');
            test.assertExists('a[href="'+ sEditiOSLink + '"]', 'Edit iOS download link successfully');
            test.assertExists('a[href="'+ sEditAndroidLink + '"]', 'Edit Android download link successfully');
        }, function() {
            test.fail('Fail to edit a game '+ sEditGame +'!');
        }, casper.intMaxWaitingTime);   
    });

    casper.run(function() {
        test.done();
    });
});

// TC06: Check setting private of game
casper.test.begin('\n\n TC06_GameAdmin_Check_GamePage_Localization \n', function suite(test) {
    
    casper.start(urlGamePage).waitForText(sTitleGamePage, function() {
        casper.click(lnkIndoLanguage);    
    });

    casper.then(function(){
        casper.waitForText('Indonesian', function() {
            this.scrollToBottom();
            casper.wait(2000, function(){
                this.scrollToBottom();
                test.assertTextExists(sIndoShortDescription + sUniqueValue,'The Indonesian Short Description displayed');
                urlGameDetail = urlHomepage + casper.getElementsAttribute(lnkGameDetail(sUniqueValue), 'href');
            });            
        });
    });

    casper.run(function() {
        test.done();
    });
});

// TC07: 
casper.test.begin('\n\n TC07_GameAdmin_Check_GameDetail_Localization \n', function suite(test) {

    casper.start(urlGameDetail).waitForText('Informasi Tambahan', function(){
        test.assertTextExists(sIndoDescription + sUniqueValue,'The Indonesian Description displayed');
        test.assertTextExists(sIndoGenre + sUniqueValue,'The Indonesian Genre displayed');
        test.assertTextExists(sIndoContentRating + sUniqueValue, 'The Indonesian Content Rating displayed');
        test.assertTextExists(sIndoChangeLog + sUniqueValue, 'The Indonesian Change Log displayed');
    });

    casper.run(function() {
        test.done();
    });
});

// TC08: Check setting private of game
casper.test.begin('\n\n TC08_GameAdmin_CheckPrivate \n', function suite(test) {
    var sNormalUser = sUsername;

    casper.start(urlHomepage).waitForSelector(lnkEnglishLanguage, function(){
        casper.click(lnkEnglishLanguage);
        casper.waitForText('English', function(){
            test.pass('Back to English version successfully.');
        });
    });

    casper.waitForText(sTitleHomePage, function() {
        casper.acSignOut();
    }); 

    casper.thenOpen(urlGamePage).waitForText(sTitleGamePage, function() {
        test.assertDoesntExist(divGame(intGameID), 'The Game with ID ' + intGameID + ' has been set Private');    
    });

    // Login with acc FoxyMax/123
    casper.thenOpen(urlSignIn).waitForText(sTitleSignInPage, function() {
        casper.acSignIn(sNormalUser, sPassword);
    });

    casper.thenOpen(urlGamePage).waitForText(sTitleGamePage, function() {
        test.assertDoesntExist(divGame(intGameID), 'The Game with ID ' + intGameID +' not displayed for normal user ' + sNormalUser);    
    });

    casper.run(function() {
        test.done();
    });
});



























// // TC06: Delete a current game on Admin Panel
// casper.test.begin('\n\n TC06_GameAdmin_Delete_Game \n', function suite(test) {
//     var sDeleteGame = sEditName;

//     // Login with acc FoxyAdmin/123
//     casper.start(urlSignIn).waitForText(sTitleSignInPage, function() {
//         casper.acSignIn(sAdminUsername, sAdminPassword);
//     });

//     casper.thenOpen(urlAdminPage).waitForText(sTitleAdminPage, function() {
//         casper.click(topMenuGames);        
//     });

//     // Fill details for a game
//     casper.then(function(){
//         casper.waitForSelector(btnAddGame, function(){
//             casper.click(btnDeleteGame(intGameID));
//             casper.click(btnConfirmDeleteGame(intGameID));
//         });
//     });

//     casper.then(function(){
//         casper.waitForText('Removed game ' + sDeleteGame, function(){
//             test.pass('The Game '+ sDeleteGame +' has been deleted.');
//         }, function(){
//             test.fail('Fail to delete a game '+ sDeleteGame +'!');
//         }, casper.intMaxWaitingTime);
//     });

//     casper.thenOpen(urlGamePage).waitForText(sTitleGamePage, function() {
//         test.assertDoesntExist(divGame(intGameID), 'The Game with ID ' + intGameID +' not displayed on Games List');    
//     });

//     casper.run(function() {
//         test.done();
//     });
// });

