casper.test.comment("Loading Interface AdminTransactionTab...");

var itfAdminTransactionTab = (function(){
	return {
		tabGcoinIncome: 'a[href="#gcoin-income"]',
		tabGcoinOutcome: 'a[href="#gcoin-outcome"]'
	};
})();

var btnQueryTopup = 'div[id="topup"]>form>div:nth-child(2)>input[id="query"]';
var btnQueryGcoinIncome = 'div[id="gcoin-income"]>form>div:nth-child(2)>input[id="query"]';

var cbxTimeZone = '#timezone';

var frmSearchExchange = 'div[id="exchange"]>form';
var frmSearchTopUp = 'div[id="topup"]>form';
var frmSearchGCoinIncome = 'div[id="gcoin-income"]>form';
var frmSearchGCoinOutcome = 'div[id="gcoin-outcome"]>form';

var lstTransaction = 'table>tbody>tr';
var lst1stTransactionCell = '.table-wrap>table>tbody>tr:nth-child(1)>td'

var	tabTopUp = 'a[href="#topup"]';
var	tabGcoinIncome = 'a[href="#gcoin-income"]';
var	tabGcoinOutcome = 'a[href="#gcoin-outcome"]';

var txtSearchUsernameTopup = '#topup>form>div>div>#username';

var txtSearchOrderID = function(kindOfTransaction){
	var cssSelector = '#'+kindOfTransaction+'>form>div>div>#orderID';
	return cssSelector;
};

var btnQueryTransaction = function(kindOfTransaction){
	var cssSelector = 'div[id="'+ kindOfTransaction +'"]>form>div:nth-child(2)>input[id="query"]';
	return cssSelector;
};