namespace Challenge.CRM.Rommanel.Application.Common.Models;

public sealed class Result
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public object? Data { get; init; }
    public IReadOnlyList<ResultError> Errors { get; init; } = [];

    public static Result Ok(string message = "Processamento realizado com sucesso", object? data = null) =>
        new() { Success = true, Message = message, Data = data };

    public static Result Fail(string message, params ResultError[] errors) =>
        new() { Success = false, Message = message, Errors = errors.ToList() };

    public static Result Forbidden(string message = "Proibido") =>
        Fail(message, new ResultError("FORBIDDEN", message));

    public static Result NotFound(string message = "Não encontrado") =>
        Fail(message, new ResultError("NOT_FOUND", message));

    public static Result Conflict(string message) =>
        Fail(message, new ResultError("CONFLICT", message));

    public static Result Validation(List<ResultError> errors, string message = "Erro de validação") =>
        new() { Success = false, Message = message, Errors = errors };
}

public sealed class Result<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyList<ResultError> Errors { get; init; } = [];

    public static Result<T> Ok(T data, string message = "Processamento realizado com sucesso") =>
        new() { Success = true, Message = message, Data = data };

    public static Result<T> Fail(string message, params ResultError[] errors) =>
        new() { Success = false, Message = message, Errors = errors.ToList() };

    public static Result<T> Fail(string message, string code, string errorMessage) =>
        Fail(message, new ResultError(code, errorMessage));

    public static Result<T> InternalError(string message) =>
        Fail("Ocorreu um erro interno", new ResultError("INTERNAL_SERVER_ERROR", message));

    public static Result<T> Forbidden(string message = "Proibido") =>
        Fail(message, new ResultError("FORBIDDEN", message));

    public static Result<T> NotFound(string message) =>
        Fail("Não encontrado", new ResultError("NOT_FOUND", message));

    public static Result<T> Conflict(string message) =>
        Fail(message, new ResultError("CONFLICT", message));

    public static Result<T> Validation(List<ResultError> errors, string message = "Erro de validação") =>
        new() { Success = false, Message = message, Errors = errors };
}
