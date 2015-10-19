casper.test.comment("Loading Interface Support Page...");

var frmSupport = '#customer-support-form';

var txtCustomerName = 'input[name="customerName"]';
var txtCustomerEmail = 'input[name="customerEmail"]';
var txtSubject = 'input[name="subject"]';
var txtMessage = 'textarea[name="message"]';
var txtGameVersion = 'input[name="gameVersion"]';
var txtGameDevice = 'input[name="gameDevice"]';
var txtGameOSVersion = 'input[name="gameOSVersion"]';

var cbxPlatform = 'select[name*="platform"]';
var cbxGame = 'select[name*="gameID"]';
var cbxGameOS = 'select[name*="gameOSName"]';

var btnMessageSubmit = 'input[value*="send your message"]';

var errEmptyField = 'li.error[data-error-message*="This field is required"]';