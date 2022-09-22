using Microsoft.AspNetCore.SignalR;

namespace GensouSakuya.QQGuildLiveChat.App
{
    public class ChatHub : Hub<IChatClient>
    {
        private QQHelper _qqHelper;
        public ChatHub(QQHelper qQHelper)
        {
            _qqHelper = qQHelper;
        }

        public override async Task OnConnectedAsync()
        {
            var param = this.Context.GetHttpContext().Request.Query;
            var roomname = param["room"];
            var guildName = param["guildName"];
            var connectionId = this.Context.ConnectionId;
            await _qqHelper.AttachRoomConnection(connectionId, guildName, roomname);
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = this.Context.ConnectionId;
            _qqHelper.DetachRoomConnection(connectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
