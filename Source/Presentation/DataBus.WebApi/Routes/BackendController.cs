using Asp.Versioning.Builder;
using DataBus.Application;
using DataBus.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DataBus.WebApi;

public static class BackendRouter
{
    private static readonly string[] ValidMethods = { "query", "single", "scalar", "execute", "text" };

    public static void UseBackendRoutes(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        // POST /api/v2/{method}
        // method: query | single | scalar | execute
        app.MapPost("api/v{version:apiVersion}/{method}", ExecuteAsync)
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2));

        app.MapPost("api/{version:apiVersion}/{method}", ExecuteAsync)
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2));
    }

    /// <summary>
    /// Ejecuta una operación en base de datos o HTTP externo
    ///
    /// POST /api/v2/query   → QueryAsync (múltiples filas) - StoredProcedure
    /// POST /api/v2/single  → QuerySingleAsync (una fila) - StoredProcedure
    /// POST /api/v2/scalar  → ScalarAsync (un valor) - StoredProcedure
    /// POST /api/v2/execute → ExecuteAsync (INSERT/UPDATE/DELETE) - StoredProcedure
    /// POST /api/v2/text    → QueryAsync (múltiples filas) - SQL directo (CommandType.Text)
    /// </summary>
    static async Task<IResult> ExecuteAsync(
        IMediator mediator,
        [FromRoute] string method,
        DataBusRequest request)
    {
        var normalizedMethod = method.ToLower();

        if (!ValidMethods.Contains(normalizedMethod))
        {
            return Results.BadRequest(new BackEndResponse
            {
                ResponseCode = "400",
                ResponseData = null,
                Message = $"Método '{method}' no válido",
                ErrorMessage = $"Use uno de: {string.Join(", ", ValidMethods)}"
            });
        }

        return Results.Ok(await mediator.Send(new DataBusQuery(request, normalizedMethod)));
    }
}
