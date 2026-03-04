using DataBus.Domain;

namespace DataBus.Application;

public interface IConnectionProvider
{
    ConnectionConfig? GetConnection(string key);
    bool ConnectionExists(string key);
}
