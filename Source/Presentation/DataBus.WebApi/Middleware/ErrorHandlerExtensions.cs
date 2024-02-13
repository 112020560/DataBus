using System.Net;
using System.Text.Json;
using DataBus.Application;
using DataBus.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Serilog.Core;

namespace DataBus.WebApi;

public static class ErrorHandlerExtensions
{
    public static void UseErrorHandler(this IApplicationBuilder app, Logger logger)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature == null) return;

                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = contextFeature.Error switch
                {
                    ApiException => (int)HttpStatusCode.InternalServerError,
                    OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    PropertyNullExeption => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var errorResponse = new BackEndResponse
                {
                    ResponseCode = context.Response.StatusCode.ToString(),
                    Message = $"Error al procesar la solicitud [{context.Response.Headers["x-correlation-id"].ToString() ?? context.TraceIdentifier}]",
                    ErrorMessage = contextFeature.Error.GetBaseException().Message,
                };

                logger.Error(contextFeature.Error.GetBaseException(), "Error al procesar la solicitud: [{correlationid}], Error: {error}", context.Response.Headers["x-correlation-id"].ToString() ?? context.TraceIdentifier, errorResponse.ErrorMessage);

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            });
        });
    }
}
