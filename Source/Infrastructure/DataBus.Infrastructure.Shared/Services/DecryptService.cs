using System.Security.Cryptography;
using System.Text;
using DataBus.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataBus.Infrastructure.Shared;

public class DecryptService : IDecryptService
{
    private readonly ILogger<DecryptService> _logger;
    private readonly byte[] _iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
    private readonly string _encryptionKey;

    public DecryptService(ILogger<DecryptService> logger, IConfiguration configuration)
    {
        _logger = logger;
        // La llave se puede configurar en appsettings, o usar una por defecto
        _encryptionKey = configuration["EncryptionSettings:Key"] ?? "&%#@?,:*";
    }

    public async Task<string> DecryptAsync(string encryptedValue, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(encryptedValue))
        {
            throw new ArgumentException("El valor encriptado no puede ser nulo o vacío", nameof(encryptedValue));
        }

        try
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey[..8]);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedValue);

            using var des = DES.Create();
            des.Key = keyBytes;
            des.IV = _iv;
            des.Padding = PaddingMode.PKCS7;
            des.Mode = CipherMode.CBC;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            await cs.WriteAsync(encryptedBytes.AsMemory(0, encryptedBytes.Length), cancellationToken);
            await cs.FlushFinalBlockAsync(cancellationToken);

            return Encoding.UTF8.GetString(ms.ToArray());
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "El valor no es un Base64 válido");
            throw new InvalidOperationException("El valor encriptado no tiene un formato Base64 válido", ex);
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex, "Error al desencriptar el valor");
            throw new InvalidOperationException("No se pudo desencriptar el valor. Verifique la llave de encriptación", ex);
        }
    }
}
