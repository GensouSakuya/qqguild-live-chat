using GensouSakuya.GoCqhttp.Sdk.Models.Guild;
using GensouSakuya.GoCqhttp.Sdk.Sessions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace GensouSakuya.QQGuildLiveChat.App
{
    public class QQHelper
    {
        private ConcurrentDictionary<string, MonitorChannelInfo> _connectionRoomMap = new ConcurrentDictionary<string, MonitorChannelInfo>();
        public static QQHelper Instance { get; private set; }
        private WebsocketSession _session;

        private IServiceProvider _serviceProvider;
        public QQHelper(IServiceProvider serviceProvider, WebsocketSession session)
        {
            _serviceProvider = serviceProvider;
            _session = session;
            Instance = this;
            _session.GuildMessageReceived += SendDanmu;
        }

        public async Task AttachRoomConnection(string connectionId, string guildName, string channelName)
        {
            var guilds = await _session.GetGuildList();
            var guild = guilds.FirstOrDefault(p=>p.GuildName == guildName);
            var channels = await _session.GetGuildChannelList(guild.GuildId, false);
            var channel = channels.FirstOrDefault(p=>p.ChannelName == channelName);
            _connectionRoomMap.TryAdd(connectionId, new MonitorChannelInfo
            {
                ChannelId = channel.ChannelId,
                GuildId = guild.GuildId,
            });
        }

        public void DetachRoomConnection(string connectionId)
        {
            _connectionRoomMap.TryRemove(connectionId, out _);
        }

        private static readonly Regex _scRegex = new Regex(@"\[CQ:redbag,title=(.*)\]");

        public async Task SendDanmu(object sender, GoCqhttp.Sdk.Sessions.Models.PostEvents.Message.GuildMessage msg)
        {
            try
            {
                var guildId = msg.GuildId;
                var channelId = msg.ChannelId;
                var userId = msg.UserId;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var memoryCache = scope.ServiceProvider.GetService<IMemoryCache>();
                    var profile = await GetUserProfile(memoryCache, guildId, userId.ToString());
                    var client = scope.ServiceProvider.GetService<IHubContext<ChatHub, IChatClient>>();
                    var isSc = _scRegex.IsMatch(msg.Message?.ToString() ?? "");
                    var msgSender = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(msg.Sender));
                    var senderName = msgSender?.nickname ?? profile?.NickName;
                    if (isSc)
                    {
                        var groups = _scRegex.Match(msg.Message?.ToString() ?? "").Groups;
                        var message = new SuperChatMessage
                        {
                            AvatarUrl = profile?.AvatarUrl,
                            IsOwner = false,
                            Message = groups[groups.Count -1].Value,
                            Name = senderName
                        };
                        foreach (var map in _connectionRoomMap)
                        {
                            if (map.Value.GuildId == guildId && map.Value.ChannelId == channelId)
                                await client.Clients.Client(map.Key).SendSuperChatMessage(message);
                        }
                    }
                    else
                    {
                        var message = new GuildMessage
                        {
                            AvatarUrl = profile?.AvatarUrl,
                            IsOwner = false,
                            Message = msg.Message?.ToString(),
                            Name = senderName
                        };
                        foreach (var map in _connectionRoomMap)
                        {
                            if (map.Value.GuildId == guildId && map.Value.ChannelId == channelId)
                                await client.Clients.Client(map.Key).SendChatMessage(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static MemoryCacheEntryOptions _memoryCacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        private async Task<GuildMemberInfo?> GetUserProfile(IMemoryCache memoryCache,string guildId, string userId)
        {
            if (memoryCache.TryGetValue<GuildMemberInfo>($"{guildId}:{userId}", out var profile))
            {
                return profile;
            }
            else
            {
                try
                {
                    profile = await _session.GetGuildMemberProfile(guildId, userId.ToString());
                    if (profile != null)
                    {
                        memoryCache.Set($"{guildId}:{userId}", profile, _memoryCacheEntryOptions);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("get profile failed, error:{0}", ex.ToString());
                    //try get all
                    var res = await _session.GetGuildMemberList(guildId, null);
                    if (res?.Members != null)
                    {
                        profile = res.Members.FirstOrDefault(p => p.TinyId == userId);
                        if (profile != null)
                        {
                            memoryCache.Set($"{guildId}:{userId}", profile, _memoryCacheEntryOptions);
                        }
                    }
                }
                return profile;
            }
        }
    }

    public class MonitorChannelInfo
    {
        public string ChannelId { get; set; }
        public string GuildId { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is MonitorChannelInfo info &&
                   ChannelId == info.ChannelId &&
                   GuildId == info.GuildId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ChannelId, GuildId);
        }
    }
}
