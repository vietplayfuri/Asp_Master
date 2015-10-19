/**
 * tcLanguageOptions.js - testing Lauguage Switch.
 * casperjs test TestCase/08_HomePage/tcLanguageOptions.js --xunit=Reports/Report_LanguageOptions.xml
 */
//call references
phantom.injectJs('./Global.js');
phantom.injectJs('./Interface/itfFooterHeader.js');
phantom.injectJs('./Interface/itfSignIn.js');
phantom.injectJs('./Interface/itfProfilePage.js');

var acFilterLanguageID = function(pathFile, separator) {
    var newIDs = new Array();
    var data = fs.read(pathFile);
    var content = data.replace(/^.*#~.*$/mg, "");
    var res = content.split(separator);
    casper.each(res, function(self, subString) {
        var getKeys = subString.split("\"");
        newIDs.push(getKeys[1]);
    });
    return newIDs;
}
var acCheckWorksTranslate = function(arrCheckWords) {
    var lstWordsNotTrans = 'List Of Words not translated: ';
    var bolWordsNotTrans = false;

    casper.wait(casper.intWaitingTime * 2, function() {
        casper.each(arrCheckWords, function(self, word) {
            var sPageContent = this.getPageContent();
            var sEncode = encodeURI(word);
            if (sPageContent.indexOf(word) == -1) {
                bolWordsNotTrans = true;
                lstWordsNotTrans += word + ', ';
            };
        });
    });

    casper.then(function() {
        if (bolWordsNotTrans) {
            casper.test.fail(lstWordsNotTrans);
        } else {
            casper.test.pass('All of the words on the page have been translated.');
        };
    });
}

var lstEnglishID = acFilterLanguageID(pathEnglishLanguage, 'msgid');
var arrCheckThaiWords = ['เกม', 'เกี่ยวกับ', 'ความช่วยเหลือ',
    'ไทย', 'เข้าระบบ', 'สมัครใช้งาน', 'วิธีที่ดีกว่า', 'เพื่อเพิ่มประสบการณ์การเล่นเกมของคุณให้ดีขึ้น',
    'ข้อตกลง', 'ทำไมถึงเลือก GToken ?'
];
var arrCheckMalayWords = ['Permainan', 'Tentang', 'Dukungan',
    'Masuk', 'Mendaftar', 'Syarat-syarat', 'Mengapa memilih ?'
];
var arrCheckChinaWords = ['游戏', '关于我们', '客服',
    '简体中文', '登入', '登记及得到', '条款'
];
var arrCheckWords = ['Games', 'Support', 'Terms',
    'Profile', 'Transaction', 'About', 'English', 'Friends'
];


//TC01: Lauguage switchs to Thailand completely
casper.test.begin('\n\n TC01_LanguageOptions_Switch_Thailand_Language \n', function suite(test) {
    var lstMissingID = '#   List Of Missing Message ID on the Version: ';
    var bolMissingID = false;

    casper.start().then(function() {
        var lstThailandID = acFilterLanguageID(pathThaiLanguage, 'msgid');
        this.each(lstEnglishID, function(self, ID) {
            if (!(lstThailandID.indexOf(ID) > -1)) {
                lstMissingID += '\n' + ID + ',';
                bolMissingID = true;
            };
        });
    });

    // Check Point for comparing the two files
    casper.then(function() {
        if (bolMissingID) {
            this.test.fail(lstMissingID + '\n\n');
        } else {
            this.test.pass('This Version matches English Version.');
        };
    });

    // Open the page and verify the text be translated
    casper.thenOpen(urlHomepage, function() {
        casper.waitForText(sTitleHomePage, function() {
            if (this.exists(btnSignOut)) {
                casper.acSignOut();
            };
            casper.click(lnkThaiLanguage);
        }, function() {
            casper.test.fail('Cannot not navigate to HomePage');
        });

        casper.then(function() {
            acCheckWorksTranslate(arrCheckThaiWords);
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC02: Lauguage switchs to Malaysia completely
casper.test.begin('\n\n TC02_LanguageOptions_Switch_Malaysia_Language \n', function suite(test) {
    var lstMissingID = '#   List Of Missing Message ID on the Version: ';
    var bolMissingID = false;

    casper.start().then(function() {
        var lstThailandID = acFilterLanguageID(pathMayLanguage, 'msgid');
        this.each(lstEnglishID, function(self, ID) {
            if (!(lstThailandID.indexOf(ID) > -1)) {
                lstMissingID += '\n' + ID + ',';
                bolMissingID = true;
            };
        });
    });

    casper.then(function() {
        if (bolMissingID) {
            this.test.fail(lstMissingID + '\n\n');
        } else {
            this.test.pass('This Version matches English Version.');
        };
    });

    // Open the page and verify the text be translated
    casper.thenOpen(urlHomepage, function() {
        casper.waitForText('ไทย', function() {
            casper.click(lnkMaylasiaLanguage);
        }, function() {
            casper.test.fail('Cannot not navigate to HomePage');
        });
        casper.then(function() {
            acCheckWorksTranslate(arrCheckMalayWords);
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC03: Lauguage switchs to China completely
casper.test.begin('\n\n TC03_LanguageOptions_Switch_China_Language \n', function suite(test) {
    var lstMissingID = '#   List Of Missing Message ID on the Version: ';
    var bolMissingID = false;

    // Compare China and English versions
    casper.start().then(function() {
        var lstThailandID = acFilterLanguageID(pathChinaLanguage, 'msgid');
        this.each(lstEnglishID, function(self, ID) {
            if (!(lstThailandID.indexOf(ID) > -1)) {
                lstMissingID += '\n' + ID + ',';
                bolMissingID = true;
            };
        });
    });

    casper.then(function() {
        if (bolMissingID) {
            this.test.fail(lstMissingID + '\n\n');
        } else {
            this.test.pass('This Version matches English Version.');
        };
    });

    // Open the page and verify the text be translated
    casper.thenOpen(urlHomepage, function() {
        casper.waitForText('Indonesian', function() {
            casper.click(lnkChinaLanguage);
        }, function() {
            casper.test.fail('Cannot not navigate to HomePage');
        });
        casper.then(function() {
            acCheckWorksTranslate(arrCheckChinaWords);
        });
    });

    casper.run(function() {
        test.done();
    });
});

//TC04: After login, Lauguage switchs to English completely
casper.test.begin('\n\n TC04_LanguageOptions_Switch_Language_After_Login \n', function suite(test) {
    
    casper.start(urlHomepage).waitForText('简体中文', function() {
        casper.thenClick(lnkLogin).waitForSelector(frmSignIn, function() {
            // Fill all TextBox
            var tblValues = {};
            tblValues[txtUsername] = sUsername;
            tblValues[txtPassword] = sPassword;

            casper.waitForSelector(frmSignIn, function() {
                this.fillSelectors(frmSignIn, tblValues, false);
                this.click(btnSignIn);
            }, function() {
                casper.test.fail('Cannot not load Log In Page');
            }, casper.intMaxWaitingTime);

            casper.waitWhileVisible(frmSignIn, function() {
                casper.test.pass('The Sign In with "' + sUsername + '" successfully.');
            }, function() {
                casper.test.fail('Cannot not login to GToken with account "' + sUsername + '"');
            });
        });

        casper.then(function() {
            casper.test.assertTextExists(sUsername, 'Back to the current page after switching language.');
        });
    }, function() {
        casper.test.fail('Cannot not navigate to HomePage');
    });

    casper.run(function() {
        test.done();
    });
});

//TC05: date localization
casper.test.begin('\n\n TC05_LanguageOptions_date_localization \n', function suite(test) {
    var sJoinDate;

    // Compare China and English versions
    casper.start().then(function() {
        casper.waitForText('English', function() {
            casper.click(lnkChinaLanguage);
        }, function() {
            casper.test.fail('Cannot not change to China language!');
        });
    });

    casper.then(function(){
        sJoinDate = casper.evaluate(function(selector) {
            return document.querySelectorAll(selector)[0].getAttribute('src');
        }, lblJoinDate);
        
        if (sJoinDate.indexOf('月')) {
            test.pass('The Join Date has been locailized.')
        } else {
            test.fail('Fail to localized join date!');
        };
    });

    // Back to Profile Tab
    casper.thenClick(tabTransaction).waitForSelector(dateTransaction, function() {
        var sdateTransaction = casper.evaluate(function(selector) {
            return document.querySelectorAll(selector)[0].getAttribute('src');
        }, dateTransaction);
        if (sdateTransaction.indexOf('月')) {
            test.pass('The Transaction Date has been locailized.')
        } else {
            test.fail('Fail to localized transaction date!');
        };
    }, function() {
        test.fail('Cannot back to Profile Page');
    });

    casper.run(function() {
        test.done();
    });
});

