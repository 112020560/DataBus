using DataBus.Domain;

namespace DataBus.Application;

public interface IDataBaseRepository
{
    Task<IEnumerable<T>> GetExecutionAsync<T>(ExecutionMoldel executionMoldel);
}
