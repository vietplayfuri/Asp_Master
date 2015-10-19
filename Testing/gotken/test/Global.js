casper.test.comment("Loading Global Variables...");

//call references
phantom.injectJs('./Configuration.js');
phantom.injectJs('./Action/acCommon.js');

// Variable of URL
var urlHomepage = 'http://localhost:9000/';
// var urlHomepage = 'http://staging.gtoken.com/';
var urlRegister = urlHomepage + '/account/register';
var urlSignIn = urlHomepage + '/account/login';
var urlTransaction = urlHomepage + '/transaction';
var urlProfile = urlHomepage + '/account/profile';
var url500Page = urlHomepage + '/500';
var urlGamePage = urlHomepage + '/game';
var urlAdminPage = urlHomepage + '/admin';
var urlSupportPage = urlHomepage + '/support';
var urlAdminGamePage = urlHomepage + '/admin/game';
var urlAdminTransactionPage = urlHomepage + '/admin/transaction';
var urlAdminTransactionIncomePage = urlHomepage + '/admin/transaction#gcoin-income';
var urlAdminTransactionOutcomePage = urlHomepage + '/admin/transaction#gcoin-outcome';
var urlAdminStudioPage = urlHomepage + '/admin/studio';
var urlAdminUserPage = urlHomepage + '/admin/user';
var urlAdminExchangePage = urlHomepage + '/admin/exchange/';
var urlClubPage = urlHomepage + '/club';
var urlTopUp = urlHomepage + '/transaction/topup/';

// call Module of Casperjs
var x = require('casper').selectXPath;
var fs = require('fs');
var utils = require('utils');

// Variable of path 
var path_gcoin_income = "TestData/gcoin_income.txt"
var path_gcoin_outcome = "TestData/gcoin_outcome.txt"

var pathIconImage = './TestData/game-icon.png';
var pathThumbImage = './TestData/game-thumbnail.png';
var pathCoverImage = './TestData/game-cover.png';

// Variable for Timeout
casper.intWaitingTime = 1 * 1000;
casper.intMaxWaitingTime = 10 * 1000;
casper.options.waitTimeout = 5000;

// UPoint
var sValidUPointPhone = '085222093438';

// Global Varibles
var sAdminUsername = 'foxyadmin';
var sAdminPassword = '123';
var sAdminEmail = 'phuong.gtoken+admin@gmail.com';

var sExchangeUsername = 'gtokentester';
var sExchangePassword = '123456';

var sReferralUsername = 'foxyreferral4';
var sReferralPassword = '123';

var sUsername = 'foxymax1';
var sPassword = '123';
var sEmail = 'phuong.gtoken@gmail.com';
var sReferralID = sReferralUsername;
var bAcceptTOS = true;

var sChatUsername = 'foxychat';
var sChatPassword = '123';
var sChatEmail = 'phuong.gtoken+chat@gmail.com';
var sChatReferralID = sUsername;

var sEditEmailUsername = 'foxyeditemail';
var sEditEmailPassword = '123';
var sEditEmailEmail = 'phuong.gtoken+editemail@gmail.com';

var sGameAdminUsername = 'foxygameadmin';
var sGameAdminPassword = '123';
var sGameAdminEmail = 'phuong.gtoken+gameadmin@gmail.com';
var idGameAdmin = 20184;

var sGameAccountantUsername = 'foxygameaccountant';
var sGameAccountantPassword = '123';
var sGameAccountantEmail = 'phuong.gtoken+gameaccountant@gmail.com';

var sAccountantUsername = 'foxyaccountant';
var sAccountantPassword = '123';
var sAccountantEmail = 'phuong.gtoken+accountant@gmail.com';

var sCustomerSupportUsername = 'foxycustomersupport';
var sCustomerSupportPassword = '123';
var sCustomerSupportEmail = 'phuong.gtoken+customersupport@gmail.com';

var sReceiverUsername = 'gtokenreceiver';
var sReceiverPassword = '123';

var sChangePasswordUsername = 'foxychangepassword';
var sNewPassword1 = 'Banana';
var sNewPassword2 = 'BlueBerry';

// Paypal Account 
var sUsernamePaypal = 'khang+paypalbuyer@gtoken.com';
var sPasswordPaypal = 'secretsecret';

var sSendingSupportGameEmail = 'phuong.gtoken+supportgame@gmail.com';
var sSendingSupportWebsiteEmail = 'phuong.gtoken+supportwebsite@gmail.com';

var TimeNow = new Date();
var sUniqueValue = TimeNow.getHours().toString() + TimeNow.getMinutes().toString() + TimeNow.getSeconds().toString();

var sUsernameUnique = sUsername + '_' + sUniqueValue;
var sPasswordUnique = '123' + sUniqueValue;
var sEmailUnique = 'phuong.gtoken+' + sUniqueValue + '@gmail.com';

var pathEnglishLanguage = '../gtoken/translations/en/LC_MESSAGES/messages.po';
var pathThaiLanguage = '../gtoken/translations/th/LC_MESSAGES/messages.po';
var pathMayLanguage = '../gtoken/translations/ms/LC_MESSAGES/messages.po';
var pathChinaLanguage = '../gtoken/translations/zh/LC_MESSAGES/messages.po';
var pathTestDataFolder = './TestData/';

// Title of Page
var sTitleSignInPage = 'Login and having fun!';
var sTitleSignUpPage = 'get a free';
var sTitleHomePage = 'A better way';
var sTitleProfilePage = 'Things you might want to do:';
var sTitleInvoice = 'Thank you for your business. This is an electronic receipt.';
var sTitleGamePage = 'Well-supported with GToken for better gaming experiences';
var sTitleAdminPage = 'GToken Admin panel';
var sTitleExchangeTab = 'select a game';
var sTitleTopUpTab = 'Top Up GToken';
var sTitleTransactionTab = 'Your Balance';
var sTitleTransactionHistoryTab = 'Transactions History';
var sTitleChangePasswordPage = 'Change your password';
var sTitleSupportPage = 'What\'s On Your Mind';
var sTitleChangeEmailPage = 'Change your email';
var sTitleAdminStudioTab = 'Add Studio';
var sTitleAdminGameTab = 'Add Game';
var sTitleAdminUserTab = 'Last login at';
var sTitleAdminTransactionTab = 'Free Transaction';
var sTitleGCoinTab = 'Convert G-Coin';

// Message from System
var msgExchangeSuccessfully = 'Exchanged successfully';
var msgLogInSuccessfully = 'Logged in successfully';
var msgSendSupport = 'We will contact you soon';
var msgSignUpSuccessfully = 'Your account is successfully created';
var msgSignOutSuccessfully = 'You have been logged out';
var msgCancelPaypal = 'Your PayPal payment has been cancelled';
var msgChangePassword = 'Changed password successfully';
var msgChangeEmail = 'Changed email successfully';
var msgEditProfile = 'You\'ve updated your profile';
var msgAssignGameAdmin = 'Successfully assigned game admin';
var msgUnassignGameAdmin = 'Unassigned game admin';
var msgConvertGCoin = 'Transaction is successful';
var msgCreateCredit = 'Successfully added credit type';

