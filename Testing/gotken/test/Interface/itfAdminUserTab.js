casper.test.comment("Loading Interface AdminUserTab...");

var lnkUserID = function(idUser){
	var cssSelector = 'a[href="/admin/user/'+ idUser +'/"]';
	return cssSelector;
};

var lblRevealedPassword = '.label.alert.radius';