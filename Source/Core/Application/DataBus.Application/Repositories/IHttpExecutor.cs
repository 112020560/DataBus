using DataBus.Domain;

namespace DataBus.Application;

public interface IHttpExecutor
{
    Task<object?> ExecuteAsync(HttpExecutionModel model, CancellationToken cancellationToken);
}
