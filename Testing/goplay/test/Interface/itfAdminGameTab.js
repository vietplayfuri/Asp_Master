casper.test.comment("Loading Interface AdminGameTab...");

var topMenuGames = 'a[href*="/admin/game"]';

var btnAddGame = 'a[href*="/admin/game/add"]';
var btnUploadBanner = '#banner';
var btnUploadIcon = '#icon';
var btnUploadThumbnail = '#thumb';
var btnSubmitGame = '#submit';
var btnShowLocalizedShortDescription = 'a[ng-click*="show_l10n_short_description_l10n = true"]';
var btnShowLocalizedDescription = 'a[ng-click*="show_l10n_description_l10n = true"]';
var btnShowLocalizedGenre ='a[ng-click*="show_l10n_genre_l10n = true"]';
var btnShowLocalizeChangeLog = 'a[ng-click="show_l10n_current_changelog_l10n = true"]';
var btnShowLocalizeContentRating = 'a[ng-click="show_l10n_content_rating_l10n = true"]';
var btn1stEditGame = '[id*="game-row-"]:nth-child(1)>td:nth-child(8)>ul>li:nth-child(2)>a';

var frmAddGame = '#admin-games-add-form';
var frmEditGame = '#admin-game-edit-form';

var txtGameName = '#name';
var txtGameShortDescription = '#short_description_l10n-0';
var txtGameDescription = '#description_l10n-0';
var txtGameGenre = '#genre_l10n-0';
var txtGameContentRating = '#content_rating_l10n-0';
var txtGameChangeLog = '#current_changelog_l10n-0';
var txtiOSDownload = '#iosDownloadLink';
var txtAndroidDownload = '#androidDownloadLink';
var txtIndoGameShortDescription = '#short_description_l10n-1';
var txtIndoGameDescription = '#description_l10n-1';
var txtIndoGameGenre = '#genre_l10n-1';
var txtIndoGameChangeLog = '#current_changelog_l10n-1';
var txtIndoGameContentRating = '#content_rating_l10n-1';

var lblActiveStatus = '.label.success';
var lblGameID = 'tbody:first-child>tr:first-child>td:nth-child(2)';

var lstGameStudioName = 'tr[id*="game-row"]>td:nth-child(4)';

var chkGameActive = '.switch.round.small>input';

var cbxStudio = '#studio_id';

var btnDeleteGame = function(idGame){
	var cssSelector = 'a[data-reveal-id*="delete-game-'+ idGame +'-modal"]';
	return cssSelector;
};

var btnConfirmDeleteGame = function(idGame){
	var cssSelector = 'form[id*="delete-game-'+ idGame +'"]>div>input';
	return cssSelector;
};

var btnEditGame = function(idGame){
	var cssSelector = 'a[href*="/admin/game/'+ idGame +'/edit"]';
	return cssSelector;
};

var btnViewGame = function(idGame){
	var cssSelector = 'a[href*="/admin/game/' + idGame + '"]';
	return cssSelector;
};