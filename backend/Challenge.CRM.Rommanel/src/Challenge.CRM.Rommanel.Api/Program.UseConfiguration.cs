using Challenge.CRM.Rommanel.Application;
using Scalar.AspNetCore;
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
        app.UseCors("frontend");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            // Fonte: https://scalar.com/products/api-references/integrations/aspnetcore/integration
            app.MapScalarApiReference("/docs", (options, httpContext) =>
            {
                options.WithTitle($"Rommanel Challenge API");
            });
        }

        return app;
    }
}