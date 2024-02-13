using DataBus.Domain;

namespace DataBus.Application;

public interface IGetData
{
    Task<T> GetAsync<T>(ExecutionMoldel internalObject);
    Task<T> QueryStringAsync<T>(ExecutionMoldel internalObject);
}
