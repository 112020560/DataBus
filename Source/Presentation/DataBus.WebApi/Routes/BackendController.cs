using Asp.Versioning.Builder;
using DataBus.Application;
using DataBus.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DataBus.WebApi;

public static class BackendRouter
{
    public static void UseBackendRoutes(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        // Ruta con método de ejecución: /api/v2/query/execute, /api/v2/single/execute, etc.
        app.MapPost("api/v{version:apiVersion}/{method}/execute", ExecuteWithMethodAsync)
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2));

        app.MapPost("api/{version:apiVersion}/{method}/execute", ExecuteWithMethodAsync)
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2));

        // Ruta simple (usa "query" por defecto)
        app.MapPost("api/v{version:apiVersion}/execute", ExecuteAsync)
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2));

        app.MapPost("api/{version:apiVersion}/execute", ExecuteAsync)
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2));
    }

    /// <summary>
    /// Ejecuta con método específico desde URL
    /// POST /api/v2/query/execute   → QueryAsync (múltiples filas)
    /// POST /api/v2/single/execute  → QuerySingleAsync (una fila)
    /// POST /api/v2/scalar/execute  → ScalarAsync (un valor)
    /// POST /api/v2/execute/execute → ExecuteAsync (sin retorno, solo affected rows + output params)
    /// </summary>
    static async Task<IResult> ExecuteWithMethodAsync(
        IMediator mediator,
        [FromRoute] string method,
        DataBusRequest request)
    {
        var validMethods = new[] { "query", "single", "scalar", "execute" };
        var normalizedMethod = method.ToLower();

        if (!validMethods.Contains(normalizedMethod))
        {
            return Results.BadRequest(new
            {
                Error = $"Método '{method}' no válido. Use: query, single, scalar, execute"
            });
        }

        return Results.Ok(await mediator.Send(new DataBusQuery(request, normalizedMethod)));
    }

    /// <summary>
    /// Ejecuta con método "query" por defecto
    /// POST /api/v2/execute
    /// </summary>
    static async Task<IResult> ExecuteAsync(IMediator mediator, DataBusRequest request)
    {
        return Results.Ok(await mediator.Send(new DataBusQuery(request, "query")));
    }
}
