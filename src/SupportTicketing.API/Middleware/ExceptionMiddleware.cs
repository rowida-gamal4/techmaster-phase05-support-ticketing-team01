using System.Net;
using System.Text.Json;

namespace SupportTicketing.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddlware> _Logger;
    public ExceptionMiddleware(RequestDelegate next,Ilogger<ExceptionMiddleware> logger)
    {
        _Logger= logger;
        _next= next;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex,"Resource not found");
            await WriteErrorResponse(context, HttpStatusCode.NotFound, "Resource not found", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _Logger.LogWarning(ex, "Resource not found");
            await WriteErrorResponse(context, HttpStatusCode.NotFound, "Resource not found", ex.Message);
        }catch(ArgumentException ex)
        {
            _Logger.LogWarning(ex, "Invalid request");
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, "Invalid request", ex.Message);
        }catch(UnauthorizedAccessException ex)
        {
            _Logger.LogWarning(ex, "Unauthorized request");
            await WriteErrorResponse(context, HttpStatusCode.Unauthorized, "Unauthorized", ex.Message);
        }
        catch (ForbiddenException ex)
        {
            logger.LogWarning(ex, "Forbidden request",);
            await WriteErrorResponse(context, HttpStatusCode.Forbidden, "Access denied", ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            logger.LogWarning(ex,"Buissness rule voilation ");
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, "Invalid request", ex.Message);
        }
        catch (Exception ex)
        {
            _Logger.LogWarning(ex, "Unhandled exception");
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "An unexpected error occurred", null);
        }
        
    }
    private static async Task WriteErrorResponse(HttpContext context,HttpStatusCode statusCode,string message,string? details)
    {
        if (context.Response.HasStarted)
        {
            return;
        }
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            statusCode = (int)statusCode,
            message,
            details,
            traceId = context.TraceIdentifier
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
