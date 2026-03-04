using Asp.Versioning.Builder;
using DataBus.Application;
using DataBus.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DataBus.WebApi;

public static class BackendRouter
{
    public static void UseBackendVersion1Routes(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        //GET
        app.MapPost("api/{version:apiVersion}/backend/{tenantid}/get", GetBackendAsync)
         .WithApiVersionSet(apiVersionSet)
         .MapToApiVersion(new Asp.Versioning.ApiVersion(1));

        app.MapPost("api/v{version:apiVersion}/backend/{tenantid}/get", GetBackendAsync)
        .WithApiVersionSet(apiVersionSet)
        .MapToApiVersion(new Asp.Versioning.ApiVersion(1));

        app.MapPost("api/{version:apiVersion}/{tenantid}/get", GetBackendAsync)
        .WithApiVersionSet(apiVersionSet)
        .MapToApiVersion(new Asp.Versioning.ApiVersion(1));

        //Multi
        app.MapPost("api/{version:apiVersion}/backend/{tenantid}/multi", GetBackendAsync)
        .WithApiVersionSet(apiVersionSet)
        .MapToApiVersion(new Asp.Versioning.ApiVersion(1));

        app.MapPost("api/v{version:apiVersion}/backend/{tenantid}/multi", GetBackendAsync)
        .WithApiVersionSet(apiVersionSet)
        .MapToApiVersion(new Asp.Versioning.ApiVersion(1));

        app.MapPost("api/{version:apiVersion}/{tenantid}/multi", GetBackendAsync)
        .WithApiVersionSet(apiVersionSet)
        .MapToApiVersion(new Asp.Versioning.ApiVersion(1));
    }

    public static void UseBackendVersion2Routes(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        // Nuevo endpoint simplificado v2
        app.MapPost("api/v{version:apiVersion}/execute", ExecuteAsync)
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2));

        app.MapPost("api/{version:apiVersion}/execute", ExecuteAsync)
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2));
    }

    static async Task<IResult> GetBackendAsync(IMediator _mediator, [FromRoute] string tenantid, BackEndRequest backendRequest)
    {
        return Results.Ok(await _mediator.Send(new GetQueryData(backendRequest, 1, "get", tenantid)));
    }

    static async Task<IResult> ExecuteAsync(IMediator mediator, DataBusRequest request)
    {
        return Results.Ok(await mediator.Send(new DataBusQuery(request)));
    }
}
