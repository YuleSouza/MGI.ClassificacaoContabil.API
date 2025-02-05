using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Infra.Service.Interfaces;

namespace MGI.ClassificacaoContabil.Infra.Service
{
    public class KeyVaultService : IKeyVaultService
    {
        private string _keyVaultUrl = "https://esg-capex-keyvault.vault.azure.net/";
        private readonly SecretClient _secreteClient;

        public KeyVaultService()
        {
            _secreteClient = new SecretClient(new Uri(_keyVaultUrl), new DefaultAzureCredential());
        }

        public async Task<string> ConsultarSegredo(string nome)
        {
            try
            {
                KeyVaultSecret secret = await _secreteClient.GetSecretAsync(nome);
                return secret.Value;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
