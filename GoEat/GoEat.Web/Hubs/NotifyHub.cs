using System.Linq;
using System.Threading.Tasks;
using GoEat.Web.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using GoEat.Web.Models;

namespace GoEat.Web.Hubs
{
    [Authorize]
    [HubName("NotifyHub")]
    public class NotifyHub : Hub
    {
        public readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public void SendMessage(string who, string message, string connectionId)
        {
            string name = Context.User.Identity.GetUserId<int>().ToString();

   
                Clients.Client(connectionId).addChatMessage(name + ": " + message);
        }

        public void ShowTransactionConfirmation(string connectionId, TransactionInformation shownTransaction)
        {
            string name = Context.User.Identity.GetUserId<int>().ToString();
            Clients.Client(connectionId).showTransactionConfirmation(shownTransaction);
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.GetUserId<int>().ToString();

            _connections.Add(name, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.GetUserId<int>().ToString();

            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.GetUserId<int>().ToString();

            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
    }
}