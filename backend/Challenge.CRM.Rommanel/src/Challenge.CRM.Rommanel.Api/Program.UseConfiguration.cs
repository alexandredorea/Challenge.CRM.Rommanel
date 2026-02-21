using Challenge.CRM.Rommanel.Application;
using Serilog;

namespace Challenge.CRM.Rommanel.Api;

public static class UseConfigurations
{
    /// <summary>
    /// Configura a pipeline de requisições HTTP.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseServices(this WebApplication app)
    {
        app.UseGlobalExceptionFromApplication();
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        return app;
    }
}