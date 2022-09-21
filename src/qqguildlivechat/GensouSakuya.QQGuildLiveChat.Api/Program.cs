using GensouSakuya.GoCqhttp.Sdk.Sessions;
using GensouSakuya.QQGuildLiveChat.App;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<WebsocketSession>(p =>
{
    var config = p.GetService<IConfiguration>();
    var botconf = config.GetSection("bot");
    var host = botconf.GetValue<string>("host");
    var port = botconf.GetValue<int>("port");
    var session = new WebsocketSession(host, port, null);
    return session;
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();
app.MapHub<ChatHub>("/chat");

app.MapControllers();

await app.Services.CreateScope().ServiceProvider.GetService<WebsocketSession>().ConnectAsync();

app.Run();
