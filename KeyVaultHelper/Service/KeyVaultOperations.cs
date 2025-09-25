using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using KeyVaultHelper.Domain.Enums;
using KeyVaultHelper.IOC;
using System.Collections.Concurrent;

namespace KeyVaultHelper.Service
{
    public class KeyVaultOperations : IKeyVaultOperations
    {
        private readonly SecretClient _client;
        private readonly ParallelOptions _parallelOptions;
        public KeyVaultOperations(AuthenticationType identity)
        {
            _parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 20 };
            string keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
            string kvUriText = $"https://{keyVaultName}.vault.azure.net";
            var kvUri = new Uri(kvUriText);
            switch (identity)
            {
                case AuthenticationType.Default:
                    _client = new(kvUri, new DefaultAzureCredential());
                    break;
                case AuthenticationType.Credentials:
                    string tenantId = Environment.GetEnvironmentVariable("TenantId");
                    string clientId = Environment.GetEnvironmentVariable("ClientId");
                    string clientSecret = Environment.GetEnvironmentVariable("ClientSecret");

                    var clientCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

                    _client = new(kvUri, clientCredential);
                    break;
            }
        }

        public async Task<string> GetSecretAsync(string secretName = null, CancellationToken ct = default)
        {
            try
            {
                Response<KeyVaultSecret> result = await _client.GetSecretAsync(secretName ?? Environment.GetEnvironmentVariable("secretName"), cancellationToken: ct);

                return result?.Value?.Value ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ConcurrentDictionary<string, string>> GetSecretsAsync(ConcurrentDictionary<string, string> keyValues)
        {
            try
            {
                List<string> keys = keyValues.Keys.ToList();
                await Parallel.ForEachAsync(keys, _parallelOptions, async (key, ct) =>
                {
                    try
                    {
                        Response<KeyVaultSecret> result = await _client.GetSecretAsync(key, cancellationToken: ct);

                        keyValues[key] = result?.Value?.Value ?? string.Empty;
                    }
                    catch
                    {
                        keyValues[key] = string.Empty;
                    }
                });

                return keyValues;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
