casper.test.comment("Loading Interface ExchangeTab...");

var cbxExchangeSelectGame = '#s2id_exchange-select-game>a';
var cbxExchangeSelectItem = '#s2id_exchange-select-exchange-item>a';

var btnExchangeGToken = 'a[ng-click="selectExchange(exchange)"]';
var btnConfirmExchange = 'a[onclick*="Exchange Item Confirm"]';

var txtSearchGame = 'input[ng-model="searchTerm"]';

var lblExchangeConfirm = '.exchange-success';

var errPositiveInteger = '.error[data-error-message*="Exchange amount needs to be a positive integer"]';
var errInsufficientBalance = '.error[data-error-message*="Insufficient Balance"]';
var errExchangeAmountRequire = '.error[data-error-message*="Exchange amount is required"]';
var errExchange = 'span[ng-show="exchange.message"]';

var pnl_exchange = '.exchange-panel';

var lnkGame = function(game){
	var cssSelector = 'p[class*="game-name"][title*="'+ game + '"]';
	return cssSelector;
};

var lnkGamePackage = function(gamePackage){
	var cssSelector = 'li[title="'+ gamePackage + '"]>a';
	return cssSelector;
};

var txtGameCredit = function(gameCredit){
	var cssSelector = 'input[ng-model="exchange.number"][placeholder*="'+ gameCredit + '"]';
	return cssSelector;
};



