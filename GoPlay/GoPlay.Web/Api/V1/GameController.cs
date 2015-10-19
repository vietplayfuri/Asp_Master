using GoPlay.Core;
using GoPlay.Models;
using GoPlay.Web.ActionFilter;
using GoPlay.Web.Api.Filters;
using GoPlay.Web.Models;
using Newtonsoft.Json;
using Platform.Models;
using Platform.Utility;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using GoPlay.Web.Helpers;
using System.Dynamic;
using GoPlay.Web.Helpers.Extensions;
using System.Threading.Tasks;

namespace GoPlay.WebApi.V1
{
    public class GameController : BaseApiController
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        [ActionName("save-progress")]
        [ExecutionMeter]
        public object SaveProgress()
        {
            GoPlayApi api = GoPlayApi.Instance;
            var request = HttpContext.Current.Request;
            string gameUid = request.Params["game_id"];
            string data = request.Params["data"];
            string meta = request.Params["meta"];

            string session = !string.IsNullOrEmpty(request.Form["session"])
                ? UrlHelperExtensions.GetSession(request.Form["session"])
                : request.Params["session"];

            Game game = api.GetGame(gameUid).Data;

            customer_account user = api.LoadFromAccessToken(session).Data;
            var storageFile = (request.Files != null && request.Files.Count > 0) ? request.Files[0] : null;
            game_access_token gameAccessToken = api.GetGameAccessToken(session).Data;

            string filename = string.Empty;
            ErrorCodes? error = null;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null || gameAccessToken == null)
                error = ErrorCodes.INVALID_SESSION;
            else if (string.IsNullOrEmpty(data) && (
                storageFile == null
                || storageFile.InputStream.Length == 0
                || string.IsNullOrEmpty(storageFile.FileName)))
                error = ErrorCodes.MISSING_FIELDS;
            else
            {
                gameAccessToken.data = data;
                gameAccessToken.meta = meta;
                gameAccessToken.saved_at = DateTime.UtcNow;

                string path = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["GAME_PROGRESS_UPLOADS_DIR"]);
                string rootPath = HttpContext.Current.Server.MapPath("~");
                if (storageFile != null && storageFile.InputStream.Length != 0 && !string.IsNullOrEmpty(storageFile.FileName))
                {
                    if (UrlHelperExtensions.GetFileName(storageFile.FileName, out filename))
                    {
                        filename = api.HandleFile(rootPath, storageFile.InputStream, path, filename);
                        if (string.IsNullOrEmpty(filename))
                            error = ErrorCodes.SYSTEM_ERROR;
                    }
                    else
                        error = ErrorCodes.INVALID_ATTACHED_FILE;
                }

