using SampleCron;
using SampleGame.DB;

var builder = Host.CreateApplicationBuilder(args);
UserDB.Init(builder.Configuration.GetConnectionString(nameof(UserDB)));
LogDB.Init(builder.Configuration.GetConnectionString(nameof(LogDB)));
AdminDB.Init(builder.Configuration.GetConnectionString(nameof(AdminDB)));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();