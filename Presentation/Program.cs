using Presentation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

WebApplication? app = builder.Build();
startup.Configure(app);

app.Run();

namespace Presentation
{
    public partial class Program
    { }
}