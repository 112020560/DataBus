using Asp.Versioning.Builder;
using DataBus.Application;
using DataBus.Domain;
using MediatR;

namespace DataBus.WebApi;

public static class BackendRouter
{
    public static void UseBackendRoutes(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        app.MapPost("api/v{version:apiVersion}/execute", ExecuteAsync)
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2));

        app.MapPost("api/{version:apiVersion}/execute", ExecuteAsync)
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2));
    }

    static async Task<IResult> ExecuteAsync(IMediator mediator, DataBusRequest request)
    {
        return Results.Ok(await mediator.Send(new DataBusQuery(request)));
    }
}
