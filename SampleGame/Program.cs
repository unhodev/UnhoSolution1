using SampleGame.DB;


var builder = WebApplication.CreateBuilder(args);
UserDB.Init(builder.Configuration.GetConnectionString(nameof(UserDB)));
LogDB.Init(builder.Configuration.GetConnectionString(nameof(LogDB)));
AdminDB.Init(builder.Configuration.GetConnectionString(nameof(AdminDB)));

builder.Services.AddControllers();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller}/{action}");
app.Run();