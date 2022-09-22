namespace GensouSakuya.QQGuildLiveChat.App
{
    public interface IChatClient
    {
        Task SendChatMessage(GuildMessage msg);
    }
}
