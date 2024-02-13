using DataBus.Domain;

namespace DataBus.Application;

public interface IBulkData
{
    Task<T> BulkInsertAsync<T>(ExecutionMoldel internalObject);
}
