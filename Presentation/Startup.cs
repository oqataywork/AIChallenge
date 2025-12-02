using DomainService;

using Integrations.DeepSeek;

namespace Presentation;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddDeepSeek();
        services.AddHandlers();
    }

    public void Configure(WebApplication app)
    {
        app.UseHttpsRedirection();

        // app.Use(
        //     async (context, next) =>
        //     {
        //         context.Request.EnableBuffering();
        //         using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        //         string? body = await reader.ReadToEndAsync();
        //         Console.WriteLine($"Incoming Request: {body}");
        //         context.Request.Body.Position = 0;
        //
        //         await next();
        //     });

        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
    }
}