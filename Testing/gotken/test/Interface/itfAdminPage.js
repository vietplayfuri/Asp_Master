casper.test.comment("Loading Interface AdminPage...");

var txtSearchUsername = '#username';

var btnQuery = '#query';

var itfAdminPage = (function(){
	return {
		btnQuery: '#query',
		cbxGame: '#source',
		chartActiveUser: 'svg[class="pygal-chart"]',
		chkDaily: '#isDAU',
		frmDailyReport: 'form[class="model-form"]',
		lblResultRow: '.table-wrap>table>tbody>tr',
		lnkAdminReport: 'a[href*="/admin/report"]',
		lnkGamesMenu: 'a[href*="/admin/game"]',
		lnkStudiosMenu: 'a[href*="/admin/studio"]',
		lnkExchangeOptionsMenu: 'a[href*="/admin/exchange"]',
		lnkTransactionsMenu: 'a[href*="/admin/transaction"]',
		lnkTopUpCardsMenu: 'a[href*="/admin/topup-card"]',
		lnkUsersMenu: 'a[href*="/admin/user"]',
		lnkPaypalMenu: 'a[href*="/admin/paypal"]',
		txtFromTime:'#fromTime',
		txtToTime: '#toTime',
	};
})();

var itfAdminExchangeTab = (function(){

	var pblbtnEditCredit = function(idExchangeOption){
		var cssSelector = 'a[href*="/admin/exchange/creditType/'+idExchangeOption+'/edit"]';
		return cssSelector;
	};

	return {
		btnAddCredit: 'a[href="/admin/exchange/creditType/add"]',
		btnAddPackge: 'a[href="/admin/exchange/package/add"]',
		txtStringIdentifier : '#string_identifier',
		btnEditCredit : pblbtnEditCredit,
		txtName : '#name',
		txtExchangeRate : '#exchange_rate',
		txtFreeExchangeRate : '#free_exchange_rate',
		chkActive : '#is_active',
		chkArchive : '#is_archived',
		btnUploadIcon : '#icon',
		frmAdminExchange :'#admin-exchange-entity-edit-form',
		btnSubmit : '#submit',
		lblExchangeOptionID : '#exchange-entity-detail-id'
	};
})(); 

var itfAdminPaypalTab = (function(){
	return {
		btnSearchPayment: 'a[href="/admin/search-paypal-payment/"]',
		btnSubmit: '#submit',
		txtTransactionID: '#transaction_id',
		tblPaymentDetail: '#search-form>.row>table'
	};
})(); 
