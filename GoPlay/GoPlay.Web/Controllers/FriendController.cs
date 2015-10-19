using GoPlay.Core;
using GoPlay.Models;
using GoPlay.Web.ActionFilter;
using GoPlay.Web.Helpers;
using Newtonsoft.Json;
using Platform.Models;
using Platform.Models.Models;
using Platform.Utility.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GoPlay.Web.Controllers
{
    [RequiredLogin]
    public class FriendController : BaseController
    {
        public ActionResult Index()
        {
            var api = GoPlayApi.Instance;
            var pendings = api.GetSimpleFriendList(CurrentUser.Id, FriendStatus.Pending);
            var accepteds = api.GetSimpleFriendList(CurrentUser.Id, FriendStatus.Accepted);

            var intConfig = ConfigurationManager.AppSettings["MAXIMUM_NUMBER_SHOWN_IN_FRIEND_INDEX"];
            int intMaximum = 0;
            if (!Int32.TryParse(intConfig, out intMaximum))
            {
                intMaximum = Int32.MaxValue;
            }
            //TODO: need to apply new code 3/9
            FriendViewModel model = new FriendViewModel
            {
                pending_friends = pendings != null ? pendings.Skip(0).Take(intMaximum).ToList() : null,
                count = pendings != null ? pendings.Count() : 0,
                accepted_friends = accepteds
            };

            return View(model);
        }

        [Route("friend/search")]
        public ActionResult Search()
        {
            var param = HttpContext.Request.Params;
            var page = (int)((param["page"] == null || Convert.ToInt32(param["page"]) < 1)
                ? 1
                : Convert.ToInt32(param["page"]));
            var term = param["term"] == null
                ? string.Empty
                : param["term"].ToLower().Trim('"');

            if (term.Length == 0)
                return RedirectToAction("Index");

            string pageSizeConfig = ConfigurationManager.AppSettings["PAGE_SIZE_IN_FIND_FRIEND"];
            int pageSize = 0;
            Int32.TryParse(pageSizeConfig, out pageSize);
            pageSize = pageSize > 0 ? pageSize : int.MaxValue;

            var offset = pageSize == int.MaxValue
                ? 0
                : pageSize * (page - 1);

            int totalCount = 0;
            var results = GoPlayApi.Instance.FindFriends(out totalCount, CurrentUser.Id, term, offset, pageSize);
            var model = new SearchFriendViewModel
            {
                friends = results,
                pagination = new Pagination(page, pageSize, totalCount),
                term = term,
                total_count = totalCount
            };
            return View(model);
        }

        ////TODO: Add method search-friend
        //@friendBlueprint.route('/search-friend', methods=['POST'])
        //@login_required
        //@csrf.exempt
        //def findFriends():
        //    data = request.json
        //    term = data.get('term').lower().strip()
        //    keyword = urllib.unquote(term.encode('utf-8')).decode('utf-8')
        //    results = CustomerAccount.findFriends(current_user.id, keyword)
        //    users = results['users']
        //    friends = [result.toPublicDictionary() for result in results['users']]
        //    return json.dumps(friends)

        [HttpPost]
        [Route("friend/quick-search")]
        public JsonResult QuickFindUserAccount(string term)
        {
            term = string.IsNullOrEmpty(term)
                ? string.Empty
                : term.ToLower().Trim('"');
            List<object> result = null;
            int totalCount = 0;

            if (term.Length != 0)
            {
                string offsetConfig = ConfigurationManager.AppSettings["DEFAULT_OFFSET_IN_FIND_FRIEND"];
                string pageSizeConfig = ConfigurationManager.AppSettings["DEFAULT_PAGE_SIZE_IN_QUICK_FIND_FRIEND"];
                int offset = 0;
                int pageSize = 0;
                Int32.TryParse(offsetConfig, out offset);
                Int32.TryParse(pageSizeConfig, out pageSize);
                pageSize = pageSize > 0 ? pageSize : 6;

                var results = GoPlayApi.Instance.FindFriends(out totalCount, CurrentUser.Id, term, offset, pageSize);

                result = new List<object>();
                results.ForEach(i => result.Add(i.ToPublicDictationary()));
            }

            return Json(new
            {
                success = term.Length != 0,
                count = totalCount,
                users = result
            });
        }

        [HttpPost]
        [Route("friend/add")]
        public async Task<ActionResult> AddFriend(string friend)
        {
            var param = HttpContext.Request.Params;
            var friendName = string.IsNullOrEmpty(friend)
                ? string.Empty
                : friend.ToLower().Trim('"');

            var api = GoPlayApi.Instance;
            var userFriend = api.GetUserByUserName(friend).Data;

            if (userFriend == null)
                return Json(new { success = false });

            //TODO: add friend to chat system of SignalR later,need to apply new code 3/9

            string gtokenSession = HttpContext.Session["gtoken_section"] == null
                ? string.Empty
                : HttpContext.Session["gtoken_section"].ToString();

            bool isSendRequest = api.SendRequest(gtokenSession, CurrentUser.Id, userFriend.id, null);
            if (isSendRequest)
            {
                string rootUrl = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, String.Empty);
                var template = new FriendRequestEmail
                {
                    receiver_display_name = userFriend.GetDisplayName(),
                    to_email = userFriend.email,
                    sender_display_name = CurrentUser.GetDisplayName(),
                    sender_avatar = CurrentUser.avatar_filename,
                    sender_username = CurrentUser.UserName
                };
                await EmailHelper.SendMailFriendRequest(template);
            }

            return Json(new
            {
                success = string.IsNullOrEmpty(gtokenSession)
                 ? false
                 : isSendRequest
            });
        }

        [HttpPost]
        [Route("friend/accept")]
        public ActionResult AcceptFriend(string friend)
        {
            var param = HttpContext.Request.Params;
            var friendName = string.IsNullOrEmpty(friend)
                ? string.Empty
                : friend.ToLower().Trim('"');

            var api = GoPlayApi.Instance;

            var userFriend = api.GetUserByUserName(friend).Data;
            if (userFriend == null)
                return Json(new { success = false });

            string gtokenSession = HttpContext.Session["gtoken_section"] == null
                ? string.Empty
                : HttpContext.Session["gtoken_section"].ToString();

            bool isSuccess = api.RespondRequest(gtokenSession, CurrentUser.Id, userFriend.id, ConstantValues.S_ACCEPTED).HasData;

            //TODO: add friend to chat system of SignalR later,need to apply new code 3/9

            return Json(new
            {
                success = string.IsNullOrEmpty(gtokenSession)
                 ? false
                 : isSuccess
            });
        }

        [HttpPost]
        [Route("friend/remove-request")]
        public ActionResult RemoveRequestFriend(string friend)
        {
            var param = HttpContext.Request.Params;
            var friendName = string.IsNullOrEmpty(friend)
                ? string.Empty
                : friend.ToLower().Trim('"');

            var api = GoPlayApi.Instance;

            var userFriend = api.GetUserByUserName(friend).Data;
            if (userFriend == null)
                return Json(new { success = false });

            string gtokenSession = HttpContext.Session["gtoken_section"] == null
                ? string.Empty
                : HttpContext.Session["gtoken_section"].ToString();

            bool isSuccess = api.RespondRequest(gtokenSession, CurrentUser.Id, userFriend.id, ConstantValues.S_REJECTED).HasData;

            return Json(new
            {
                success = string.IsNullOrEmpty(gtokenSession)
                 ? false
                 : isSuccess
            });
        }

        [HttpPost]
        [Route("friend/unfriend")]
        public ActionResult UnFriend(string friend)
        {
            var param = HttpContext.Request.Params;
            var friendName = string.IsNullOrEmpty(friend)
                ? string.Empty
                : friend.ToLower().Trim('"');

            var api = GoPlayApi.Instance;

            var userFriend = api.GetUserByUserName(friend).Data;
            if (userFriend == null)
                return Json(new { success = false });

            string gtokenSession = HttpContext.Session["gtoken_section"] == null
                ? string.Empty
                : HttpContext.Session["gtoken_section"].ToString();

            bool isSuccess = api.RespondRequest(gtokenSession, CurrentUser.Id, userFriend.id, ConstantValues.S_REJECTED).HasData;

            //TODO: remove friend in system chat using SignalR, need to apply new code 3/9            

            return Json(new
            {
                success = string.IsNullOrEmpty(gtokenSession)
                 ? false
                 : isSuccess
            });
        }

        [HttpPost]
        [Route("friend/friendlist")]
        public object GetFriendlist()
        {
            var api = GoPlayApi.Instance;
            var accepteds = api.GetFriendList(CurrentUser.Id, FriendStatus.Accepted);
            Dictionary<string, object> friends = new Dictionary<string, object>();
            foreach (var friend in accepteds)
            {
                friends.Add(friend.username, friend.ToPublicDictationary());
            }

            return JsonConvert.SerializeObject(friends);
        }

        [HttpPost]
        [Route("friend/transfer")]
        public ActionResult Transfer(TransferModel model)
        {
            var param = HttpContext.Request.Params;
            var api = GoPlayApi.Instance;
            model.senderId = CurrentUser.Id;
            model.maxAmount = CurrentUser.play_token;
            customer_account receiver = null;
            var errors = model.IsValid(out receiver);
            string description = string.Empty;

            if (!ModelState.IsValid || (errors != null && errors.Any()))
            {
                if (errors != null && errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Item1, error.Item2);
                    }
                }
                return Json(new { errors = Errors(ModelState) });
            }
            else if (model.forValidate)
            {
                return Json(new { correct = true });
            }
            else
            {
                //# Save 2 coin_transactions (positive and negative) to Gtoken using record API
                //# Then, we will save them to GoPlay DB later

                #region Sender transaction, negative amount
                description = string.Format(GoPlayConstantValues.S_TRANSFER_TOKEN_DESCRIPTION,
                     model.playTokenAmount,
                     receiver.GetDisplayName());
                coin_transaction sendingTransaction = new coin_transaction(CurrentUser.Id, receiver.id, description);
                sendingTransaction.amount = model.playTokenAmount * -1;
                sendingTransaction.use_gtoken = true;

                TokenTransactionJson transactionJSON = new TokenTransactionJson(CurrentUser.UserName, sendingTransaction.order_id, sendingTransaction.description, sendingTransaction.amount.Value, GoPlayConstantValues.S_PLAY_TOKEN);
                var isRecordGtoken = api.GTokenAPITransaction(new GtokenModelTransactionAction
                {
                    enumAction = EGtokenAction.RecordTransaction,
                    token_transaction = transactionJSON
                });

                if (!isRecordGtoken.Succeeded)
                {
                    ModelState.AddModelError(GoPlayConstantValues.S_RECEIVER_ID, isRecordGtoken.Error.Value.ToErrorMessage());
                    return Json(new { errors = Errors(ModelState) });
                }
                #endregion

                #region Receiver transaction, positive amount
                description = string.Format(GoPlayConstantValues.S_RECEIVE_TOKEN_DESCRIPTION,
                     model.playTokenAmount,
                     CurrentUser.GetDisplayName());
                coin_transaction receivingTransaction = new coin_transaction(receiver.id, CurrentUser.Id, description);
                receivingTransaction.amount = model.playTokenAmount;

                transactionJSON = new TokenTransactionJson(receiver.username, receivingTransaction.order_id, receivingTransaction.description, receivingTransaction.amount.Value, GoPlayConstantValues.S_PLAY_TOKEN);
                isRecordGtoken = api.GTokenAPITransaction(new GtokenModelTransactionAction
                {
                    enumAction = EGtokenAction.RecordTransaction,
                    token_transaction = transactionJSON
                });

                if (!isRecordGtoken.Succeeded)
                {
                    ModelState.AddModelError(GoPlayConstantValues.S_RECEIVER_ID, isRecordGtoken.Error.Value.ToErrorMessage());
                    return Json(new { errors = Errors(ModelState) });
                }
                #endregion

                //Save to DB this coin_transaction if all Gtoken work well
                api.CreateTransferCoinTransaction(sendingTransaction);
                api.CreateTransferCoinTransaction(receivingTransaction);
            }

            description = string.Format(GoPlayConstantValues.S_TRANSFER_TOKEN_DESCRIPTION,
                model.playTokenAmount,
                receiver.GetDisplayName());

            return Json(new { correct = true, message = description, amount = -model.playTokenAmount });
        }

    }
}