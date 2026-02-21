using Challenge.CRM.Rommanel.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Challenge.CRM.Rommanel.Api;

public static class AddServices
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        // Serilog
        builder.Host.UseSerilog((ctx, cfg) =>
            cfg.ReadFrom.Configuration(ctx.Configuration));

        // Serviços de Aplicação e Infraestrutura
        builder.AddApplication();

        // Keycloak JWT
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.Authority = builder.Configuration["Keycloak:Authority"];
                opt.Audience = builder.Configuration["Keycloak:Audience"];
                opt.RequireHttpsMetadata =
                    bool.Parse(builder.Configuration["Keycloak:RequireHttpsMetadata"] ?? "true");
            });

        builder.Services.AddAuthorization();
        builder.Services.AddControllers(option =>
        {
            option.RespectBrowserAcceptHeader = true;
            option.ReturnHttpNotAcceptable = true;
            option.AllowEmptyInputInBodyModelBinding = true;
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();
        builder.Services
            .Configure<RouteOptions>(option => { option.LowercaseUrls = true; })
            .Configure<ApiBehaviorOptions>(option => { option.SuppressModelStateInvalidFilter = true; }); //Suprime a validação automática do ModelState para que o FluentValidation seja o único responsável
        builder.Services.ConfigureHttpJsonOptions(option =>
        {
            option.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            option.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        return builder;
    }
}