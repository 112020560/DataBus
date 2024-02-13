using DataBus.Domain;

namespace DataBus.Application;

public interface ISetData
{
    Task<T> SetAsync<T>(ExecutionMoldel internalObject);
}
