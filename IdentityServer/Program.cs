var builder = WebApplication.CreateBuilder(args);
builder.Services.AddIdentityServer();
var app = builder.Build();

app.UseRouting();
app.UseIdentityServer();

app.MapGet("/", () => "Hello World!");

app.Run();
