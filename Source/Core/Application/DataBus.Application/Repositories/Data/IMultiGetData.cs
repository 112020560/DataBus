using DataBus.Domain;

namespace DataBus.Application;

public interface IMultiGetData
{
    Task<T> MultiGetAsync<T>(ExecutionMoldel internalObject);
}
