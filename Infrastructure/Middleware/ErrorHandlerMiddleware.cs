using Chatbot.Infrastructure.WebEssentials;
using FluentValidation;
using Serilog;
using System.Net;
using System.Text.Json;

namespace Chatbot.Infrastructure.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var responseModel = ex switch
            {
                ValidationException validationException => Results.Problem(
                    string.Join('\n', validationException.Errors.Select(x => x.ErrorMessage))),
                ProblemException problem => Results.Problem(problem.Message, problem.Code.ToString()),
                _ => Results.Problem(ex.Message)
            };
            Log.Logger.Error($"Message: {ex?.Message}\n Stack Trace: {ex?.StackTrace}");
            response.StatusCode = ex switch
            {
                ValidationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError,
            };
            var result = JsonSerializer.Serialize(responseModel);
            await response.WriteAsync(result);
        }
    }
}

