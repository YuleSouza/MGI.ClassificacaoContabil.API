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
            //string tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
            //var options = new ClientSecretCredentialOptions
            //{
            //    AdditionallyAllowedTenants = { "*" }
            //};

            //var credentials = new ClientSecretCredential(
            //    tenantId,
            //    Environment.GetEnvironmentVariable("AZURE_CLIENT_ID"),
            //    Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET"), options
            //);
            
            //_secreteClient = new SecretClient(new Uri(_keyVaultUrl), credentials);
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
