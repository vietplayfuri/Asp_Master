casper.test.comment("Loading Interface ProFilePage...");

var	btnSignOut = '.user-log-out';
var	btnEditProfile = '.edit-prf-btn';
var	btnExchange = '.exchange-td';
var	btnTopUp = '.topup-td';
var	btnTransfer = '.transfer-td';
var	btnEditEmail = '#edit-email-button';
var	btnChangePassword = '#edit-password-button';
var btnRecruitPackage = '#new-recruit-package';
var btnSkilledPackage = '#skilled-one-package';
var btnKingPackage = '#king-club-package';
var btnTransferGtoken = '#transfer-submit';
var btnTopUpAmountGToken = '#submit-custom-gtoken-form';
var btnSaveProfile = 'form[name="editProfileForm"]>input[value="save"]';
var btnShowReferralLink = '#show-referral-link';
var btnHideReferralLink = '#hide-referral-link';

var lnkProfilePage = 'a[href="/account/profile"]';
var lnkGCoinTab = '#show-gcoin-panel-link';

var tabTransaction = '.acc-nav-links>li>a[href*="/transaction/"]';
var tabFriends = '.acc-nav-links>li>a[href*="/friend/"]';
var tabProfile = '.acc-nav-links>li>a[href*="/account/profile"]';
var tabPaypal = 'a[ng-click*="paypal"]';

var lblAmoutGtoken = '#user-gtoken';
var lblAmoutFreeGtoken = '#user-free-gtoken';
var lblAmoutTotalGtoken = '#user-total-gtoken';
var lblReceiverName = 'div[class="receiver-name"]>span';
var lblUserLocation = '.user-location';
var lblUserBioBox = '.user-bio';
var lblNickName = '.account-name>a';
var lblUserEmail = '.has-tip.tip-top.radius';
var lblReferee = '.referee-ava>a';
var lblReferralLink = '.referral-code';
var lblJoinDate = '.info-list>li:nth-child(2)>span';

var txtReceiverSearch = '#receiver-input_value';
var txtGtokenTransfer = '#gtoken-amount-input';
var txtGTokenPaypal = 'input[ng-model="customGTokenQuantity"]';
var txtUserLocation = '#country-select_value';
var txtUserBioBox = 'textarea[ng-model="profile.bio"]';
var txtUserNickName = 'input[ng-model="profile.nickname"]';
var txtProfileReferralID = 'input[name="referralID"]';

var itmSearchReceiver = 'div[class*="angucomplete-title"]';

var errOverTwoDecimals = '.error[data-error-message="You can only transfer up to 2 decimal points."]';
var errDenominationsOf10 = 'li[ng-if="customGTokenQuantity % 10 != 0"]';
var err10To4000 = 'li[ng-if*="customGTokenForm.quantity.$error.min"]';
var errReferralNotExist = 'li[data-error-message*="Referral Code does not exist"]';
var errInsufficientAmount = 'li[data-error-message*="Your GToken balance is insufficient for the transaction"]';

var imgTransaction = '.trans-cell.type>img';
var descTransaction = 'td[class="trans-cell desc"]';
var amountTransaction = 'td[class*="trans-cell amnt"]';
var invoiceTransaction = '.trans-cell.invc>a';
var dateTransaction = 'td[class="trans-cell time"]';

var itemUserLocation = '.angucomplete-title.ng-binding.ng-scope';

