using GensouSakuya.GoCqhttp.Sdk.Sessions;
using GensouSakuya.QQGuildLiveChat.App;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<QQHelper>();
builder.Services.AddSingleton<WebsocketSession>(p =>
{
    var config = p.GetService<IConfiguration>();
    var botconf = config.GetSection("bot");
    var host = botconf.GetValue<string>("host");
    var port = botconf.GetValue<int>("port");
    var session = new WebsocketSession(host, port, null);
    return session;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();
app.MapHub<ChatHub>("/chat");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "web")),
});
app.UseCors();

await app.Services.CreateScope().ServiceProvider.GetService<WebsocketSession>().ConnectAsync();

var host = app.Services.GetService<IConfiguration>().GetValue<string>("urls");
Console.WriteLine($"µØƒª…Ë÷√“≥√Ê:{host}/index.html");

app.Run();

