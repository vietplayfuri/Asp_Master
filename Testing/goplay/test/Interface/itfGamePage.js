casper.test.comment("Loading Interface GamePage...");

var divGameBrief = '.game-brief';

var imgGameThump = '.game-thumb';

var lnkGameDetail = function(GameName){
	var cssSelector = 'a[class="g-title"][href*="' + GameName + '"]';
	return cssSelector;
};

var lblGameTitle = '.g-title';
var lblGameName = '.g-title>h4';
var lblGameGerne = '.g-genre>p';
var lblGameDetailTitle = '.game-title-intro>h2';

var icoiOSDowload = '.ios-plat';
var icoAndroidDowload = '.gplay-plat';

var divGame = function(idGame){
	var cssSelector = '.g-title[href*="-' + idGame + '"]';
	return cssSelector;
};