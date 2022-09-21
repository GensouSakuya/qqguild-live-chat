using GensouSakuya.GoCqhttp.Sdk.Sessions;
using GensouSakuya.GoCqhttp.Sdk.Sessions.Models.PostEvents.Message;
using Microsoft.AspNetCore.SignalR;

namespace GensouSakuya.QQGuildLiveChat.App
{
    public class ChatHub : Hub<IChatClient>
    {
        private WebsocketSession _session;
        public ChatHub(WebsocketSession session)
        {
            _session = session;
        }

        public override Task OnConnectedAsync()
        {
            _session.GuildMessageReceived += ProcessGuildMessage;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _session.GuildMessageReceived -= ProcessGuildMessage;
            return base.OnDisconnectedAsync(exception);
        }

        private async Task ProcessGuildMessage(object sender, GuildMessage msg)
        {
            try
            {
                var guildId = msg.GuildId;
                var userId = msg.UserId;
                var icon = await _session.GetGuildMemberProfile(guildId, userId.ToString());
                this.Clients.All.SendChatMessage();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