                if (error == null)
                {
                    if (!api.UpdateGameAccessToken(gameAccessToken.id, data, meta, gameAccessToken.saved_at, filename))
                        error = ErrorCodes.ServerError;
                    else if (!string.IsNullOrEmpty(gameAccessToken.storage_file_name))
                        api.DeleteFile(rootPath + gameAccessToken.storage_file_name);
                }
            }

            api.LogApi("1", request.Url.LocalPath, error == null,
                request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id, user == null ? 0 : user.id,
                request.UserHostAddress, error.ToErrorCode(),
                request.Params.ToString());

            if (error == null)
                return Json(new { success = true, saved_at = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) });

            return Json(new
            {
                success = false,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode()
            });
        }

        [HttpPost]
        [ActionName("get-progress")]
        [ExecutionMeter]
        public object GetProgress([FromBody] APIProgressModel model)
        {
            GoPlayApi api = GoPlayApi.Instance;
            var request = HttpContext.Current.Request;
            string gameUid = model.game_id;
            Game game = api.GetGame(gameUid).Data;
            string session = !string.IsNullOrEmpty(model.session)
                ? UrlHelperExtensions.GetSession(model.session)
                : request.Params["session"];

            customer_account user = api.LoadFromAccessToken(session).Data;
            ErrorCodes? error = null;
            bool sendData = model.send_data.HasValue ? model.send_data.Value : true;
            game_access_token gameAccessToken = api.GetGameAccessToken(session).Data;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null || gameAccessToken == null)
                error = ErrorCodes.INVALID_SESSION;

            api.LogApi("1", request.Url.LocalPath, error == null,
                request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id,
                user == null ? 0 : user.id,
                request.UserHostAddress,
                error.ToErrorCode(), request.Params.ToString());

            if (error == null)
            {
                return Json(new
                {
                    success = true,
                    saved_at = Convert.ToInt64((gameAccessToken.saved_at - new DateTime(1970, 1, 1)).TotalSeconds),
                    meta = string.IsNullOrEmpty(gameAccessToken.meta) ? string.Empty : gameAccessToken.meta,
                    file = string.IsNullOrEmpty(gameAccessToken.storage_file_name) ? string.Empty : gameAccessToken.storage_file_name,
                    data = sendData ? string.IsNullOrEmpty(gameAccessToken.data) ? string.Empty : gameAccessToken.data : string.Empty,
                });
            }
            return Json(new
            {
                success = false,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
            });
        }

        [HttpPost]
        [ActionName("read-progress")]
        public object ReadProgress()
        {
            var request = HttpContext.Current.Request;
            APIProgressModel model = new APIProgressModel()
            {
                game_id = request.Params["game_id"],
                session = request.Params["session"],
                data = request.Params["data"],
                meta = request.Params["meta"]
            };

            GoPlayApi api = GoPlayApi.Instance;
            string gameUid = model.game_id;
            Game game = api.GetGame(gameUid).Data;
            //string session = model.session;
            string session = !string.IsNullOrEmpty(request.Form["session"])
                ? UrlHelperExtensions.GetSession(request.Form["session"])
                : request.Params["session"];
            customer_account user = api.LoadFromAccessToken(session).Data;
            ErrorCodes? error = null;
            bool sendData = model.send_data.HasValue ? model.send_data.Value : true;
            game_access_token gameAccessToken = api.GetGameAccessToken(model.session).Data;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null || gameAccessToken == null)
                error = ErrorCodes.INVALID_SESSION;
            else if (string.IsNullOrEmpty(gameAccessToken.storage_file_name))
                error = ErrorCodes.NON_EXISTING_FILENAME;
            else
            {
                string path = HttpContext.Current.Server.MapPath(gameAccessToken.storage_file_name);
                var file = new StreamContent(new FileStream(path, FileMode.Open));
                string filename = Path.GetFileName(path);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = file;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = filename };
                return response;
            }

            api.LogApi("1", request.Url.LocalPath, error == null,
                request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id,
                user == null ? 0 : user.id,
                request.UserHostAddress,
                error.ToErrorCode(), request.Params.ToString());

            if (error == null)
            {
                return Json(new
                {
                    success = true,
                    saved_at = Convert.ToInt64((gameAccessToken.saved_at - new DateTime(1970, 1, 1)).TotalSeconds),
                    meta = string.IsNullOrEmpty(gameAccessToken.meta) ? string.Empty : gameAccessToken.meta,
                    file = string.IsNullOrEmpty(gameAccessToken.storage_file_name) ? string.Empty : gameAccessToken.storage_file_name,
                    data = sendData ? string.IsNullOrEmpty(gameAccessToken.data) ? string.Empty : gameAccessToken.data : string.Empty,
                });
            }
            return Json(new
            {
                success = false,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
            });
        }

        [HttpPost]
        [ActionName("game-list")]
        public object GameList([FromBody] APIGetGameListModel model)
        {
            GoPlayApi api = GoPlayApi.Instance;
            string gameUid = model.game_id;
            Game game = api.GetGame(gameUid).Data;
            var request = HttpContext.Current.Request;
            string session = !string.IsNullOrEmpty(model.session)
                ? UrlHelperExtensions.GetSession(model.session)
                : request.Params["session"];
            customer_account user = api.LoadFromAccessToken(session).Data;
            ErrorCodes? error = null;
            List<Game> games = null;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null)
                error = ErrorCodes.INVALID_SESSION;
            else
                //TODO: need to apply new code 3/9
                games = api.GetInvitableGames(user.id, true, false, true).Data;

            api.LogApi("1", request.Url.LocalPath, error == null,
                request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id,
                user == null ? 0 : user.id,
                request.UserHostAddress,
                error.ToErrorCode(), request.Params.ToString());

            return new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                games = error == null ? games : null
            };
        }

        [HttpPost]
        [ActionName("update-game-stats")]
        [ExecutionMeter]
        public object UpdateGameStats([FromBody] APIUpdateGameStatusModel model)
        {
            ErrorCodes? error = null;
            GoPlayApi api = GoPlayApi.Instance;
            var request = HttpContext.Current.Request;
            string gameUid = model.game_id;
            string session = !string.IsNullOrEmpty(model.session)
                ? UrlHelperExtensions.GetSession(model.session)
                : request.Params["session"];
            Game game = api.GetGame(gameUid).Data;
            customer_account user = api.LoadFromAccessToken(session).Data;
            game_access_token gameAccessToken = api.GetGameAccessToken(session).Data;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null || gameAccessToken == null)
                error = ErrorCodes.INVALID_SESSION;
            else if (string.IsNullOrEmpty(model.stats))
                error = ErrorCodes.MISSING_FIELDS;
            else
            {
                var newStatsData = JsonHelper.DeserializeObject<List<UpdateGameStatusModel>>(model.stats);
                if (newStatsData == null || !newStatsData.Any())
                {
                    error = ErrorCodes.INVALID_GAME_STAT;
                }
                else
                {
                    for (int i = 0; i < newStatsData.Count; i++)
                    {
                        if (string.IsNullOrEmpty(newStatsData[i].title)
                            || string.IsNullOrEmpty(newStatsData[i].value)
                            || string.IsNullOrEmpty(newStatsData[i].@public))
                        {
                            error = ErrorCodes.INVALID_GAME_STAT;
                            break;
                        }
                    }
                }

                if (error == null)
                {
                    string oldStats = gameAccessToken.stats;
                    string newStats = model.stats;
                    string latestStats = model.stats;
                    if (!string.IsNullOrEmpty(oldStats))
                    {
                        var oldStatsData = JsonHelper.DeserializeObject<List<UpdateGameStatusModel>>(oldStats);

                        for (int i = 0; i < newStatsData.Count; i++)
                        {
                            bool fgIndex = true;
                            for (int j = 0; j < oldStatsData.Count; j++)
                            {
                                if (newStatsData[i].title == oldStatsData[j].title)
                                {
                                    oldStatsData[j].value = newStatsData[i].value;
                                    oldStatsData[j].@public = newStatsData[i].@public;
                                    fgIndex = false;
                                    break;
                                }
                            }
                            if (fgIndex)
                            {
                                oldStatsData.Add(newStatsData[i]);
                            }
                        }
                        latestStats = JsonConvert.SerializeObject(oldStatsData);
                    }

                    gameAccessToken.stats = latestStats;
                    gameAccessToken.saved_at = DateTime.UtcNow;
                    if (!api.UpdateGameAccessToken(gameAccessToken.id, latestStats, gameAccessToken.saved_at))
                    {
                        error = ErrorCodes.ServerError;
                    }
                }
            }

            api.LogApi("1", request.Url.LocalPath, error == null,
                request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id,
                user == null ? 0 : user.id,
                request.UserHostAddress,
                error.ToErrorCode(), request.Params.ToString());

            return new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                saved_at = error == null ? Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) : 0
            };
        }


        [HttpPost]
        [ActionName("in-app-purchase")]
        [ExecutionMeter]
        public async Task<object> InAppPurchase([FromBody] APIInAppPurchaseModel model)
        {
            var request = HttpContext.Current.Request;
            GoPlayApi api = GoPlayApi.Instance;
            string gameUid = model.game_id;
            Game game = api.GetGame(gameUid).Data;
            string session = !string.IsNullOrEmpty(model.session)
                ? UrlHelperExtensions.GetSession(model.session)
                : request.Params["session"];
            customer_account user = api.LoadFromAccessToken(session).Data;

            int quantity = model.quantity > 0 ? model.quantity : model.amount;
            string order_id = model.order_id;
            string exchangeOptionType = model.exchange_option_type;
            int exchangeOptionID = model.exchange_option_id;
            string exchangeOptionIdentifier = model.exchange_option_identifier;
            ErrorCodes? error = null;

            dynamic exchangeOption = new ExpandoObject();
            BaseExchangeHandlerInterface hander = null;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null)
                error = ErrorCodes.INVALID_SESSION;
            else if (quantity == 0
                || (string.IsNullOrEmpty(exchangeOptionIdentifier)
                    && (exchangeOptionID == 0 || string.IsNullOrEmpty(order_id))
                   ))
                error = ErrorCodes.MISSING_FIELDS;
            else
            {
                RBAC rbac = new RBAC(user.id);
                bool? isActive = true;
                if (!rbac.HasRole(GoPlayConstantValues.S_ROLE_GAME_ADMIN) && !rbac.HasRole(GoPlayConstantValues.S_ROLE_ADMIN))
                {
                    isActive = null;
                }

                if (exchangeOptionID > 0 && !string.IsNullOrEmpty(order_id))
                {
                    exchangeOption = BaseExchangeHandler.retrieveExchangeOption(exchangeOptionType, exchangeOptionID, isActive);
                }
                else if (!string.IsNullOrEmpty(exchangeOptionIdentifier))
                {
                    exchangeOption = BaseExchangeHandler.retrieveExchangeOptionByStrIdentifier(exchangeOptionIdentifier, isActive);

                }

                hander = BaseExchangeHandler.retrieveExchangeHandler(user, game, exchangeOption, quantity, IPAddress.Parse(WebApiIpHelper.GetClientIp(Request)));

                var errorDict = new Dictionary<string, ErrorCodes>()
                {
                };

                if (!hander.validate())
                {
                    var errors = hander.getErrors();
                    error = EnumEx.GetValueFromDescription<ErrorCodes>(errors[errors.Keys.First()].First());
                }
                else if (!await hander.exchange())
                {
                    var errors = hander.getErrors();
                    error = EnumEx.GetValueFromDescription<ErrorCodes>(errors[errors.Keys.First()].First());
                }
            }

            api.LogApi("1", request.Url.LocalPath, error == null,
                request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id, user == null ? 0 : user.id,
                request.UserHostAddress,
                error.ToErrorCode(), request.Params.ToString());

            if (error != null)
                return Json(new
                {
                    success = false,
                    message = error.ToErrorMessage(),
                    error_code = error.ToErrorCode()
                });

            return Json(new
            {
                success = true,
                transaction_id = new string[] { hander.getCoinTransaction().order_id },
                exchange = TransactionHelper.toDictionary(hander.getCoinTransaction())
            });
        }

        public bool ExchangeHandlerValidation(dynamic exchangeOption,
            out List<Tuple<string, string>> result,
            Game game, customer_account user)
        {
            result = new List<Tuple<string, string>>();
            if (exchangeOption == null)
            {
                result.Add(Tuple.Create<string, string>("exchange_option_id", "Exchange Option has been removed"));
                return false;
            }

            if (exchangeOption.is_archived)
            {
                result.Add(Tuple.Create<string, string>("exchange_option_id", "Exchange Option has been removed"));
            }

            if (exchangeOption.game != game.id)
            {
                result.Add(Tuple.Create<string, string>("exchange_option_id", "Exchange Option does not belong to game"));
            }

            RBAC rbac = new RBAC(CurrentUser.Id);
            if (!rbac.HasRole(GoPlayConstantValues.S_ROLE_GAME_ADMIN) && !rbac.HasRole(GoPlayConstantValues.S_ROLE_ADMIN))
            {
                if (!game.is_active || game.is_archived)
                {
                    result.Add(Tuple.Create<string, string>("exchange_option_id", "Exchange Option has been removed"));
                }
            }

            int in_game_amount = 0;
            bool isParse = Int32.TryParse(exchangeOption.inGameAmount, out in_game_amount);
            if (!isParse)
            {
                result.Add(Tuple.Create<string, string>("exchange_option_id", "Exchange amount needs to be a positive integer"));
            }

            if (in_game_amount == 0)
            {
                result.Add(Tuple.Create<string, string>("exchange_option_id", "Exchange amount is required"));
            }

            if (!result.Any())
            {
                decimal freePlayToken = 0;
                decimal playToken = 0;
                GoPlayApi.Instance.Calculate(exchangeOption, out freePlayToken, out playToken,
                    user, in_game_amount);

                if (playToken == 0 && freePlayToken == 0)
                    result.Add(Tuple.Create<string, string>("exchange_option_id", "Insufficient Balance"));
                else
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Get id, game name
        /// </summary>
        [HttpPost]
        [ActionName("get-games")]
        public object GetGames()
        {
            GoPlayApi api = GoPlayApi.Instance;
            return api.GetGamesForDropdownlist(0, new List<string> { GoPlayConstantValues.S_ROLE_ADMIN });
        }
    }
}
