using System.Collections.Concurrent;

namespace KeyVaultHelper.IOC
{
    public interface IKeyVaultOperations
    {
        Task<string> GetSecretAsync(string secretName = null, CancellationToken ct = default);
        Task<ConcurrentDictionary<string, string>> GetSecretsAsync(ConcurrentDictionary<string, string> keyValues);
    }
}
