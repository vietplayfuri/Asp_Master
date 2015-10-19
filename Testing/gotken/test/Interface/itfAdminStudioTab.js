casper.test.comment("Loading Interface AdminStuidoTab...");

var lblStudioName = function(idStudio){
	var cssSelector = '#studio-row-'+idStudio+'>td:nth-child(2)';
	return cssSelector;
};

var btnViewStudio = function(idStudio){
	var cssSelector = 'a[href="/admin/studio/'+ idStudio +'/"]';
	return cssSelector;
};
var btnEditStudio = function(idStudio){
	var cssSelector = 'a[href*="/admin/studio/'+ idStudio +'/edit"]';
	return cssSelector;
};
var btnUnassignGameAdmin = function(idGameAdmin){
	var cssSelector = 'form[id*="unassign-game-admin-'+ idGameAdmin +'"]>[value*="ssign"]';
	// var cssSelector = 'form[id*="'+ idGameAdmin +'"]>input[value="Unassign"]';
	return cssSelector;
};
var btnAssignGameAdmin = function(idGameAdmin){
	var cssSelector = 'form[id*="assign-game-admin-'+ idGameAdmin +'"]>[value*="ssign"]';
	// var cssSelector = 'form[id*="'+ idGameAdmin +'"]>input[value="Assign"]';
	return cssSelector;
};

var btnAssignAdmin = function(idStudio){
	// var cssSelector = 'a[href*="/admin/studio/'+idStudio+'/assign-game-admin"]';
	var xpath = x('//a[contains(@href,"/admin/studio/'+idStudio+'/assign-game-admin")][1]');
	return xpath;
};

var lstStudio = 'tr[id*="studio"]';

