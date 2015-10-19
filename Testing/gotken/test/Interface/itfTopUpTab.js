casper.test.comment("Loading Interface TopUpTab...");

var tabTopUp = 'a[ng-click*="topUpMethod=\'topUpCard\'"]';
var txtTopUpCardNumber = 'input[ng-model="cardNumber"]';
var txtTopupCardPassword = 'input[ng-model="cardPassword"]';
var btnTopUpCard = 'input[ng-click="submitTopupCardForm()"]';

var errTopUpCardExpired = '.error[data-error-message*="The card has already expired"]';
var errTopUpCardUsed = '.error[data-error-message*="The card has already been used"]';
