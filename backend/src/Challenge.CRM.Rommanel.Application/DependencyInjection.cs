using Challenge.CRM.Rommanel.Application.Common.Behaviors;
using Challenge.CRM.Rommanel.Application.Common.Middlewares;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Challenge.CRM.Rommanel.Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplication(this IHostApplicationBuilder builder)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        builder.AddMediatorPattern(assembly);
        builder.AddValidationsBusinessRule(assembly);
        return builder;
    }

    private static void AddValidationsBusinessRule(this IHostApplicationBuilder builder, Assembly assembly)
    {
        // Ordem da pipeline behaviors (importa a ordem): Logging -> Validation -> Handler
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        // FluentValidation
        builder.Services.AddValidatorsFromAssembly(assembly);
    }

    private static void AddMediatorPattern(this IHostApplicationBuilder builder, Assembly assembly)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
    }

    public static IApplicationBuilder UseGlobalExceptionFromApplication(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }
}