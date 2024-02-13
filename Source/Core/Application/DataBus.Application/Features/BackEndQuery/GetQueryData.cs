using MediatR;

namespace DataBus.Application;

public sealed class GetQueryData: IRequest<IBackendResponse> 
{
    public readonly BackEndRequest backEndRequest;
    public readonly int backendVersion;
    public readonly string backendAction;
    public readonly string? tenant;
    public GetQueryData(BackEndRequest backEndRequest, int backendVersion, string backendAction, string? tenant)
    {
        this.backEndRequest = backEndRequest;
        this.backendVersion = backendVersion;
        this.backendAction = backendAction;
        this.tenant = tenant;
    }
}
