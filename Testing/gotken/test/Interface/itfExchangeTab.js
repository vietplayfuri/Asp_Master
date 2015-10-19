casper.test.comment("Loading Interface ExchangeTab...");

var cbxExchangeSelectGame = '#s2id_exchange-select-game>a';
var cbxExchangeSelectItem = '#s2id_exchange-select-exchange-item>a';

var txtExchangeAmount = 'input[name="exchangeAmount"]';

var btnExchangeGToken = '.exchange-submit';

var msgExchangeConfirm = '.exchange-confirm-msg';

var errPositiveInteger = '.error[data-error-message*="Exchange amount needs to be a positive integer"]';
var errInsufficientBalance = '.error[data-error-message*="Insufficient Balance"]';
var errExchangeAmountRequire = '.error[data-error-message*="Exchange amount is required"]';