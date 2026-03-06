namespace DataBus.Application;

public interface IDecryptService
{
    /// <summary>
    /// Desencripta un valor encriptado (como una cadena de conexión)
    /// </summary>
    /// <param name="encryptedValue">Valor encriptado en Base64</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Valor desencriptado</returns>
    Task<string> DecryptAsync(string encryptedValue, CancellationToken cancellationToken = default);
}
