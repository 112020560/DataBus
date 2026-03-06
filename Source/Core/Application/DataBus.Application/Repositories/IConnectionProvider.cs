using DataBus.Domain;

namespace DataBus.Application;

public interface IConnectionProvider
{
    /// <summary>
    /// Obtiene la configuración de conexión por su llave.
    /// Si IsEncrypted=true, desencripta el ConnectionString automáticamente.
    /// </summary>
    Task<ConnectionConfig?> GetConnectionAsync(string key, CancellationToken cancellationToken = default);

    bool ConnectionExists(string key);
}
