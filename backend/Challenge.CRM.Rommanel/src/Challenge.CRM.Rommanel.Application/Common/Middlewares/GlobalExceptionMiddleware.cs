using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace Challenge.CRM.Rommanel.Application.Common.Middlewares;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .Select(e => new ResultError(e.ErrorCode, e.ErrorMessage))
                .ToArray();

            await WriteResponse(context, HttpStatusCode.UnprocessableEntity,
                Result<object>.Fail("Ocorreu um ou mais erro.", errors));
        }
        catch (BusinessRuleException ex)
        {
            await WriteResponse(context, HttpStatusCode.Conflict,
                Result<object>.Fail(ex.Message, ex.Code, ex.Message));
        }
        catch (DomainException ex)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest,
                Result<object>.Fail(ex.Message, ex.Code, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro inesperado: {Message}", ex.Message);

            await WriteResponse(context, HttpStatusCode.InternalServerError,
                Result<object>.InternalError("Ocorreu um erro interno. Tente novamente mais tarde."));
        }
    }

    private static async Task WriteResponse<T>(
        HttpContext context,
        HttpStatusCode statusCode,
        Result<T> result)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}