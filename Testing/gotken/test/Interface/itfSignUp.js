casper.test.comment("Loading Interface SignUpPage...");

var	frmSignUp = '#accounts_signup';

var txtUsername = '#username';
var txtPassword = '#password';
var txtEmail = '#email';
var txtRepeatedEmail = '#confirmEmail';
var txtReferralID = '#referralID';

var btnSignUp = '#submit';
var	btnSignOut = '.user-log-out';

var rdnAcceptTOS = '#acceptTOS';

var errInvalidEmail = 'li.error[data-error-message*="Invalid email address"]';
var errInvalidUsername = 'li.error[data-error-message*="Username can only contain"]';
var errRepeatEmailNotMatch = 'li.error[data-error-message*="Confirm email does not match"]';
var errEmptyField = 'li.error[data-error-message*="This field is required"]';
var errUsernameTaken = 'li.error[data-error-message*="This username is already taken"]';
var errEmailTaken = 'li.error[data-error-message*="This email is already taken"]';

var txtLocation = '#country-select_value';
var itemLocation = '.angucomplete-title';
var lblUserLocation = '.user-location';


