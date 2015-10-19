casper.test.comment("Loading Interface UPointTab...");

var btnBalanceSubmit = 'input[ng-click="submitBalanceDeductionForm()"]';
var btnTMoneySubmit = 'input[ng-click="submitTMoneyForm()"]';
var btnUPointSkilledPackage = '#upoint-skilled-one-package';
var btnBalanceMethod = 'a[ng-click="upointMethod = \'Balance Deduction\'"]';
var btnTMoneyMethod = 'a[ng-click="upointMethod = \'T-Money\'"]';

var errInvalidPhone = '.error[data-error-message*="Invalid parameter: phone_number"]';
var errInvalidBalanceAmount = '.error[data-error-message*="number"]';

var lblBalanceSMS = '[ng-bind="balanceDeductionSMS"]';

var tabUPoint = '[ng-click*="topUpMethod=\'upoint\'"]';

var txtBalancePhone = 'input[ng-model="balanceDeductionData.phoneNumber"]';
var txtTMoneyPhone = 'input[ng-model="tmoneyData.phoneNumber"]';


