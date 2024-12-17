using SampleChat;
using SampleGame.DB;

var builder = WebApplication.CreateBuilder(args);
UserDB.Init(builder.Configuration.GetConnectionString(nameof(UserDB)));
LogDB.Init(builder.Configuration.GetConnectionString(nameof(LogDB)));
AdminDB.Init(builder.Configuration.GetConnectionString(nameof(AdminDB)));

builder.Services.AddSignalR();

var app = builder.Build();
app.MapHub<GameHub>("/hub");
app.Run();