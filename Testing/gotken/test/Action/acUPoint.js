casper.test.comment("Loading UPoint Actions...");

// Actions
var acOpenUPoint = function(paymentMethodSelector) {
    var arrAmoutGtoken = new Array();

    // Select one package on transaction page
    casper.then(function() {
        casper.click(tabTransaction);
        casper.waitForText(sTitleTransactionTab, function() {
            // Get Amount of each kind of Gtoken
            arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutGtoken)));
            arrAmoutGtoken.push(parseFloat(this.fetchText(lblAmoutFreeGtoken)));

            // Navigate to Paypal Tab
            this.click(btnTopUp);
            this.click(tabUPoint);
            this.thenClick(btnUPointSkilledPackage, function(){
                casper.click(paymentMethodSelector);
            });
        }, function() {
            casper.test.fail('Fail to open TopUp page');
        }, casper.intWaitingTime * 5);
    });
    return arrAmoutGtoken;
};
